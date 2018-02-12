using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace KnockMvc.TableHelper
{
    public class TableBuilder<TModel> : ITableBuilderOptions<TModel> where TModel : class
    {
        private List<ITableColumn<TModel>> columns = new List<ITableColumn<TModel>>();

        private HtmlHelper htmlHelper;

        internal ICollection<TModel> Model { get; set; }

        public TableBuilder(HtmlHelper html, ICollection<TModel> model)
        {
            this.htmlHelper = html;
            this.Model = model;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Render();
        }

        /// <summary>
        /// Returns an HTML-encoded string.
        /// </summary>
        /// <returns>
        /// An HTML-encoded string.
        /// </returns>
        public string ToHtmlString()
        {
            return this.ToString();
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        /// <returns></returns>
        private string Render()
        {
            var colCount = this.columns.Count;

            var headRow = string.Empty;
            foreach (var column in this.columns)
            {
                var cssClass = column.HeaderCssClass != null ? $" class=\"{column.HeaderCssClass}\"" : string.Empty;
                headRow += $"<th{cssClass}>{(string.IsNullOrEmpty(column.ColumnTitle) ? "&nbsp;" : column.ColumnTitle)}</th>";
            }

            var bodyRows = string.Empty;
            foreach (var row in this.Model)
            {
                var bodyRow = string.Empty;
                foreach (var column in this.columns)
                {
                    var cssClass = column.CssClass != null ? $" class=\"{column.CssClass}\"" : string.Empty;

                    var cellValue = column.Evaluate(row);
                    if (!string.IsNullOrEmpty(column.Template))
                        cellValue = column.Template.Replace("{value}", cellValue);

                    if (column.IsHeader)
                        bodyRow += $"<th{cssClass}>{cellValue}</th>";
                    else
                        bodyRow += $"<td{cssClass}>{cellValue}</td>";
                }

                bodyRows += $"<tr>{bodyRow}</tr>";
            }

            var footer = string.Empty;
            var hasFooter = this.columns.Any(m => m.FooterExpression != null);
            if (hasFooter)
            {
                var canSpan = true;
                var spanValue = 0;
                for (int i = 0; i < colCount; i++)
                {
                    if (this.columns[i].FooterExpression != null)
                    {
                        if (canSpan)
                        {
                            footer += $"<td colspan=\"{spanValue}\">Total:</td>";
                            canSpan = false;
                        }

                        footer += $"<td>{this.columns[i].EvaluateFooter(this.Model)}</td>";
                    }
                    else
                    {
                        if (canSpan)
                            spanValue++;
                        else
                        {
                            footer += "<td>&nbsp;</td>";
                        }
                    }
                }

                footer = $"<tfoot><tr>{footer}</tr></tfoot>";
            }

            var table = $"<table class=\"table table-striped table-bordered table-hover table-condensed\"><thead><tr>{headRow}</tr></thead>{footer}<tbody>{bodyRows}</tbody></table>";

            return table;
        }

        public ITableBuilderOptions<TModel> Columns(Action<ColumnBuilder<TModel>> builderAction)
        {
            var builder = new ColumnBuilder<TModel>(this);
            builderAction.Invoke(builder);
            return this;
        }

        public ITableBuilderOptions<TModel> Attributes(object htmlAttributes)
        {
            throw new NotImplementedException();
        }

        internal void AddColumn<TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            var column = new TableColumn<TModel, TProperty>(expression, this.Model);
            this.columns.Add(column);
        }

        internal void AddColumn<TProperty>(ITableColumn<TModel> column)
        {
            this.columns.Add(column);
        }
    }
}