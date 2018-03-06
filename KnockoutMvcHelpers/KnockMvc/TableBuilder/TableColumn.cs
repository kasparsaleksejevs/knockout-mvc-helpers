using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace KnockMvc.TableHelper
{
    /// <summary>
    /// Class to contain column related data, like the name, type, format, etc.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TProperty">The type of the property.</typeparam>
    /// <seealso cref="KnockMvc.TableHelper.ITableColumn{TModel}" />
    public class TableColumn<TModel, TProperty> : ITableColumn<TModel> where TModel : class
    {
        /// <summary>
        /// The model rows.
        /// </summary>
        private ICollection<TModel> model;

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

        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        /// <value>
        /// The type of the column.
        /// </value>
        public Type ColumnType { get; set; }

        /// <summary>
        /// Compiled lambda expression to get the property value from a model object.
        /// </summary>
        public Func<TModel, TProperty> ValueExpression { get; set; }

        /// <summary>
        /// Gets or sets the footer expression.
        /// </summary>
        /// <value>
        /// The footer expression.
        /// </value>
        public Func<ICollection<TModel>, object> FooterExpression { get; set; }

        /// <summary>
        /// Gets or sets the format to apply to the cell value (e.g., ToString("format")).
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the header CSS class.
        /// </summary>
        /// <value>
        /// The header CSS class.
        /// </value>
        public string HeaderCssClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the table cell.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        public string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the footer CSS class.
        /// If footer CSS class is not specified, the CssClass property value is applied.
        /// </summary>
        /// <value>
        /// The footer CSS class.
        /// </value>
        public string FooterCssClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is a 'header' - the <c>th</c> tag will be applied to the table cell instead of regular <c>td</c> tag.
        /// </summary>
        /// <value>
        ///   <c>True</c> if this column is to be displayed as header; otherwise, <c>false</c>.
        /// </value>
        public bool IsHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is a 'spacer' column.
        /// This can be useful mainly in horizontal flow columns, where this can create table 'split'.
        /// </summary>
        /// <value>
        ///   <c>True</c> if this column is a spacer; otherwise, <c>false</c>.
        /// </value>
        public bool IsSpacer { get; set; }

        /// <summary>
        /// Gets or sets the boolean true value representation as string (e.g., "Yes", "Ok", etc).
        /// Used only for boolean type columns.
        /// </summary>
        /// <value>
        /// The boolean true value.
        /// </value>
        public string BooleanTrueValue { get; set; } = true.ToString();

        /// <summary>
        /// Gets or sets the boolean false value representation as string (e.g., "No", "Declined", etc).
        /// Used only for boolean type columns.
        /// </summary>
        /// <value>
        /// The boolean false value.
        /// </value>
        public string BooleanFalseValue { get; set; } = false.ToString();

        /// <summary>
        /// Gets or sets the template to apply to the formatted cell value.
        /// By default the string '{value}' defines where the formatted cell value will be put in the resulting string.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        /// <example>col.Bind(m => m.DecimalValue).Format("N2").Template("{value}x"); - will display value like '123.34x'.</example>
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets the template specifier, which will be later replaced with the actual cell value.
        /// This should be changed only for edge cases where the resulting text must contain actual string '{value}'.
        /// </summary>
        /// <value>
        /// The template specifier (default is '{value}').
        /// </value>
        public string TemplateSpecifier { get; set; } = "{value}";

        /// <summary>
        /// Gets or sets the HTML attributes to apply to the column.
        /// </summary>
        /// <value>
        /// The HTML attributes.
        /// </value>
        public IList<AttributeData<TModel>> Attributes { get; set; } = new List<AttributeData<TModel>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumn{TModel, TProperty}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public TableColumn(Expression<Func<TModel, TProperty>> expression, ICollection<TModel> model)
        {
            this.model = model;

            if (expression.Body is MemberExpression memberExpression)
            {
                if (memberExpression.Member is PropertyInfo property)
                {
                    if (string.IsNullOrEmpty(this.ColumnTitle))
                    {
                        var displayAttr = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
                        this.ColumnTitle = displayAttr != null ? displayAttr.Name : property.Name;
                    }

                    var formatAttr = property.GetCustomAttribute(typeof(DisplayFormatAttribute)) as DisplayFormatAttribute;
                    this.Format = formatAttr?.DataFormatString;

                    this.ColumnName = property.Name;
                    this.ColumnType = property.PropertyType;
                }
                else
                {
                    if (memberExpression.Member is FieldInfo field)
                    {
                        this.ColumnName = string.Empty;
                        this.ColumnType = field.FieldType;
                    }
                    else
                        this.ColumnType = typeof(string);
                }
            }
            else
            {
                this.ColumnType = expression.ReturnType;
            }

            this.ValueExpression = expression.Compile();
        }

        /// <summary>    
        /// Get the property value from a model object.    
        /// </summary>    
        /// <param name="model">Model to get the property value from.</param>    
        /// <returns>Property value from the model.</returns>    
        public string Evaluate(TModel model)
        {
            var value = this.ValueExpression(model);
            if (value == null)
                return null;

            return this.EvaluateInternal(value, this.Format);
        }

        public string EvaluateFooter(ICollection<TModel> model)
        {
            var property = this.FooterExpression(this.model);
            if (property == null)
                return null;

            string format = null;
            var columnType = Nullable.GetUnderlyingType(property.GetType());
            if (columnType == null)
                columnType = property.GetType();

            var originalColumnType = Nullable.GetUnderlyingType(this.ColumnType);
            if (originalColumnType == null)
                originalColumnType = this.ColumnType;

            if (columnType == originalColumnType)
                format = this.Format;

            return this.EvaluateInternal(property, format);
        }

        public Func<TModel, TExpression> GetValueExpression<TExpression>()
        {
            return this.ValueExpression as Func<TModel, TExpression>;
        }

        private string EvaluateInternal(object property, string format)
        {
            var columnType = property.GetType();

            var result = string.Empty;
            if (columnType == typeof(int) || columnType == typeof(int?))
                result = (property as int?)?.ToString(format);
            else if (columnType == typeof(decimal) || columnType == typeof(decimal?))
                result = (property as decimal?)?.ToString(format);
            else if (columnType == typeof(double) || columnType == typeof(double?))
                result = (property as double?)?.ToString(format);
            else if (columnType == typeof(DateTime) || columnType == typeof(DateTime?))
                result = (property as DateTime?)?.ToString(format);
            else if (columnType == typeof(bool) || columnType == typeof(bool?))
                result = (property as bool?) == true ? this.BooleanTrueValue : this.BooleanFalseValue;
            else if (columnType.IsEnum)
            {
                if (columnType.GetField(property.ToString()).GetCustomAttribute(typeof(DisplayAttribute)) is DisplayAttribute displayAttr)
                    result = displayAttr.Name;
                else
                {
                    // for legacy MVC3+ projects the DescriptionAttribute is usually used
                    if (columnType.GetField(property.ToString()).GetCustomAttribute(typeof(DescriptionAttribute)) is DescriptionAttribute descriptionAttr)
                        result = descriptionAttr.Description;
                    else
                        result = property.ToString();
                }
            }
            else
                result = property.ToString();

            return result;
        }
    }
}