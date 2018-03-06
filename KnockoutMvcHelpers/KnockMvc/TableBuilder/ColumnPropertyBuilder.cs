using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace KnockMvc.TableHelper
{
    public class ColumnPropertyBuilder<TModel, TProperty> where TModel : class
    {
        private ColumnBuilder<TModel> columnBuilder;

        internal ITableColumn<TModel> Column { get; set; }

        public ColumnPropertyBuilder(ColumnBuilder<TModel> columnBuilder, ITableColumn<TModel> column)
        {
            this.columnBuilder = columnBuilder;
            this.Column = column;
        }

        public ColumnPropertyBuilder<TModel, TProperty> Footer(Expression<Func<ICollection<TModel>, object>> expression)
        {
            this.Column.FooterExpression = expression.Compile();
            return this;
        }

        public ColumnPropertyBuilder<TModel, TProperty> Format(string formatString)
        {
            this.Column.Format = formatString;
            return this;
        }

        /// <summary>
        /// Applies specific CSS class to the 'thead > tr > th' for this column.
        /// </summary>
        /// <param name="cssClasses">The CSS classes to apply.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> HeaderClass(string cssClasses)
        {
            this.Column.HeaderCssClass = cssClasses;
            return this;
        }

        /// <summary>
        /// Applies specific CSS class to the 'tbody > tr > td' for this column.
        /// </summary>
        /// <param name="cssClasses">The CSS classes to apply.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> CssClass(string cssClasses)
        {
            this.Column.CssClass = cssClasses;
            return this;
        }

        /// <summary>
        /// Applies specific CSS class to the 'tfoot > tr > td' for this column.
        /// </summary>
        /// <param name="cssClasses">The CSS classes.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> FooterCssClass(string cssClasses)
        {
            this.Column.FooterCssClass = cssClasses;
            return this;
        }

        /// <summary>
        /// The <c>th</c> tag will be used instead of the regular <c>td</c> tag for the current column.
        /// </summary>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> IsHeader()
        {
            this.Column.IsHeader = true;
            return this;
        }

        /// <summary>
        /// Applies a custom title to the column.
        /// Therefore any <seealso cref="DisplayNameAttribute"/> or <seealso cref="DescriptionAttribute "/> specified for the column property will be ignored.
        /// </summary>
        /// <param name="customTitle">The custom title.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> Title(string customTitle)
        {
            this.Column.ColumnTitle = customTitle;
            return this;
        }

        /// <summary>
        /// Applies template to use to render the columns value.
        /// By default, the column property value replaces <c>{value}</c> string, but this can be changed using <see cref="TemplateSpecifier(string)" />.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> Template(string template)
        {
            this.Column.Template = template;
            return this;
        }

        /// <summary>
        /// Specifies custom replacement string for the template.
        /// </summary>
        /// <param name="templateSpecifier">The custom template specifier. The default is <c>{value}</c>.</param>
        /// <returns>Column builder instance.</returns>
        public ColumnPropertyBuilder<TModel, TProperty> TemplateSpecifier(string templateSpecifier)
        {
            this.Column.TemplateSpecifier = templateSpecifier;
            return this;
        }

        public ColumnPropertyBuilder<TModel, TProperty> WithAttribute(string attributeName, Expression<Func<TModel, object>> valueExpression)
        {
            this.Column.Attributes.Add(new AttributeData<TModel>
            {
                Name = attributeName,
                Value = valueExpression.Compile()
            });

            return this;
        }
    }
}