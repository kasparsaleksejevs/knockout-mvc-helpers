using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace KnockMvc.TypeScriptGenerator
{
    public class TypeScriptClassGenerator
    {
        /// <summary>
        /// The generated data - namespace, contents and content type.
        /// </summary>
        private readonly IList<TypeScriptData> generatedData = new List<TypeScriptData>();

        /// <summary>
        /// The types for which to generate TypeScript.
        /// </summary>
        private readonly IList<Type> types = new List<Type>();

        public string Generate(Type type)
        {
            this.types.Add(type);

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
        }

        public string Generate<TGenerateAttribute>(Assembly sourceAssembly) where TGenerateAttribute : Attribute
        {
            foreach (var type in sourceAssembly.GetExportedTypes())
            {
                if (type.GetCustomAttribute<TGenerateAttribute>(false) == null)
                    continue;

                this.types.Add(type);
            }

            foreach (var item in this.types)
                this.GenerateTsClass(item, typeof(TGenerateAttribute));

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
        }

        private string GenerateClassName(Type classToGenerate)
        {
            if (!classToGenerate.IsGenericType)
                return classToGenerate.Name;

            var className = classToGenerate.Name.Substring(0, classToGenerate.Name.IndexOf('`'));
            var types = new List<string>();
            foreach (var item in classToGenerate.GetGenericArguments())
                types.Add(item.Name);

            className += $"<{string.Join(", ", types)}>";

            return className;
        }

        /// <summary>
        /// Generates the ts class.
        /// </summary>
        /// <param name="classToGenerate">The item.</param>
        private void GenerateTsClass(Type classToGenerate, Type typeOfAttribute = null)
        {
            if (this.generatedData.Any(m => m.Namespace == classToGenerate.Namespace && m.Name == classToGenerate.Name))
                return;

            var genericClass = classToGenerate;
            if (classToGenerate.IsConstructedGenericType)
                genericClass = classToGenerate.GetGenericTypeDefinition();

            var className = this.GenerateClassName(genericClass);

            // https://stackoverflow.com/questions/17480990/get-name-of-generic-class-without-tilde

            var sbClass = new StringBuilder();
            var sbInterface = new StringBuilder();
            sbClass.AppendLine();
            sbClass.AppendLine($"    export class {className} {{");
            this.GenerateConstAndReadonlyProperties(genericClass, sbClass);

            sbInterface.AppendLine();
            sbInterface.AppendLine($"    export interface I{className} {{");

            foreach (var prop in genericClass.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                sbClass.AppendLine($"        {prop.Name} = {this.GetKnockoutPropertyDefinition(prop.PropertyType, typeOfAttribute)}");
                sbInterface.AppendLine($"        {prop.Name}{this.GetPropertyInterfaceDefinition(prop.PropertyType, typeOfAttribute)}");
            }

            // add constructor
            sbClass.AppendLine();
            sbClass.AppendLine($"        constructor(p?: I{className}) {{");
            sbClass.AppendLine("            if (p) {");
            foreach (var prop in genericClass.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (this.IsPropertyArray(prop.PropertyType))
                {
                    Type arrayElementType;
                    if (prop.PropertyType.IsGenericType)
                        arrayElementType = prop.PropertyType.GetGenericArguments()[0];
                    else
                        arrayElementType = prop.PropertyType.GetElementType();

                    var arrayElementTypeName = this.GeneratePropertyType(arrayElementType, typeOfAttribute);

                    if (this.ShouldGenerateSubclass(arrayElementType, typeOfAttribute))
                    {
                        sbClass.AppendLine($"                this.{prop.Name}([]);");
                        sbClass.AppendLine($"                if (p.{prop.Name}) {{");
                        sbClass.AppendLine($"                    p.{prop.Name}.forEach((item) => {{");
                        sbClass.AppendLine($"                        this.{prop.Name}.push(new {arrayElementTypeName}(item));");
                        sbClass.AppendLine($"                    }});");
                        sbClass.AppendLine($"                }}");
                    }
                    else
                        sbClass.AppendLine($"                this.{prop.Name}.push(...p.{prop.Name});");
                }
                else
                {
                    if (this.ShouldGenerateSubclass(prop.PropertyType, typeOfAttribute))
                        sbClass.AppendLine($"                this.{prop.Name}(new {prop.PropertyType.Name}(p.{prop.Name}));");
                    else if (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?))
                        sbClass.AppendLine($"                this.{prop.Name}(new Date(p.{prop.Name}));");
                    else
                        sbClass.AppendLine($"                this.{prop.Name}(p.{prop.Name});");
                }
            }

            sbClass.AppendLine("            }");
            sbClass.AppendLine("        }");
            sbClass.AppendLine("    }");
            sbInterface.AppendLine("    }");

            sbClass.Append(sbInterface);

            this.generatedData.Add(new TypeScriptData { Namespace = genericClass.Namespace, Name = className, Contents = sbClass.ToString(), ContentType = TypeScriptDataContentTypeEnum.TsClass });
        }

        private void GenerateConstAndReadonlyProperties(Type classToGenerate, StringBuilder sbClass)
        {
            foreach (var prop in classToGenerate.GetFields(BindingFlags.Public | BindingFlags.Static)
                            .Where(m => m.IsLiteral && !m.IsInitOnly))
            {
                var value = prop.GetValue(null);
                var valueForJs = this.GetPropertyValueAsJS(value);

                sbClass.AppendLine($"        public static readonly {prop.Name} = {valueForJs};");
            }

            foreach (var prop in classToGenerate.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.IsInitOnly))
            {
                var value = prop.GetValue(null);
                var valueForJs = this.GetPropertyValueAsJS(value);

                sbClass.AppendLine($"        public static readonly {prop.Name} = {valueForJs};");
            }

            foreach (var prop in classToGenerate.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.IsInitOnly))
            {
                object lateBound = Activator.CreateInstance(classToGenerate);

                var value = prop.GetValue(lateBound);
                var valueForJs = this.GetPropertyValueAsJS(value);

                sbClass.AppendLine($"        public readonly {prop.Name} = {valueForJs};");
            }
        }

        /// <summary>
        /// Determines if it is needed to generate class for the property type.
        /// If we do not specify the TS Generation attribute, then all custom types should be generated (except System types).
        /// Otherwise - we check if it has TS Generation attribute and is not an enum.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="typeOfAttribute">The type of attribute.</param>
        /// <returns></returns>
        private bool ShouldGenerateSubclass(Type propertyType, Type typeOfAttribute)
        {
            // do not generate classes for enums, as they should be handled as enums!
            if (propertyType.IsEnum)
                return false;

            if (typeOfAttribute == null)
            {
                // we are generating TS for specific types, without taking into account TSGen attributes, so check if it is 'system' type
                return !propertyType?.FullName?.StartsWith("System.") ?? false;
            }

            return propertyType.GetCustomAttribute(typeOfAttribute, false) != null;
        }

        private string GeneratePropertyType(Type propertyType, Type typeOfAttribute)
        {
            if (!propertyType.IsGenericType)
                return propertyType.Name;

            var propertyName = propertyType.Name.Substring(0, propertyType.Name.IndexOf('`'));
            var types = new List<string>();
            foreach (var item in propertyType.GetGenericArguments())
            {
                types.Add(this.GetPropertyDefinition(item, typeOfAttribute));
            }

            propertyName += $"<{string.Join(", ", types)}>";

            return propertyName;
        }

        private string GetKnockoutPropertyDefinition(Type propertyType, Type typeOfAttribute)
        {
            var tsPropertyType = this.GetPropertyDefinition(propertyType, typeOfAttribute);
            var isArray = this.IsPropertyArray(propertyType);
            var koType = isArray ? "observableArray" : "observable";
            return $"ko.{koType}<{tsPropertyType}>();";
        }

        /// <summary>
        /// Gets the property definition in the form 'ko.observable&lt;number&gt;>();'.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>Property definition</returns>
        private string GetPropertyDefinition(Type propertyType, Type typeOfAttribute)
        {
            // ToDo: default values?
            // ToDo: cycle checking for custom classes

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
            if (this.ShouldGenerateSubclass(propertyType, typeOfAttribute))
            {
                this.GenerateTsClass(propertyType, typeOfAttribute); // ToDo: generate correct namespace for the type
                tsPropertyType = this.GeneratePropertyType(propertyType, typeOfAttribute);
            }
            else
            {
                // is it an Enum?
                if (propertyType.IsEnum)
                    return this.GenerateEnum(propertyType); // ToDo: generate correct namespace for the type

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

                if (propertyType.FullName == null) // it is a generic type
                    tsPropertyType = propertyType.Name;
            }

            return tsPropertyType;
        }

        /// <summary>
        /// Gets the property definition in the form 'ko.observable&lt;number&gt;>();'.
        /// </summary>
        /// <param name="property">The property info.</param>
        /// <returns>Property definition</returns>
        private string GetPropertyInterfaceDefinition(Type propertyType, Type typeOfAttribute)
        {
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
            if (this.ShouldGenerateSubclass(propertyType, typeOfAttribute))
                tsPropertyType = "I" + GeneratePropertyType(propertyType, null);
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

                if (propertyType.FullName == null) // it is a generic type
                    tsPropertyType = propertyType.Name;
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
                sb.AppendLine($"    export enum {propertyType.Name} {{");
                foreach (var item in fields)
                    sb.AppendLine($"        {item.Name} = {item.GetRawConstantValue()},");

                sb.AppendLine("    }");

                sb.AppendLine();
                sb.AppendLine($"    export const {propertyType.Name}Text = new Map<number, string>([");
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

        /// <summary>
        /// Gets the property value as JS object (quoted, initialized).
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>Value expressed as JS code object.</returns>
        private string GetPropertyValueAsJS(object value)
        {
            var result = "#error";

            if (value is string)
                result = $"'{value}'";
            else if (value == null)
                result = "null";
            else if (value is int intValue)
                result = intValue.ToString(CultureInfo.InvariantCulture);
            else if (value is decimal decimalValue)
                result = decimalValue.ToString(CultureInfo.InvariantCulture);
            else if (value is float floatValue)
                result = floatValue.ToString(CultureInfo.InvariantCulture);
            else if (value is double doubleValue)
                result = doubleValue.ToString(CultureInfo.InvariantCulture);
            else if (value is long longValue)
                result = longValue.ToString(CultureInfo.InvariantCulture);
            else if (value is DateTime dateValue)
                result = $"new Date('{dateValue.ToString("s")}')";
            else if (value is bool boolValue)
                result = boolValue ? "true" : "false";
            else if (this.IsPropertyArray(value.GetType()))
            {
                var lstArrValues = new List<string>();

                var array = value as IEnumerable;
                foreach (var item in array)
                    lstArrValues.Add(this.GetPropertyValueAsJS(item));

                result = "[" + string.Join(", ", lstArrValues) + "]";
            }

            return result;
        }
    }
}
