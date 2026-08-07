using System;
using System.Linq;
using ctorx.Core.Validation;
using FluentValidation;

namespace Openbrew.Web
{
	public class ViewModelValidatorFactory : IValidatorFactory
	{
		/// <summary>
		/// Gets the validator for the specified type.
		/// </summary>
		public IValidator<T> GetValidator<T>()
		{
			return this.GetValidator(typeof(T)) as IValidator<T>;
		}

		/// <summary>
		/// Gets the validator for the specified type.
		/// </summary>
		public IValidator GetValidator(Type type)
		{
			if (!(typeof(IValidatingViewModel).IsAssignableFrom(type)))
			{
				// Try to Find a Validator by convention (i.e. TypeNameValidator)
				// If Found, return an instance of it
				var match = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(x => x.GetTypes())
					.FirstOrDefault(x => x.Name == string.Concat(type.Name, "Validator"));

				if(match != null)
				{
					if(typeof(IValidator).IsAssignableFrom(match))
					{
						return match.GetConstructors()[0].Invoke(new object[] { }) as IValidator;
					}
				}

				return null;
			}

			var genericTypes = type.BaseType.GetGenericArguments();

			if (genericTypes.Length == 0)
			{
				return null;
			}

			var validatorType = genericTypes[0];

			if (!(typeof(IValidator).IsAssignableFrom(validatorType)))
			{
				return null;
			}

			return validatorType.GetConstructors()[0].Invoke(new object[] { }) as IValidator;
		}
	}
}