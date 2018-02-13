using KnockMvc.TableHelper;
using System.Linq;

namespace KnockMvc
{
    public static class Extensions
    {
        /// <summary>
        /// Sets the True/False textual values to display for <seealso cref="bool"/> columns.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="builder">The column property builder.</param>
        /// <param name="trueValue">The text to display when the table cells value is <c>true</c>.</param>
        /// <param name="falseValue">The text to display when the table cells value is <c>false</c>.</param>
        /// <returns>Column property builder instance.</returns>
        public static ColumnPropertyBuilder<TModel, bool?> TrueFalse<TModel>(this ColumnPropertyBuilder<TModel, bool?> builder, string trueValue, string falseValue) where TModel : class
        {
            var tableColumn = builder.Column;
            tableColumn.BooleanTrueValue = trueValue;
            tableColumn.BooleanFalseValue = falseValue;

            return builder;
        }

        /// <summary>
        /// Sets the True/False textual values to display for <seealso cref="bool"/> columns.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <param name="builder">The column property builder.</param>
        /// <param name="trueValue">The text to display when the table cells value is <c>true</c>.</param>
        /// <param name="falseValue">The text to display when the table cells value is <c>false</c>.</param>
        /// <returns>Column property builder instance.</returns>
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
            tableColumn.FooterExpression = (m) => m.Any() ? m.Sum(expression) : default(int);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, int?> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, int?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<int?>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Sum(expression) : default(int?);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, double> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Sum(expression) : default(double);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double?> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, double?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double?>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Sum(expression) : default(double?);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, decimal> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Sum(expression) : default(decimal);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal?> FooterSum<TModel>(this ColumnPropertyBuilder<TModel, decimal?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal?>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Sum(expression) : default(decimal?);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, int> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, int> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<int>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Average(expression) : default(int);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, int?> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, int?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<int?>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Average(expression) : default(int?);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, double> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Average(expression) : default(double);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, double?> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, double?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<double?>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Average(expression) : default(double?);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, decimal> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Average(expression) : default(decimal);
            return builder;
        }

        public static ColumnPropertyBuilder<TModel, decimal?> FooterAvg<TModel>(this ColumnPropertyBuilder<TModel, decimal?> builder) where TModel : class
        {
            var tableColumn = builder.Column;
            var expression = tableColumn.GetValueExpression<decimal?>();
            tableColumn.FooterExpression = (m) => m.Any() ? m.Average(expression) : default(decimal?);
            return builder;
        }
    }
}
