using KnockMvc.TableHelper;

namespace KnockMvc
{
    public static class Extensions
    {
        public static ColumnPropertyBuilder<TModel, bool?> TrueFalse<TModel>(this ColumnPropertyBuilder<TModel, bool?> builder, string trueValue, string falseValue) where TModel : class
        {
            var tableColumn = builder.Column;
            tableColumn.BooleanTrueValue = trueValue;
            tableColumn.BooleanFalseValue = falseValue;

            return builder;
        }

        public static ColumnPropertyBuilder<TModel, bool> TrueFalse<TModel>(this ColumnPropertyBuilder<TModel, bool> builder, string trueValue, string falseValue) where TModel : class
        {
            var tableColumn = builder.Column;
            tableColumn.BooleanTrueValue = trueValue;
            tableColumn.BooleanFalseValue = falseValue;
            return builder;
        }
    }
}
