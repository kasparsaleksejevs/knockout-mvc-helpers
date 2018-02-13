using KnockMvc.TableHelper;
using System.Collections.Generic;
using System.Web.Mvc;

namespace KnockMvc
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Creates a table builder instance and draws the generated table to the output.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="html">The HTML (Razor helper).</param>
        /// <param name="model">The model of the data.</param>
        /// <returns>Table builder instance for method chaining.</returns>
        public static TableBuilder<TModel> TableFor<TModel>(this HtmlHelper html, ICollection<TModel> model) where TModel : class
        {
            return new TableBuilder<TModel>(html, model);
        }
    }
}
