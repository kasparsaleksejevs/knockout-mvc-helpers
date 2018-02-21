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
    }
}