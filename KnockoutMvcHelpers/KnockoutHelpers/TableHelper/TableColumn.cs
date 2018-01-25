using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace KnockoutHelpers.TableHelper
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
        /// If we use knockout binding, this is not needed (as it requires execution on top of actual data model). 
        /// </summary>
        public Func<TModel, TProperty> CompiledExpression { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TableColumn{TModel, TProperty}"/> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public TableColumn(Expression<Func<TModel, TProperty>> expression)
        {
            var property = (expression.Body as MemberExpression).Member as PropertyInfo;
            var displayAttr = property.GetCustomAttribute(typeof(DisplayAttribute)) as DisplayAttribute;
            this.ColumnTitle = displayAttr != null ? displayAttr.Name : property.Name;

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
            var result = this.CompiledExpression(model);
            return result == null ? string.Empty : result.ToString();
        }
    }
}