using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace KnockMvc.TypeScriptGenerator
{
    public class TypeScriptControllersGenerator<TGenerateAttribute> where TGenerateAttribute : Attribute
    {
        /// <summary>
        /// The source assembly from which to take classes to generate TypeScript.
        /// </summary>
        private readonly Assembly sourceAssembly = null;

        /// <summary>
        /// The types for which to generate TypeScript.
        /// </summary>
        private readonly IList<Type> types = new List<Type>();

        /// <summary>
        /// The generated data - namespace, contents and content type.
        /// </summary>
        private readonly IList<TypeScriptData> generatedData = new List<TypeScriptData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScriptControllersGenerator{TGenerateAttribute}"/> class.
        /// </summary>
        /// <param name="sourceAssembly">The source assembly.</param>
        public TypeScriptControllersGenerator(Assembly sourceAssembly)
        {
            this.sourceAssembly = sourceAssembly;
        }

        public string Generate()
        {
            foreach (var type in sourceAssembly.GetExportedTypes())
            {
                if (type.GetCustomAttribute<TGenerateAttribute>(false) == null)
                    continue;

                this.types.Add(type);
            }

            foreach (var item in this.types)
                this.GenerateTsClass(item);


            var tempResult = string.Empty;
            foreach (var item in this.generatedData.GroupBy(g => g.Namespace))
            {
                tempResult += "module " + item.Key + " {";
                foreach (var dataItem in item.OrderBy(o => o.ContentType))
                {
                    tempResult += dataItem.Contents;
                }

                tempResult += "}" + Environment.NewLine + Environment.NewLine;
            }

            return tempResult;
        }

        private void GenerateTsClass(Type item)
        {
            var methods = item.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                var methodName = method.Name;
                var parStr = new List<string>();
                var methodType = this.GetActionType(method);
                var url = this.GenerateUrl(item, method, methodType);


                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                    parStr.Add($"{parameter.Name}: {parameter.ParameterType.Name}");

                parStr.Add("callback: (response: ResponseType) => void");

                var methodBody = $@"
    {method.Name} = ({string.Join(", ", parStr)}) => {{
        $.ajax({{ 
            url: `{url}`,
            type: '{methodType.ToString().ToUpperInvariant()}',
            data: 'ss',
            success: (response: IResponseType) => {{
                callback(new ResponseType(response));
            }}
        }};
    }}
    ";

                this.generatedData.Add(new TypeScriptData { Namespace = item.Namespace, Name = item.Name, Contents = methodBody, ContentType = TypeScriptDataContentTypeEnum.TsController });

                /*
                callBackend_ApiGetMyStyff = (data: RequestType, callback: (response: ResponseType) => void) => {
                    return $.ajax({
                        url: 'api/url',
                        type: 'POST',
                        data: data,
                        success: (response: IResponseType) => {
                            callback(new ResponseType());
                        }
                    });
                }
                */

            }
        }

        private ActionTypeEnum GetActionType(MethodInfo action)
        {
            var attributes = action.GetCustomAttributes();
            if (attributes.OfType<HttpGetAttribute>().Any() || action.Name.Equals("Get", StringComparison.InvariantCultureIgnoreCase))
                return ActionTypeEnum.Get;
            else if (attributes.OfType<HttpPostAttribute>().Any() || action.Name.Equals("Post", StringComparison.InvariantCultureIgnoreCase))
                return ActionTypeEnum.Post;
            else if (attributes.OfType<HttpPutAttribute>().Any() || action.Name.Equals("Put", StringComparison.InvariantCultureIgnoreCase))
                return ActionTypeEnum.Put;
            else if (attributes.OfType<HttpDeleteAttribute>().Any() || action.Name.Equals("Delete", StringComparison.InvariantCultureIgnoreCase))
                return ActionTypeEnum.Delete;

            return ActionTypeEnum.Post;
        }

        private enum ActionTypeEnum
        {
            Get,
            Post,
            Put,
            Delete
        }

        private string GenerateUrl(Type controller, MethodInfo action, ActionTypeEnum actionType)
        {
            // by default: 'api/{controller}/{id}'
            var urlDefault = "api/{controller}/{id}";

            var urlParts = urlDefault.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            var recognizedActions = new List<string> { "Get", "Post", "Put", "Delete" };

            var routePrefix = controller.GetCustomAttribute<RoutePrefixAttribute>();
            var routeAttribute = action.GetCustomAttribute<RouteAttribute>();
            var actionParameters = action.GetParameters();

            if (routePrefix == null && routeAttribute == null)
            {
                // easy - use default route
                if (!urlParts.Contains("{action}") && !recognizedActions.Any(m => m.Equals(action.Name, StringComparison.InvariantCultureIgnoreCase)))
                    urlParts.InsertAfter("{controller}", action.Name);

                var controllerName = this.GetControllerNameShortened(controller.Name);
                urlParts.UpdatePart("{controller}", controllerName);

                if (actionParameters.Length > 0 && actionParameters.All(m => m.Name.Equals("id", StringComparison.InvariantCultureIgnoreCase)))
                    urlParts.UpdatePart("{id}", "${id}");
                else
                    urlParts.RemovePart("{id}");
            }
            else
            {
                if (routePrefix != null)
                    urlParts = routePrefix.Prefix.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                else
                {
                    urlParts.RemoveAllPartsAfterPart("{controller}");
                    urlParts.UpdatePart("{controller}", this.GetControllerNameShortened(controller.Name));
                }

                if (routeAttribute != null)
                    urlParts.AddRange(routeAttribute.Template.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
                else
                {
                    if (!recognizedActions.Any(m => m.Equals(action.Name, StringComparison.InvariantCultureIgnoreCase)))
                        urlParts.Add(action.Name);
                }
            }

            return string.Join("/", urlParts);
        }

        private string GetControllerNameShortened(string controllerFullName)
        {
            int contrLen = "Controller".Length;
            if (controllerFullName.EndsWith("Controller"))
                controllerFullName = controllerFullName.Remove(controllerFullName.Length - contrLen, contrLen);

            return controllerFullName;
        }
    }

    public static class UrlExtensions
    {
        /// <summary>
        /// Updates the URL part with specified name to specified value.
        /// If multiple parts have the same name - they are all updated.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <param name="partName">Name of the part.</param>
        /// <param name="value">The target value.</param>
        /// <returns>URL parts list (for chaining).</returns>
        public static List<string> UpdatePart(this List<string> list, string partName, string value)
        {
            for (int i = 0; i < list.Count; i++)
                if (list[i].Equals(partName, StringComparison.InvariantCultureIgnoreCase))
                    list[i] = value;

            return list;
        }

        public static List<string> InsertAfter(this List<string> list, string partName, string value)
        {
            var index = list.FindIndex(m => m.Equals(partName, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
                list.Insert(index + 1, value);

            return list;
        }

        public static List<string> RemovePart(this List<string> list, string partName)
        {
            var index = list.FindIndex(m => m.Equals(partName, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
                list.RemoveAt(index);

            return list;
        }

        public static List<string> RemoveAllPartsAfterPart(this List<string> list, string partName)
        {
            var index = list.FindIndex(m => m.Equals(partName, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
                list.RemoveRange(index + 1, list.Count - index - 1);

            return list;
        }
    }
}
