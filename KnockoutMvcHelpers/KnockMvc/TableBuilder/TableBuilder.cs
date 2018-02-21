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

        private string tableCss;

        private string footerText;

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
                        cellValue = column.Template.Replace(column.TemplateSpecifier, cellValue);

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
                var spanSize = 0;
                var footerTextAdded = false;
                var footerText = this.footerText;
                if (string.IsNullOrWhiteSpace(this.footerText))
                    footerText = "&nbsp;";

                for (int i = 0; i < colCount; i++)
                {
                    var cssClass = this.columns[i].FooterCssClass != null ? $" class=\"{this.columns[i].FooterCssClass}\"" : string.Empty;
                    if (string.IsNullOrEmpty(cssClass))
                        cssClass = this.columns[i].CssClass != null ? $" class=\"{this.columns[i].CssClass}\"" : string.Empty;

                    if (this.columns[i].FooterExpression != null)
                    {
                        if (spanSize > 0)
                        {
                            footer += $"<td colspan=\"{spanSize}\">{footerText}</td>";

                            // ensure that we don't add footer text multiple times
                            if (!footerTextAdded)
                            {
                                footerTextAdded = true;
                                footerText = "&nbsp;";
                            }

                            spanSize = 0;
                        }

                        var cellValue = this.columns[i].EvaluateFooter(this.Model);
                        if (!string.IsNullOrEmpty(this.columns[i].Template))
                            cellValue = this.columns[i].Template.Replace(this.columns[i].TemplateSpecifier, cellValue);

                        footer += $"<td{cssClass}>{cellValue}</td>";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(this.columns[i].CssClass)
                            || !string.IsNullOrEmpty(this.columns[i].FooterCssClass))
                        {
                            if (spanSize > 0)
                            {
                                footer += $"<td colspan=\"{spanSize}\">{footerText}</td>";

                                // ensure that we don't add footer text multiple times
                                if (!footerTextAdded)
                                {
                                    footerTextAdded = true;
                                    footerText = "&nbsp;";
                                }

                                spanSize = 0;
                            }

                            footer += $"<td{cssClass}>{footerText}</td>";

                            // ensure that we don't add footer text multiple times
                            if (!footerTextAdded)
                            {
                                footerTextAdded = true;
                                footerText = "&nbsp;";
                            }
                        }
                        else
                        {
                            spanSize++;

                            // add empty footer for the last column
                            if (i == colCount - 1)
                                footer += $"<td colspan=\"{spanSize}\">&nbsp;</td>";
                        }
                    }
                }

                footer = $"<tfoot><tr>{footer}</tr></tfoot>";
            }

            var table = $"<table class=\"{this.tableCss}\"><thead><tr>{headRow}</tr></thead>{footer}<tbody>{bodyRows}</tbody></table>";

            return table;
        }

        public ITableBuilderOptions<TModel> Columns(Action<ColumnBuilder<TModel>> builderAction)
        {
            var builder = new ColumnBuilder<TModel>(this);
            builderAction.Invoke(builder);
            return this;
        }

        public ITableBuilderOptions<TModel> FooterText(string footerText)
        {
            this.footerText = footerText;
            return this;
        }

        public ITableBuilderOptions<TModel> Css(string cssClass)
        {
            this.tableCss = cssClass;
            return this;
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