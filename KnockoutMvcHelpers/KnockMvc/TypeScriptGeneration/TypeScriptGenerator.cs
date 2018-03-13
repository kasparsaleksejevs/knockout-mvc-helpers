using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KnockMvc.TypeScriptGeneration
{
    /// <summary>
    /// TypeScript generator class.
    /// Supports generation of TS classes and enums (with text descriptions).
    /// </summary>
    /// <typeparam name="TGenerateAttribute">The type of the TS generation attribute.</typeparam>
    public class TypeScriptGenerator<TGenerateAttribute> where TGenerateAttribute : Attribute
    {
        /// <summary>
        /// The source assembly from which to take classes to generate TypeScript.
        /// </summary>
        private Assembly sourceAssembly = null;

        /// <summary>
        /// The types for which to generate TypeScript.
        /// </summary>
        private IList<Type> types = new List<Type>();

        /// <summary>
        /// The generated data - namespace, contents and content type.
        /// </summary>
        private IList<TsData> generatedData = new List<TsData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScriptGenerator{TGenerateAttribute}"/> class.
        /// </summary>
        /// <param name="sourceAssembly">The source assembly from which to take types to generate.</param>
        public TypeScriptGenerator(Assembly sourceAssembly)
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

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($"    class {item.Name} {{");

            foreach (var prop in item.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var jsPropertyType = string.Empty;
                jsPropertyType = this.GetPropertyDefinition(prop);

                sb.AppendLine($"        {prop.Name} = {jsPropertyType}");
            }

            sb.AppendLine("    }");

            this.generatedData.Add(new TsData { Namespace = item.Namespace, Name = item.Name, Contents = sb.ToString(), ContentType = ContentTypeEnum.TsClass });
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

            // is it a subclass?
            if (propertyType.GetCustomAttribute<TGenerateAttribute>(false) != null)
            {
                this.GenerateTsClass(propertyType);
                tsPropertyType = propertyType.Name;
            }
            else
            {
                // is it an Enum?
                if (propertyType.IsEnum)
                    return this.GenerateEnum(propertyType);

                if (Nullable.GetUnderlyingType(propertyType) != null)
                    nullableSymbol = "?";

                if (propertyType == typeof(int) || propertyType == typeof(int?)
                        || propertyType == typeof(decimal) || propertyType == typeof(decimal?)
                        || propertyType == typeof(double) || propertyType == typeof(double?)
                        || propertyType == typeof(float) || propertyType == typeof(float?)
                        )
                    tsPropertyType = "number";

                if (propertyType == typeof(string))
                    tsPropertyType = "string";

                if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                    tsPropertyType = "boolean";

                if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                    tsPropertyType = "Date";
            }

            var koType = isArray ? "observableArray" : "observable";
            return $"ko.{koType}<{tsPropertyType}{nullableSymbol}>();";
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
                var underlyingType = Enum.GetUnderlyingType(propertyType);
                var enumValues = Enum.GetValues(propertyType);

                sb.AppendLine();
                sb.AppendLine($"    enum {propertyType.Name} {{");
                foreach (var enumItem in enumValues)
                {
                    var enumValue = Convert.ToInt32(Convert.ChangeType(enumItem, underlyingType));
                    sb.AppendLine($"        {enumItem.ToString()} = {enumValue},");
                }

                sb.AppendLine("    }");

                sb.AppendLine();
                sb.AppendLine($"    const {propertyType.Name}Text = new Map<number, string>([");
                foreach (var enumItem in enumValues)
                {
                    var enumDescription = string.Empty;
                    if (propertyType.GetField(enumItem.ToString()).GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttr)
                        enumDescription = displayAttr.Name;
                    else
                    {
                        // for legacy MVC3+ projects the DescriptionAttribute is usually used
                        if (propertyType.GetField(enumItem.ToString()).GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute descriptionAttr)
                            enumDescription = descriptionAttr.Description;
                        else
                            enumDescription = enumItem.ToString();
                    }

                    var enumValue = Convert.ToInt32(Convert.ChangeType(enumItem, underlyingType));
                    sb.AppendLine($"        [{propertyType.Name}.{enumItem.ToString()}, '{enumDescription}'],");
                }

                sb.AppendLine("    ]);");
                // enum description call example: 'var textVal = MyEnumText.get(MyEnum.Val2); // will get "Other (Descr)"'

                this.generatedData.Add(new TsData { Namespace = propertyType.Namespace, Name = propertyType.Name, Contents = sb.ToString(), ContentType = ContentTypeEnum.TsEnum });
            }

            var nullableSymbol = string.Empty;
            if (Nullable.GetUnderlyingType(propertyType) != null)
                nullableSymbol = "?";

            return $"ko.observable<{propertyType.Name}{nullableSymbol}>();";
        }

        /// <summary>
        /// Class to hold generated TS data.
        /// </summary>
        private class TsData
        {
            public string Namespace { get; set; }

            public string Name { get; set; }

            public string Contents { get; set; }

            public ContentTypeEnum ContentType { get; set; }
        }

        /// <summary>
        /// Enum to denote type of generated TS content.
        /// </summary>
        private enum ContentTypeEnum
        {
            TsClass = 1,

            TsEnum = 2
        }
    }
}
