namespace KnockMvc.Web.Helpers.TableHelper
{
    public class ColumnPropertyBuilder<TModel, TProperty> where TModel : class
    {
        private ColumnBuilder<TModel> columnBuilder;

        private ITableColumn<TModel> column;

        public ColumnPropertyBuilder(ColumnBuilder<TModel> columnBuilder, ITableColumn<TModel> column)
        {
            this.columnBuilder = columnBuilder;
            this.column = column;
        }

        public ColumnPropertyBuilder<TModel, TProperty> Footer(FooterEnum footer)
        {
            this.column.FooterData = footer;
            return this;
        }

        public ColumnPropertyBuilder<TModel, TProperty> Format(string formatString)
        {
            this.column.Format = formatString;
            return this;
        }

        /// <summary>
        /// Applies specifiec CSS class to the thead > tr > th.
        /// </summary>
        /// <param name="cssClasses">The CSS classes to apply.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> HeaderClass(string cssClasses)
        {
            this.column.HeaderCssClass = cssClasses;
            return this;
        }

        /// <summary>
        /// Applies specifiec CSS class to the tbody > tr > td.
        /// </summary>
        /// <param name="cssClasses">The CSS classes to apply.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> CssClass(string cssClasses)
        {
            this.column.CssClass = cssClasses;
            return this;
        }

        /// <summary>
        /// The <c>th</c> tag will be used instead of the regular <c>td</c> tag for the current column.
        /// </summary>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> IsHeader()
        {
            this.column.IsHeader = true;
            return this;
        }

        /// <summary>
        /// The column header will be without title even if data model property has Display/Description specified.
        /// </summary>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> NoTitle()
        {
            this.column.NoTitle = true;
            return this;
        }
    }
}