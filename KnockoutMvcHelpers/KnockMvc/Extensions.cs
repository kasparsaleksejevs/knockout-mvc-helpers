using KnockMvc.TableHelper;
using System.Linq;

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

        public static ColumnPropertyBuilder<TModel, int> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, int> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<int>();
            tableColumn.FooterExpression = (m) => m.Sum(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, int?> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, int?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<int?>();
            tableColumn.FooterExpression = (m) => m.Sum(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, double> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double>();
            tableColumn.FooterExpression = (m) => m.Sum(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double?> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, double?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double?>();
            tableColumn.FooterExpression = (m) => m.Sum(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, decimal> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal>();
            tableColumn.FooterExpression = (m) => m.Sum(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal?> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, decimal?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal?>();
            tableColumn.FooterExpression = (m) => m.Sum(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, int> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, int> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<int>();
            tableColumn.FooterExpression = (m) => m.Average(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, int?> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, int?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<int?>();
            tableColumn.FooterExpression = (m) => m.Average(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, double> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double>();
            tableColumn.FooterExpression = (m) => m.Average(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double?> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, double?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double?>();
            tableColumn.FooterExpression = (m) => m.Average(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, decimal> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal>();
            tableColumn.FooterExpression = (m) => m.Average(expression);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal?> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, decimal?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal?>();
            tableColumn.FooterExpression = (m) => m.Average(expression);
            return builder;
        }
    }
}
