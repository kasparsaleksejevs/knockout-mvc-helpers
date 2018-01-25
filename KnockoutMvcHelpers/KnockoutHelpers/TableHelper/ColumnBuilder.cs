using System;
using System.Linq.Expressions;

namespace KnockoutHelpers.TableHelper
{
    public class ColumnBuilder<TModel> where TModel : class
    {
        private TableBuilder<TModel> tableBuilder;

        private ITableColumn<TModel> column;

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnBuilder{TModel}"/> class.
        /// </summary>
        /// <param name="tableBuilder">The table builder.</param>
        public ColumnBuilder(TableBuilder<TModel> tableBuilder)
        {
            this.tableBuilder = tableBuilder;
        }

        public ColumnBuilder<TModel> Bind<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            this.column = new TableColumn<TModel, TProperty>(expression);
            this.tableBuilder.AddColumn<TProperty>(this.column);

            return this;
        }

        public ColumnBuilder<TModel> Footer(FooterEnum footer)
        {
            this.column.FooterData = footer;
            return this;
        }
    }
}