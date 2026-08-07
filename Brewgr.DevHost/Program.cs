using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.Win32.SafeHandles;
using ctorx.Core.Configuration;
using Openbrew.Web.App_Start;

namespace Brewgr.DevHost
{
	internal static class Program
	{
		static int Main(string[] args)
		{
			var webRoot = ResolveWebRoot(args);
			var port = ResolvePort(args);
			var hostName = GetEnv("OPENBREW_HOST_NAME", "localhost", "BREWGR_HOST_NAME");

			var rootUrl = string.Format("http://{0}:{1}", hostName, port);
			Environment.SetEnvironmentVariable("OPENBREW_ROOT_URL", rootUrl);
			Environment.SetEnvironmentVariable("OPENBREW_ROOT_URL_SECURE", rootUrl);
			Environment.SetEnvironmentVariable("OPENBREW_STATIC_ROOT_URL", rootUrl);
			Environment.SetEnvironmentVariable("OPENBREW_STATIC_ROOT_URL_SECURE", rootUrl);

			var mediaRoot = Path.Combine(webRoot, "Media");
			Environment.SetEnvironmentVariable("OPENBREW_MEDIA_PHYSICAL_ROOT", mediaRoot);
			Environment.CurrentDirectory = webRoot;
			var binRoot = Path.Combine(webRoot, "bin");
			AppDomain.CurrentDomain.SetData(".appPath", webRoot + Path.DirectorySeparatorChar);
			AppDomain.CurrentDomain.SetData(".appVPath", "/");
			AppDomain.CurrentDomain.SetData(".appId", "Brewgr.DevHost");
			AppDomain.CurrentDomain.SetData(".hostingInit", true);
			AppDomain.CurrentDomain.SetData(".hostingEnvironment", true);
			AppDomain.CurrentDomain.SetData("APPBASE", webRoot + Path.DirectorySeparatorChar);
			AppDomain.CurrentDomain.SetData("BINPATH", binRoot);
			AppDomain.CurrentDomain.FirstChanceException += (sender, eventArgs) =>
			{
				var exception = eventArgs.Exception;
				if (exception == null)
				{
					return;
				}

				var message = exception.Message ?? string.Empty;
				if (exception is ApplicationException || message.IndexOf("Error compiling application file", StringComparison.OrdinalIgnoreCase) >= 0)
				{
					Console.Error.WriteLine(exception);
				}
			};

			Console.WriteLine("Starting Brewgr on {0}", rootUrl);
			Console.WriteLine("Web root: {0}", webRoot);
			Console.WriteLine("Media root: {0}", mediaRoot);

			NinjectWebCommon.Start();

			var prefix = string.Format("http://{0}:{1}/", hostName, port);
			var listener = new HttpListener();
			listener.Prefixes.Add(prefix);
			listener.Start();

			Console.WriteLine("Listening on {0}", prefix);
			Console.WriteLine("Press Ctrl+C to stop.");

			var done = new ManualResetEvent(false);
			Console.CancelKeyPress += (sender, eventArgs) =>
			{
				eventArgs.Cancel = true;
				done.Set();
				try { listener.Stop(); } catch { }
			};

			try
			{
				while (!done.WaitOne(0))
				{
					HttpListenerContext context;

					try
					{
						context = listener.GetContext();
					}
					catch (HttpListenerException)
					{
						break;
					}
					catch (ObjectDisposedException)
					{
						break;
					}

					ThreadPool.QueueUserWorkItem(_ =>
					{
						try
						{
							ProcessRequest(context, webRoot);
						}
						catch (Exception ex)
						{
							Console.Error.WriteLine(ex);
							if (context.Response != null)
							{
								try
								{
									context.Response.StatusCode = 500;
									context.Response.StatusDescription = "Internal Server Error";
									using (var writer = new StreamWriter(context.Response.OutputStream, Encoding.UTF8))
									{
										writer.Write("Internal Server Error");
									}
								}
								catch
								{
									// Ignore secondary failures while returning the error response.
								}
							}
						}
					});
				}
			}
			finally
			{
				try
				{
					NinjectWebCommon.Stop();
				}
				catch
				{
				}

				if (listener.IsListening)
				{
					listener.Stop();
				}

				listener.Close();
			}

			return 0;
		}

		static void ProcessRequest(HttpListenerContext context, string webRoot)
		{
			var workerRequest = new ListenerWorkerRequest(context, webRoot);
			HttpRuntime.ProcessRequest(workerRequest);
		}

		static string GetEnv(string name, string fallback, string legacyName = null)
		{
			var value = ConfigReader.EnvironmentVariables.Read(name, legacyName);
			return string.IsNullOrWhiteSpace(value) ? fallback : value;
		}

		static string ResolveWebRoot(string[] args)
		{
			var envRoot = ConfigReader.EnvironmentVariables.Read("OPENBREW_WEB_ROOT", "BREWGR_WEB_ROOT");
			if (!string.IsNullOrWhiteSpace(envRoot))
			{
				return Path.GetFullPath(envRoot);
			}

			if (args.Length > 0 && !string.IsNullOrWhiteSpace(args[0]))
			{
				return Path.GetFullPath(args[0]);
			}

			var appBase = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
			return Path.GetFullPath(Path.Combine(appBase, "..", "..", "Openbrew.Web"));
		}

		static int ResolvePort(string[] args)
		{
			int port;
			if (int.TryParse(ConfigReader.EnvironmentVariables.Read("OPENBREW_HOST_PORT", "BREWGR_HOST_PORT"), out port))
			{
				return port;
			}

			if (args.Length > 1 && int.TryParse(args[1], out port))
			{
				return port;
			}

			return 8085;
		}
	}

	internal sealed class ListenerWorkerRequest : HttpWorkerRequest
	{
		readonly HttpListenerContext Context;
		readonly string WebRoot;
		readonly byte[] EntityBody;
		readonly Dictionary<string, string> ResponseHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		int StatusCode = 200;
		string StatusDescription = "OK";
		bool ResponseStarted;

		public ListenerWorkerRequest(HttpListenerContext context, string webRoot)
		{
			this.Context = context;
			this.WebRoot = webRoot;

			using (var ms = new MemoryStream())
			{
				context.Request.InputStream.CopyTo(ms);
				this.EntityBody = ms.ToArray();
			}
		}

		public override void EndOfRequest()
		{
			if (!this.Context.Response.OutputStream.CanWrite)
			{
				return;
			}

			try
			{
				this.Context.Response.OutputStream.Flush();
			}
			catch
			{
			}

			try
			{
				this.Context.Response.Close();
			}
			catch
			{
			}
		}

		public override void FlushResponse(bool finalFlush)
		{
			try
			{
				this.Context.Response.OutputStream.Flush();
			}
			catch
			{
			}
		}

		public override string GetHttpVerbName()
		{
			return this.Context.Request.HttpMethod;
		}

		public override string GetHttpVersion()
		{
			return string.Format("HTTP/{0}.{1}", this.Context.Request.ProtocolVersion.Major, this.Context.Request.ProtocolVersion.Minor);
		}

		public override string GetLocalAddress()
		{
			return this.Context.Request.Url.Host;
		}

		public override int GetLocalPort()
		{
			return this.Context.Request.Url.Port;
		}

		public override string GetQueryString()
		{
			var query = this.Context.Request.Url.Query;
			return string.IsNullOrEmpty(query) ? string.Empty : query.TrimStart('?');
		}

		public override string GetRawUrl()
		{
			return this.Context.Request.RawUrl;
		}

		public override string GetRemoteAddress()
		{
			return this.Context.Request.RemoteEndPoint != null ? this.Context.Request.RemoteEndPoint.Address.ToString() : "127.0.0.1";
		}

		public override int GetRemotePort()
		{
			return this.Context.Request.RemoteEndPoint != null ? this.Context.Request.RemoteEndPoint.Port : 0;
		}

		public override string GetUriPath()
		{
			return this.Context.Request.Url.AbsolutePath;
		}

		public override void SendKnownResponseHeader(int index, string value)
		{
			var name = HttpWorkerRequest.GetKnownResponseHeaderName(index);
			if (string.IsNullOrWhiteSpace(name))
			{
				return;
			}

			this.WriteResponseHeader(name, value);
		}

		public override void SendResponseFromFile(IntPtr handle, long offset, long length)
		{
			using (var fileHandle = new SafeFileHandle(handle, false))
			using (var stream = new FileStream(fileHandle, FileAccess.Read))
			{
				this.CopyStream(stream, offset, length);
			}
		}

		public override void SendResponseFromFile(string filename, long offset, long length)
		{
			using (var stream = File.OpenRead(filename))
			{
				this.CopyStream(stream, offset, length);
			}
		}

		public override void SendResponseFromMemory(byte[] data, int length)
		{
			this.ResponseStarted = true;
			this.Context.Response.OutputStream.Write(data, 0, length);
		}

		public override void SendStatus(int statusCode, string statusDescription)
		{
			this.StatusCode = statusCode;
			this.StatusDescription = statusDescription;
			this.Context.Response.StatusCode = statusCode;
			this.Context.Response.StatusDescription = statusDescription;
		}

		public override void SendUnknownResponseHeader(string name, string value)
		{
			this.WriteResponseHeader(name, value);
		}

		public override string GetAppPath()
		{
			return "/";
		}

		public override string GetAppPathTranslated()
		{
			return this.WebRoot;
		}

		public override string GetFilePath()
		{
			return this.Context.Request.Url.AbsolutePath;
		}

		public override string GetFilePathTranslated()
		{
			return this.MapPath(this.GetFilePath());
		}

		public override string GetPathInfo()
		{
			return string.Empty;
		}

		public override string GetServerVariable(string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return string.Empty;
			}

			switch (name.ToUpperInvariant())
			{
				case "HTTP_HOST":
					return this.Context.Request.Url.Authority;
				case "HTTPS":
					return this.Context.Request.Url.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ? "on" : "off";
				case "LOCAL_ADDR":
					return this.GetLocalAddress();
				case "LOCAL_PORT":
					return this.GetLocalPort().ToString();
				case "REMOTE_ADDR":
					return this.GetRemoteAddress();
				case "REMOTE_PORT":
					return this.GetRemotePort().ToString();
				case "REQUEST_METHOD":
					return this.Context.Request.HttpMethod;
				case "SERVER_NAME":
					return this.Context.Request.Url.Host;
				case "SERVER_PORT":
					return this.Context.Request.Url.Port.ToString();
				case "SERVER_PROTOCOL":
					return this.GetHttpVersion();
				case "URL":
					return this.Context.Request.Url.AbsolutePath;
				default:
					return this.Context.Request.Headers[name] ?? string.Empty;
			}
		}

		public override string GetKnownRequestHeader(int index)
		{
			var headerName = HttpWorkerRequest.GetKnownRequestHeaderName(index);
			return headerName != null ? this.Context.Request.Headers[headerName] : null;
		}

		public override string GetUnknownRequestHeader(string name)
		{
			return this.Context.Request.Headers[name];
		}

		public override string[][] GetUnknownRequestHeaders()
		{
			var knownHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			for (var i = 0; i < 41; i++)
			{
				var headerName = HttpWorkerRequest.GetKnownRequestHeaderName(i);
				if (!string.IsNullOrWhiteSpace(headerName))
				{
					knownHeaders.Add(headerName);
				}
			}

			return this.Context.Request.Headers.AllKeys
				.Where(key => !string.IsNullOrWhiteSpace(key) && !knownHeaders.Contains(key))
				.Select(key => new[] { key, this.Context.Request.Headers[key] })
				.ToArray();
		}

		public override byte[] GetPreloadedEntityBody()
		{
			return this.EntityBody.Length > 0 ? this.EntityBody : null;
		}

		public override int GetPreloadedEntityBody(byte[] buffer, int offset)
		{
			if (buffer == null || this.EntityBody.Length == 0)
			{
				return 0;
			}

			var count = Math.Min(buffer.Length - offset, this.EntityBody.Length);
			Buffer.BlockCopy(this.EntityBody, 0, buffer, offset, count);
			return count;
		}

		public override int GetPreloadedEntityBodyLength()
		{
			return this.EntityBody.Length;
		}

		public override int GetTotalEntityBodyLength()
		{
			return this.EntityBody.Length;
		}

		public override bool IsEntireEntityBodyIsPreloaded()
		{
			return true;
		}

		public bool HasEntityBody()
		{
			return this.EntityBody.Length > 0;
		}

		public override int ReadEntityBody(byte[] buffer, int size)
		{
			return this.ReadEntityBody(buffer, 0, size);
		}

		public override int ReadEntityBody(byte[] buffer, int offset, int size)
		{
			if (buffer == null || size <= 0 || this.EntityBody.Length == 0)
			{
				return 0;
			}

			var count = Math.Min(size, this.EntityBody.Length);
			Buffer.BlockCopy(this.EntityBody, 0, buffer, offset, count);
			return count;
		}

		public override string MapPath(string path)
		{
			if (string.IsNullOrWhiteSpace(path) || path == "/")
			{
				return this.WebRoot;
			}

			var relative = path.TrimStart('~', '/', '\\').Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
			return Path.GetFullPath(Path.Combine(this.WebRoot, relative));
		}

		public override bool HeadersSent()
		{
			return this.ResponseStarted;
		}

		void CopyStream(Stream source, long offset, long length)
		{
			this.ResponseStarted = true;

			if (offset > 0)
			{
				source.Seek(offset, SeekOrigin.Begin);
			}

			var remaining = length >= 0 ? length : long.MaxValue;
			var buffer = new byte[64 * 1024];

			while (remaining > 0)
			{
				var read = source.Read(buffer, 0, remaining < buffer.Length ? (int)remaining : buffer.Length);
				if (read <= 0)
				{
					break;
				}

				this.Context.Response.OutputStream.Write(buffer, 0, read);
				remaining -= read;
			}
		}

		void WriteResponseHeader(string name, string value)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				return;
			}

			if (string.Equals(name, "Content-Type", StringComparison.OrdinalIgnoreCase))
			{
				this.Context.Response.ContentType = value;
				return;
			}

			if (string.Equals(name, "Content-Length", StringComparison.OrdinalIgnoreCase))
			{
				long contentLength;
				if (long.TryParse(value, out contentLength))
				{
					this.Context.Response.ContentLength64 = contentLength;
				}

				return;
			}

			if (string.Equals(name, "Location", StringComparison.OrdinalIgnoreCase))
			{
				this.Context.Response.RedirectLocation = value;
				return;
			}

			this.Context.Response.AddHeader(name, value);
		}
	}
}
