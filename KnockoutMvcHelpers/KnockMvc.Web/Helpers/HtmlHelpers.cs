using KnockMvc.Web.Helpers.TableHelper;
using System.Collections.Generic;
using System.Reflection;
using System.Web.Mvc;

namespace KnockMvc.Web.Helpers
{
    public static class HtmlHelpers
    {
        public static TableBuilder<TModel> TableFor<TModel>(this HtmlHelper html, ICollection<TModel> model) where TModel : class
        {
            return new TableBuilder<TModel>(html, model);
        }
    }

    public static class Extensions
    {
        public static ColumnPropertyBuilder<TModel, bool?> TrueFalse<TModel>(this ColumnPropertyBuilder<TModel, bool?> builder, string trueValue, string falseValue) where TModel : class
        {
            var field = builder.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var tableColumn = field?.GetValue(builder) as ITableColumn<TModel>;
            tableColumn.BooleanTrueValue = trueValue;
            tableColumn.BooleanFalseValue = falseValue;

            return builder;
        }

        public static ColumnPropertyBuilder<TModel, bool> TrueFalse<TModel>(this ColumnPropertyBuilder<TModel, bool> builder, string trueValue, string falseValue) where TModel : class
        {
            var field = builder.GetType().GetField("column", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var tableColumn = field?.GetValue(builder) as ITableColumn<TModel>;
            tableColumn.BooleanTrueValue = trueValue;
            tableColumn.BooleanFalseValue = falseValue;
            return builder;
        }
    }
}