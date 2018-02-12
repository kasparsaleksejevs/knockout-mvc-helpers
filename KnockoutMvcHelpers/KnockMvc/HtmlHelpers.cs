using KnockMvc.TableHelper;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KnockMvc
{
    public static class HtmlHelpers
    {
        public static TableBuilder<TModel> TableFor<TModel>(this HtmlHelper html, ICollection<TModel> model) where TModel : class
        {
            return new TableBuilder<TModel>(html, model);
        }
    }
}
