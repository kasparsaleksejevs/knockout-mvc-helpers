using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
                var url = "api/stuff";
                var methodType = "POST";




                var parameters = method.GetParameters();
                foreach (var parameter in parameters)
                    parStr.Add($"{parameter.Name}: {parameter.ParameterType.Name}");


                parStr.Add("callback: (response: ResponseType) => void");

                var methodBody = $@"
{method.Name} = ({string.Join(", ", parStr)}) => {{
    $.ajax({{ 
        url: '{url}',
        type: '{methodType}',
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
    }
}
