using System;
using System.Linq;
using System.Reflection;

namespace Documentation
{
    public class Specifier<T> : ISpecifier
    {
        public string GetApiDescription() => 
            typeof(T).GetCustomAttribute<ApiDescriptionAttribute>()?.Description;

        public string[] GetApiMethodNames() =>
            typeof(T).GetMethods()
                .Where(m => m.GetCustomAttribute<ApiMethodAttribute>() != null)
                .Select(m => m.Name).ToArray();

        public string GetApiMethodDescription(string methodName) =>
            typeof(T).GetMethod(methodName)?
                .GetCustomAttribute<ApiDescriptionAttribute>()?.Description;

        public string[] GetApiMethodParamNames(string methodName) =>
            typeof(T).GetMethod(methodName)?.GetParameters()
                .Select(p => p.Name).ToArray();

        public string GetApiMethodParamDescription(string methodName, string paramName) => 
            GetMethodParam(methodName, paramName)?.GetCustomAttribute<ApiDescriptionAttribute>()?.Description;

        public ApiParamDescription GetApiMethodParamFullDescription(string methodName, string paramName) => 
            GetApiMethodParamFullDescription(GetMethodParam(methodName, paramName), methodName, paramName);

        private ApiParamDescription GetApiMethodParamFullDescription(ParameterInfo param, string methodName, string paramName)
        {
            var intValidation = param?.GetCustomAttribute<ApiIntValidationAttribute>();
            return new ApiParamDescription
            {
                MinValue = intValidation?.MinValue,
                MaxValue = intValidation?.MaxValue,
                Required = param?.GetCustomAttribute<ApiRequiredAttribute>()?.Required == true,
                ParamDescription = new CommonDescription(paramName, GetApiMethodParamDescription(methodName, paramName)),
            };
        }

        private ParameterInfo GetMethodParam(string methodName, string paramName) =>
            typeof(T).GetMethod(methodName)?
                .GetParameters().FirstOrDefault(p => p.Name == paramName);

        public ApiMethodDescription GetApiMethodFullDescription(string methodName)
        {
            var method = typeof(T).GetMethod(methodName);
            var isApiMethod = method?.GetCustomAttribute<ApiMethodAttribute>() != null;
            if (!isApiMethod) return null;

            return new ApiMethodDescription
            {
                MethodDescription = new CommonDescription(methodName, GetApiMethodDescription(methodName)),
                ParamDescriptions = GetApiMethodParamNames(methodName)
                    .Select(p => GetApiMethodParamFullDescription(methodName, p)).ToArray(),
                ReturnDescription = method.ReturnType != typeof(void) ? 
                    GetApiMethodParamFullDescription(method.ReturnParameter, methodName, null) :
                    null
            };
        }
    }
}