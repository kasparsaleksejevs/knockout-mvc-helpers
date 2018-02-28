using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace KnockMvc.TableHelper
{
    public class TableColumn<TModel, TProperty> : ITableColumn<TModel> where TModel : class
    {
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

        public Type ColumnType { get; set; }

        /// <summary>
        /// Compiled lambda expression to get the property value from a model object.
        /// </summary>
        public Func<TModel, TProperty> ValueExpression { get; set; }

        public Func<ICollection<TModel>, object> FooterExpression { get; set; }

        public string Format { get; set; }

        public string HeaderCssClass { get; set; }

        public string CssClass { get; set; }

        public string FooterCssClass { get; set; }

        public bool IsHeader { get; set; }

        public bool IsSpacer { get; set; }

        public string BooleanTrueValue { get; set; } = true.ToString();

        public string BooleanFalseValue { get; set; } = false.ToString();

        public string Template { get; set; }

        public string TemplateSpecifier { get; set; } = "{value}";

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

        public Func<TModel, TExpression> GetValueExpression<TExpression>()
        {
            var x = this.ValueExpression as Func<TModel, TExpression>;
            return x;
        }
    }
}