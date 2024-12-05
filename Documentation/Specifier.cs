using System.Linq;
using System.Reflection;

namespace Documentation
{
    public class Specifier<T> : ISpecifier
    {
        public string GetApiDescription()
        {
            return (typeof(T)
                    .GetCustomAttributes(typeof(ApiDescriptionAttribute))
                    .FirstOrDefault() as ApiDescriptionAttribute)?
                .Description;
        }

        public string[] GetApiMethodNames()
        {
            return typeof(T).GetMethods()
                .Where(method => method.CustomAttributes
                           .FirstOrDefault(attr => attr.AttributeType == typeof(ApiMethodAttribute))
                       != null)
                .Select(method => method.Name)
                .ToArray();
        }

        public string GetApiMethodDescription(string methodName)
        {
            return (typeof(T)
                    .GetMethod(methodName)?
                    .GetCustomAttribute(typeof(ApiDescriptionAttribute)) as ApiDescriptionAttribute)?
                .Description;
        }

        public string[] GetApiMethodParamNames(string methodName)
        {
            return typeof(T)
                .GetMethod(methodName)?
                .GetParameters()
                .Select(param => param.Name)
                .ToArray();
        }

        public string GetApiMethodParamDescription(string methodName, string parameterName)
        {
            return (typeof(T)
                    .GetMethod(methodName)?
                    .GetParameters()
                    .FirstOrDefault(param => param.Name == parameterName)?
                    .GetCustomAttribute(typeof(ApiDescriptionAttribute)) as ApiDescriptionAttribute)?
                .Description;
        }

        public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string parameterName)
        {
            var parameterInfo = typeof(T)
                .GetMethod(methodName)?
                .GetParameters()
                .FirstOrDefault(param => param.Name == parameterName);
            return parameterInfo == null
                ? new ApiParamDescription { ParamDescription = new CommonDescription(parameterName) }
                : CreateParamDescription(parameterInfo, methodName, parameterName);
        }

        private ApiParamDescription CreateParamDescription(ParameterInfo parameterInfo, string methodName, string parameterName)
        {
            var intValidationAttribute = parameterInfo
                .GetCustomAttribute(typeof(ApiIntValidationAttribute)) as ApiIntValidationAttribute;
            var requiredAttribute = parameterInfo
                .GetCustomAttribute(typeof(ApiRequiredAttribute)) as ApiRequiredAttribute;
            var paramDescription = new ApiParamDescription
            {
                MaxValue = intValidationAttribute?.MaxValue,
                MinValue = intValidationAttribute?.MinValue,
                ParamDescription = new CommonDescription
                {
                    Description = GetApiMethodParamDescription(methodName, parameterName),
                    Name = parameterName
                }
            };
            if (requiredAttribute != null)
                paramDescription.Required = requiredAttribute.Required;

            return paramDescription;
        }

        public ApiMethodDescription GetApiMethodFullDescription(string methodName)
        {
            return typeof(T)
                .GetMethod(methodName)?
                .GetCustomAttribute(typeof(ApiMethodAttribute)) == null
                ? null
                : new ApiMethodDescription
                {
                    ParamDescriptions = GetApiMethodParamNames(methodName)
                        .Select(paramName => GetApiMethodParamFullDescription(methodName, paramName))
                        .ToArray(),
                    MethodDescription = new CommonDescription
                    {
                        Description = GetApiMethodDescription(methodName),
                        Name = methodName
                    },
                    ReturnDescription = GetApiMethodReturnParam(methodName)
                };
        }

        private ApiParamDescription GetApiMethodReturnParam(string methodName)
        {
            var returnParameter = typeof(T)
                .GetMethod(methodName)?
                .ReturnParameter;
            if (returnParameter?.Name == null)
                return null;

            var returnDescription = CreateParamDescription(returnParameter, methodName, returnParameter.Name);
            returnDescription.ParamDescription.Name = returnDescription.ParamDescription.Name == ""
                ? null
                : returnDescription.ParamDescription.Name;
            return returnDescription;
        }
    }
}