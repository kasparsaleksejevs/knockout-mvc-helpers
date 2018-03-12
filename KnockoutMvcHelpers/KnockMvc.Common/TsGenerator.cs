using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KnockMvc.Common
{
    public class TsGenerator
    {
        /// <summary>
        /// The types for which to generate TypeScript.
        /// </summary>
        private IList<Type> types = null;

        /// <summary>
        /// The generated data - namespace, contents and content type.
        /// </summary>
        private IList<TsData> generatedData = new List<TsData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TsGenerator"/> class.
        /// </summary>
        /// <param name="types">The types for which to generate corresponding TypeScript classes.</param>
        public TsGenerator(IList<Type> types)
        {
            this.types = types;
        }

        /// <summary>
        /// Generates the TypeScript code.
        /// </summary>
        /// <returns>Currently for testing - full generator output.</returns>
        public string Generate()
        {
            foreach (var item in this.types)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"    class {item.Name} {{");

                foreach (var member in item.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                {
                    var prop = member as PropertyInfo;
                    if (prop == null)
                        continue;

                    // ~"ko.observable<string>();"
                    var jsPropertyType = string.Empty;
                    jsPropertyType = this.GetPropertyDefinition(prop);

                    sb.AppendLine($"        {prop.Name} = {jsPropertyType}");
                }

                sb.AppendLine("    }");

                this.generatedData.Add(new TsData { Namespace = item.Namespace, Name = item.Name, Contents = sb.ToString(), ContentType = ContentTypeEnum.TsClass });
            }

            // ============== // ============== // ============== // ============== 
            var tempResult = string.Empty;
            foreach (var item in this.generatedData.GroupBy(g => g.Namespace))
            {
                tempResult += "module " + item.Key + " {" + Environment.NewLine;
                foreach (var dataItem in item.OrderBy(o => o.ContentType))
                {
                    tempResult += dataItem.Contents + Environment.NewLine;
                }

                tempResult += "}" + Environment.NewLine;
            }

            return tempResult;
            // ============== // ============== // ============== // ============== 
        }

        /// <summary>
        /// Generates the TypeScript for enum property.
        /// </summary>
        /// <param name="propertyType">Type of the property (enum type).</param>
        /// <returns>Generated property</returns>
        private string GenerateEnum(Type propertyType)
        {
            var sb = new StringBuilder();

            var underlyingType = Enum.GetUnderlyingType(propertyType);
            var enumValues = Enum.GetValues(propertyType);

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
            // exmaple: 'var textVal = MyEnumText.get(MyEnum.Val2); // will store "Other (Descr)"'

            this.generatedData.Add(new TsData { Namespace = propertyType.Namespace, Name = propertyType.Name, Contents = sb.ToString(), ContentType = ContentTypeEnum.TsEnum });

            var nullableSymbol = string.Empty;
            if (Nullable.GetUnderlyingType(propertyType) != null)
                nullableSymbol = "?";

            return $"ko.observable<{propertyType.Name}{nullableSymbol}>();";
        }

        /// <summary>
        /// Gets the property definition in the form 'ko.observable&lt;number&gt;>();'.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>Property definition</returns>
        private string GetPropertyDefinition(PropertyInfo property)
        {
            if (property.PropertyType.IsEnum)
                return this.GenerateEnum(property.PropertyType);

            // ToDo: arrays
            // ToDo: custom classes
            // ToDo: default values?

            var propertyType = "string";
            var nullableSymbol = string.Empty;
            if (Nullable.GetUnderlyingType(property.PropertyType) != null)
                nullableSymbol = "?";

            if (property.PropertyType == typeof(int) || property.PropertyType == typeof(int?)
                    || property.PropertyType == typeof(decimal) || property.PropertyType == typeof(decimal?)
                    || property.PropertyType == typeof(double) || property.PropertyType == typeof(double?)
                    || property.PropertyType == typeof(float) || property.PropertyType == typeof(float?)
                    )
                propertyType = "number";

            if (property.PropertyType == typeof(string))
                propertyType = "string";

            if (property.PropertyType == typeof(bool) || property.PropertyType == typeof(bool?))
                propertyType = "boolean";

            if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                propertyType = "Date";

            return $"ko.observable<{propertyType}{nullableSymbol}>();";
        }

        private class TsData
        {
            public string Namespace { get; set; }

            public string Name { get; set; }

            public string Contents { get; set; }

            public ContentTypeEnum ContentType { get; set; }
        }

        private enum ContentTypeEnum
        {
            TsClass = 1,
            TsEnum = 2
        }
    }
}