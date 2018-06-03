using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KnockMvc.TypeScriptGenerator
{
    /// <summary>
    /// TypeScript generator class.
    /// Supports generation of TS classes and enums (with text descriptions).
    /// </summary>
    /// <typeparam name="TGenerateAttribute">The type of the TS generation attribute.</typeparam>
    public partial class TypeScriptClassGenerator<TGenerateAttribute> where TGenerateAttribute : Attribute
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
        /// Initializes a new instance of the <see cref="TypeScriptClassGenerator{TGenerateAttribute}"/> class.
        /// </summary>
        /// <param name="sourceAssembly">The source assembly from which to take types to generate.</param>
        public TypeScriptClassGenerator(Assembly sourceAssembly)
        {
            this.sourceAssembly = sourceAssembly;
        }

        /// <summary>
        /// Generates the TypeScript code.
        /// </summary>
        /// <returns>Currently for testing - full generator output.</returns>
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

            // ============== // ============== // ============== // ============== 
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
            // ============== // ============== // ============== // ============== 
        }

        /// <summary>
        /// Generates the ts class.
        /// </summary>
        /// <param name="item">The item.</param>
        private void GenerateTsClass(Type item)
        {
            if (this.generatedData.Any(m => m.Namespace == item.Namespace && m.Name == item.Name))
                return;

            var sbClass = new StringBuilder();
            var sbInterface = new StringBuilder();
            sbClass.AppendLine();
            sbClass.AppendLine($"    export class {item.Name} {{");

            sbInterface.AppendLine();
            sbInterface.AppendLine($"    export interface I{item.Name} {{");

            foreach (var prop in item.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                sbClass.AppendLine($"        {prop.Name} = {this.GetPropertyDefinition(prop)}");
                sbInterface.AppendLine($"        {prop.Name}{this.GetPropertyInterfaceDefinition(prop)}");
            }

            // add constructor
            sbClass.AppendLine();
            sbClass.AppendLine($"        constructor(p?: I{item.Name}) {{");
            sbClass.AppendLine("            if (p) {");
            foreach (var prop in item.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (this.IsPropertyArray(prop.PropertyType))
                {
                    Type arrayElementType;
                    if (prop.PropertyType.IsGenericType)
                        arrayElementType = prop.PropertyType.GetGenericArguments()[0];
                    else
                        arrayElementType = prop.PropertyType.GetElementType();

                    if (arrayElementType.GetCustomAttribute<TGenerateAttribute>(false) != null)
                    {
                        sbClass.AppendLine($"                this.{prop.Name}([]);");
                        sbClass.AppendLine($"                p.{prop.Name}.forEach((item) => {{");
                        sbClass.AppendLine($"                    this.{prop.Name}.push(new {arrayElementType.Name}(item));");
                        sbClass.AppendLine($"                }});");
                    }
                    else
                        sbClass.AppendLine($"                this.{prop.Name}.push(...p.{prop.Name});");
                }
                else
                {
                    if (prop.PropertyType.GetCustomAttribute<TGenerateAttribute>(false) != null && !prop.PropertyType.IsEnum)
                        sbClass.AppendLine($"                this.{prop.Name}(new {prop.PropertyType.Name}(p.{prop.Name}));");
                    else
                        sbClass.AppendLine($"                this.{prop.Name}(p.{prop.Name});");
                }
            }

            sbClass.AppendLine("            }");
            sbClass.AppendLine("        }");
            sbClass.AppendLine("    }");
            sbInterface.AppendLine("    }");

            sbClass.Append(sbInterface);

            this.generatedData.Add(new TypeScriptData { Namespace = item.Namespace, Name = item.Name, Contents = sbClass.ToString(), ContentType = TypeScriptDataContentTypeEnum.TsClass });
        }

        /// <summary>
        /// Gets the property definition in the form 'ko.observable&lt;number&gt;>();'.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>Property definition</returns>
        private string GetPropertyDefinition(PropertyInfo property)
        {
            // ToDo: default values?
            // ToDo: cycle checking for custom classes

            var propertyType = property.PropertyType;

            // is it an Array?
            var isArray = this.IsPropertyArray(propertyType);
            if (isArray)
            {
                if (propertyType.IsGenericType)
                    propertyType = propertyType.GetGenericArguments()[0];
                else
                    propertyType = propertyType.GetElementType();
            }

            var tsPropertyType = "string";

            // is it a subclass?
            if (propertyType.GetCustomAttribute<TGenerateAttribute>(false) != null && !propertyType.IsEnum)
            {
                this.GenerateTsClass(propertyType);
                tsPropertyType = propertyType.Name;
            }
            else
            {
                // is it an Enum?
                if (propertyType.IsEnum)
                    return this.GenerateEnum(propertyType);

                if (propertyType == typeof(int) || propertyType == typeof(int?)
                    || propertyType == typeof(decimal) || propertyType == typeof(decimal?)
                    || propertyType == typeof(double) || propertyType == typeof(double?)
                    || propertyType == typeof(float) || propertyType == typeof(float?))
                    tsPropertyType = "number";

                if (propertyType == typeof(string))
                    tsPropertyType = "string";

                if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    tsPropertyType = "boolean";

                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    tsPropertyType = "Date";
            }

            var koType = isArray ? "observableArray" : "observable";
            return $"ko.{koType}<{tsPropertyType}>();";
        }

        /// <summary>
        /// Gets the property definition in the form 'ko.observable&lt;number&gt;>();'.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>Property definition</returns>
        private string GetPropertyInterfaceDefinition(PropertyInfo property)
        {
            var propertyType = property.PropertyType;

            // is it an Array?
            var isArray = false;
            if (typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string))
            {
                if (propertyType.IsGenericType)
                    propertyType = propertyType.GetGenericArguments()[0];
                else
                    propertyType = propertyType.GetElementType();

                isArray = true;
            }

            var tsPropertyType = "string";
            var nullableSymbol = string.Empty;

            if (Nullable.GetUnderlyingType(propertyType) != null)
                nullableSymbol = "?";

            // is it a subclass?
            if (propertyType.GetCustomAttribute<TGenerateAttribute>(false) != null && !propertyType.IsEnum)
                tsPropertyType = "I" + propertyType.Name;
            else if (propertyType.IsEnum)
                tsPropertyType = "number";
            else
            {
                if (propertyType == typeof(int) || propertyType == typeof(int?)
                    || propertyType == typeof(decimal) || propertyType == typeof(decimal?)
                    || propertyType == typeof(double) || propertyType == typeof(double?)
                    || propertyType == typeof(float) || propertyType == typeof(float?))
                    tsPropertyType = "number";

                if (propertyType == typeof(string))
                    tsPropertyType = "string";

                if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    tsPropertyType = "boolean";

                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    tsPropertyType = "Date";
            }

            var arraySpecifier = isArray ? "[]" : string.Empty;
            return $"{nullableSymbol}: {tsPropertyType}{arraySpecifier};";
        }

        /// <summary>
        /// Generates the TypeScript for enum property.
        /// </summary>
        /// <param name="propertyType">Type of the property (enum type).</param>
        /// <returns>Generated property</returns>
        private string GenerateEnum(Type propertyType)
        {
            if (!this.generatedData.Any(m => m.Namespace == propertyType.Namespace && m.Name == propertyType.Name))
            {
                var sb = new StringBuilder();
                var fields = propertyType.GetFields(BindingFlags.Public | BindingFlags.Static);

                sb.AppendLine();
                sb.AppendLine($"    enum {propertyType.Name} {{");
                foreach (var item in fields)
                    sb.AppendLine($"        {item.Name} = {item.GetRawConstantValue()},");

                sb.AppendLine("    }");

                sb.AppendLine();
                sb.AppendLine($"    const {propertyType.Name}Text = new Map<number, string>([");
                foreach (var enumField in fields)
                {
                    var enumDescription = string.Empty;
                    if (enumField.GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttr)
                        enumDescription = displayAttr.Name;
                    else
                    {
                        // for legacy MVC3+ projects the DescriptionAttribute is usually used
                        if (enumField.GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute descriptionAttr)
                            enumDescription = descriptionAttr.Description;
                        else
                            enumDescription = enumField.Name;
                    }

                    sb.AppendLine($"        [{propertyType.Name}.{enumField.Name}, '{enumDescription}'],");
                }

                sb.AppendLine("    ]);");
                // enum description call example: 'var textVal = MyEnumText.get(MyEnum.Val2); // will get "Other (Descr)"'

                this.generatedData.Add(new TypeScriptData { Namespace = propertyType.Namespace, Name = propertyType.Name, Contents = sb.ToString(), ContentType = TypeScriptDataContentTypeEnum.TsEnum });
            }

            var nullableSymbol = string.Empty;
            if (Nullable.GetUnderlyingType(propertyType) != null)
                nullableSymbol = "?";

            return $"ko.observable<{propertyType.Name}{nullableSymbol}>();";
        }

        /// <summary>
        /// Determines whether the specified property type is an array.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns>
        ///   <c>True</c> if the specified property type is an array; otherwise, <c>false</c>.
        /// </returns>
        private bool IsPropertyArray(Type propertyType)
        {
            return typeof(IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string);
        }
    }
}
