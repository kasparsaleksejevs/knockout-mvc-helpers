using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace KnockMvc.Web.Helpers.TableHelper
{
    public class TableColumn<TModel, TProperty> : ITableColumn<TModel> where TModel : class
    {
        /// <summary>
        /// Gets or sets the column title to display in the table.
        /// </summary>
        /// <value>
        /// The column title.
        /// </value>
        public string ColumnTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        public string ColumnName { get; set; }

        public Type ColumnType { get; set; }

        public FooterEnum FooterData { get; set; }

        /// <summary>
        /// Compiled lambda expression to get the property value from a model object.
        /// </summary>
        public Func<TModel, TProperty> CompiledExpression { get; set; }

        public string Format { get; set; }

        public string HeaderCssClass { get; set; }

        public string CssClass { get; set; }

        public bool IsHeader { get; set; }

        public bool NoTitle { get; set; }

        public string BooleanTrueValue { get; set; } = true.ToString();

        public string BooleanFalseValue { get; set; } = false.ToString();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumn{TModel, TProperty}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public TableColumn(Expression<Func<TModel, TProperty>> expression)
        {
            var property = (expression.Body as MemberExpression).Member as PropertyInfo;
            var displayAttr = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            this.ColumnTitle = displayAttr != null ? displayAttr.Name : property.Name;

            var formatAttr = property.GetCustomAttribute(typeof(DisplayFormatAttribute)) as DisplayFormatAttribute;
            this.Format = formatAttr?.DataFormatString;

            this.ColumnName = property.Name;
            this.ColumnType = property.PropertyType;

            this.CompiledExpression = expression.Compile();
        }

        /// <summary>    
        /// Get the property value from a model object.    
        /// </summary>    
        /// <param name="model">Model to get the property value from.</param>    
        /// <returns>Property value from the model.</returns>    
        public string Evaluate(TModel model)
        {
            var value = this.CompiledExpression(model);
            if (value == null)
                return string.Empty;

            var result = string.Empty;
            if (this.ColumnType == typeof(int) || this.ColumnType == typeof(int?))
                result = (value as int?)?.ToString(this.Format);
            else if (this.ColumnType == typeof(decimal) || this.ColumnType == typeof(decimal?))
                result = (value as decimal?)?.ToString(this.Format);
            else if (this.ColumnType == typeof(double) || this.ColumnType == typeof(double?))
                result = (value as double?)?.ToString(this.Format);
            else if (this.ColumnType == typeof(DateTime) || this.ColumnType == typeof(DateTime?))
                result = (value as DateTime?)?.ToString(this.Format);
            else if (this.ColumnType == typeof(bool) || this.ColumnType == typeof(bool?))
                result = (value as bool?) == true ? this.BooleanTrueValue : this.BooleanFalseValue;
            else if (this.ColumnType.IsEnum)
            {
                var displayAttr = this.ColumnType.GetField(value.ToString()).GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                if (displayAttr != null)
                    result = displayAttr.Name;
                else
                {
                    // for legacy MVC3+ projects the DescriptionAttribute is usually used
                    var descriptionAttr = this.ColumnType.GetField(value.ToString()).GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute;
                    if (descriptionAttr != null)
                        result = descriptionAttr.Description;
                    else
                        result = value.ToString();
                }
            }
            else
                result = value.ToString();

            return result;
        }
    }
}