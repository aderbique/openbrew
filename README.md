OpenBrew
=========

OpenBrew is a free and open source homebrew recipe workspace for building, cloning, sharing, and tracking beer recipes.

It is a modernized fork of Brewgr.com with the current work focused on keeping the app usable on a local development stack, cleaning up legacy view and route issues, and making configuration easier through environment variables.

Highlights
----------

- Build, clone, and edit beer recipes
- Track brew sessions and recipe comments
- Upload and manage recipe photos
- Search recipes and browse style guides
- Send feedback and contact messages through the site
- Reset passwords by email when SMTP is configured
- Run locally with Docker or directly in Visual Studio/IIS Express

Recent updates
-------------

- Added Docker support for the web app and SQL Server Edge
- Moved host, database, and media path settings to environment variables
- Added SMTP configuration via environment variables for password reset and contact email
- Fixed several Mono/ASP.NET MVC view lookup issues that were breaking pages on Linux/macOS containers
- Fixed recipe photo uploads and cleanup so deleted recipes also remove their stored images and broken photo-stream links stop piling up
- Cleaned up the site footer and about navigation so source links live only on the about page
- Replaced the Facebook login flow with Sign in with Google and removed the old Facebook auth assets
- Renamed the web core project to `Openbrew.Web.Core` to match the rest of the refactor
- Updated the site branding from "OpenBrew recipe finder (beta)" to "Advanced Search"

Getting started
---------------

### Docker

The fastest way to run OpenBrew locally is with the provided Docker setup:

```bash
docker compose up --build
```

By default this starts:

- `web` on `http://localhost:8085`
- `db` on SQL Server Edge at `localhost:1433`

Useful environment variables:

- `OPENBREW_HOST_PORT`
- `OPENBREW_HOST_NAME`
- `OPENBREW_DB_NAME`
- `OPENBREW_SA_PASSWORD`
- `OPENBREW_REPO_ROOT`
- `OPENBREW_WEB_ROOT`
- `OPENBREW_CONNECTION_STRING`
- `OPENBREW_BLOG_CONNECTION_STRING`
- `OPENBREW_ROOT_URL`
- `OPENBREW_ROOT_URL_SECURE`
- `OPENBREW_STATIC_ROOT_URL`
- `OPENBREW_STATIC_ROOT_URL_SECURE`
- `OPENBREW_MEDIA_PHYSICAL_ROOT`
- `Environment`
- `SmtpHost`
- `SmtpPort`
- `SmtpUserName`
- `SmtpPassword`

For local development, copy `.openbrew.dev.env.example` to `.openbrew.dev.env`. Set `OPENBREW_SA_PASSWORD` (required), then add SMTP and Google OAuth values as needed. `scripts/run-dev.sh` reads that ignored file when it creates `brewgr-web`; it intentionally contains no credential fallbacks. Outgoing OpenBrew messages use `info@openbrew.net` as the sender.

For the cluster deployment, keep using the four external Docker secrets referenced by `docker-stack.dev.yml`: `openbrew_smtp_host`, `openbrew_smtp_port`, `openbrew_smtp_username`, and `openbrew_smtp_password`.

If you want password reset and contact email to work, set the SMTP values to a real mail server or local mail catcher.

### Swarm / Portainer

For cluster deploys, the repo now includes Swarm stack templates:

- `docker-stack.dev.yml` for `dev.openbrew.net`
- `docker-stack.prod.yml` for `openbrew.net`

Both stacks:

- run the web app from a Docker Hub image
- use `mcr.microsoft.com/mssql/server:2022-latest` for SQL Server
- keep uploaded media and SQL data on persistent storage
- dev uses `/var/lib/openbrew-dev`
- prod keeps the current `/volume1/docker/openbrew-prod` path until you decide to move it too
- route through the existing `traefik-net` overlay network
- bootstrap the database from the schema script inside the app image

To deploy them from Portainer or `docker stack deploy`, provide these environment values:

- `OPENBREW_IMAGE` for the app image, for example `yourdockerhubuser/openbrew-web:dev`
- `OPENBREW_SA_PASSWORD`
- `OPENBREW_DB_NAME`
- `OPENBREW_HOST_PORT`
- `OPENBREW_CONNECTION_STRING`
- `OPENBREW_BLOG_CONNECTION_STRING`
- `OPENBREW_ROOT_URL`
- `OPENBREW_ROOT_URL_SECURE`
- `OPENBREW_STATIC_ROOT_URL`
- `OPENBREW_STATIC_ROOT_URL_SECURE`
- `OPENBREW_MEDIA_PHYSICAL_ROOT`
- `GOOGLE_APPLICATION_KEY`
- `GOOGLE_APPLICATION_SECRET`
- `openbrew_smtp_host` secret
- `openbrew_smtp_port` secret
- `openbrew_smtp_username` secret
- `openbrew_smtp_password` secret

The stack templates mount those four secrets into `/run/secrets/...` and expose them to the app through `SMTP_*_FILE` environment variables. The app reads those files directly, so the SMTP password never has to appear in the stack YAML or in a plain environment variable.

The workflow in `.github/workflows/dockerhub.yml` publishes the app image to Docker Hub using `DOCKERHUB_USERNAME` and `DOCKERHUB_TOKEN`. It publishes `sha-<commit>` for every configured branch push, `latest` and `prod` from `master`/`main`, `dev` from development branches, and both `vX.Y.Z` and `X.Y.Z` for a release tag.

#### Cluster migration checklist

Keep real credentials in your cluster's secret store (or a Portainer/Docker secret), not in either stack YAML file. The stack templates deliberately reference `GOOGLE_APPLICATION_KEY` and `GOOGLE_APPLICATION_SECRET`; `GOOGLE_APPLICATION_KEY` is the Google OAuth client ID.

Recipe photos are both written to and served from the application `Media` directory. Mount the same persistent volume at this exact in-container path:

```text
/workspace/brewgr/Openbrew.Web/Media
```

Set `OPENBREW_MEDIA_PHYSICAL_ROOT` to that same path. Mounting the volume only at a separate path such as `/data/media` lets uploads report success but leaves `/Media/...` image URLs unreachable after a refresh.

For Google Sign in, configure these authorized JavaScript origins on the OAuth web client as applicable:

```text
http://localhost:8085
https://dev.openbrew.net
https://openbrew.net
```

The retained OAuth callback endpoints are `https://dev.openbrew.net/Auth/OAuthLogin` and `https://openbrew.net/Auth/OAuthLogin`. The current Google Identity Services button posts its credential to the same-origin `/Auth/GoogleLogin` endpoint; that URL is not an OAuth redirect URI.

Before increasing web replicas, ensure every replica can reach the same media volume and that your session/authentication setup is appropriate for more than one instance. The provided Swarm templates intentionally pin the web service to one node.

### Visual Studio / IIS Express

1. Clone the repository.
2. Restore packages.
3. Set `OPENBREW_CONNECTION_STRING` to a valid SQL Server connection string.
4. Make sure `dev.openbrew.local` points to `127.0.0.1` if you are using the host name based dev setup.
5. Open `Openbrew.Web.sln` and run the web project.

Database setup
--------------

If you are starting from scratch, follow the database instructions in:

- [Setup/Database/README.md](Setup/Database/README.md)

Source and credit
-----------------

- Current source: https://github.com/aderbique/openbrew
- Original source: https://github.com/rak-phillip/Brewgr.com

Technologies
------------

- Microsoft ASP.NET MVC
- Microsoft Entity Framework
- AutoMapper
- Ninject
- Exceptional
- Fluent Validation
- Image Resizer
- jQuery
- Various jQuery plugins

License
-------

OpenBrew is copyright (c) 2011-2015 Matthew Marksbury, Jason Zimmerman, and other contributors under the GNU General Public License v3.0.
