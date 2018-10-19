using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;

namespace KnockMvc.TableHelper
{
    public interface ITableBuilderOptions<TModel> : IHtmlString where TModel : class
    {
        ITableBuilderOptions<TModel> Columns(Action<ColumnBuilder<TModel>> builder);

        ITableBuilderOptions<TModel> FooterText(string footerText);

        ITableBuilderOptions<TModel> Css(string cssClass);

        /// <summary>
        /// Adds the HTML attribute with specified value expression to the table.
        /// This method also accepts and adds the 'class' atribute even if the class property is already specified.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <param name="valueExpression">The value expression.</param>
        /// <returns>Table builder instance.</returns>
        ITableBuilderOptions<TModel> AddAttribute(string attributeName, Expression<Func<ICollection<TModel>, object>> valueExpression);

        /// <summary>
        /// Adds the custom header HTML row string before any other auto-generated header row.
        /// If <c>thead</c> block doesn't exist (e.g. for horizontal-flow table) - it is created. 
        /// Column count and row count is available as replaceable template strings: <c>{columnCount}</c> and <c>{rowCount}</c>.
        /// </summary>
        /// <returns>Table builder instance.</returns>
        /// <example>.AddCustomHeaderRow("<tr><th colspan="3">Spanned title</th><th>Other title</th></tr>")</example>
        ITableBuilderOptions<TModel> AddCustomHeaderRow(string customHeaderRowHtml);

        /// <summary>
        /// Groups the data by specified expression/property and displays groups as additional rows.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expression">The expression to group by.</param>
        /// <param name="cssClass">The optional CSS class to add to group header cell.</param>
        /// <returns>Table builder instance.</returns>
        ITableBuilderOptions<TModel> GroupBy<TProperty>(Expression<Func<TModel, TProperty>> expression, string cssClass = null);
    }
}