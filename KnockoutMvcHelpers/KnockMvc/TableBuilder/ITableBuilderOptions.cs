using System;
using System.Web;

namespace KnockMvc.TableHelper
{
    public interface ITableBuilderOptions<TModel> : IHtmlString where TModel : class
    {
        ITableBuilderOptions<TModel> Columns(Action<ColumnBuilder<TModel>> builder);

        //ITableBuilderOptions<TModel> Attributes(object htmlAttributes);

        ITableBuilderOptions<TModel> FooterText(string footerText);

        ITableBuilderOptions<TModel> Css(string cssClass);

        /// <summary>
        /// Adds the custom header HTML row string before any other auto-generated header row.
        /// If <c>thead</c> block doesn't exist (e.g. for horizontal-flow table) - it is created. 
        /// Column count and row count is available as replaceable template strings: <c>{columnCount}</c> and <c>{rowCount}</c>.
        /// </summary>
        /// <returns>Table builder instance.</returns>
        /// <example>.AddCustomHeaderRow("<tr><th colspan="3">Spanned title</th><th>Other title</th></tr>")</example>
        ITableBuilderOptions<TModel> AddCustomHeaderRow(string customHeaderRowHtml);
    }
}