using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace KnockMvc.TableHelper
{
    public class TableBuilder<TModel> : ITableBuilderOptions<TModel> where TModel : class
    {
        private List<ITableColumn<TModel>> columns = new List<ITableColumn<TModel>>();

        private HtmlHelper htmlHelper;

        private string tableCss;

        /// <summary>
        /// Transposes columns to rows, so that rows are displayed as columns and table has so called 'horizontal-flow'.
        /// Keep in mind that horizontal space may be limited and horizontal scrolling is not associated with good UX. 
        /// This could be suitable when there are multiple columns and only a few data rows, prefferably with a common data-type.
        /// </summary>
        private bool useColumnsAsRows;

        private string footerText;

        private string customHeaderRow;

        public TableBuilder(HtmlHelper html, ICollection<TModel> model)
        {
            this.htmlHelper = html;
            this.Model = model;
        }

        internal ICollection<TModel> Model { get; set; }

        internal ICollection<GroupedData<TModel>> Groups { get; set; }

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

        /// <summary>
        /// Switches the table to columns-as-rows mode.
        /// </summary>
        /// <returns>Table builder instance.</returns>
        public ITableBuilderOptions<TModel> UseColumnsAsRows()
        {
            this.useColumnsAsRows = true;
            return this;
        }

        /// <summary>
        /// Adds the custom header HTML row string before any other auto-generated header row.
        /// If <c>thead</c> block doesn't exist (e.g. for horizontal-flow table) - it is created. 
        /// </summary>
        /// <returns>Table builder instance.</returns>
        /// <example>.AddCustomHeaderRow("<tr><th colspan="3">Spanned title</th><th>Other title</th></tr>")</example>
        public ITableBuilderOptions<TModel> AddCustomHeaderRow(string customHeaderRowHtml)
        {
            this.customHeaderRow = customHeaderRowHtml;
            return this;
        }

        /// <summary>
        /// Groups the data by specified expression/property and displays groups as additional rows.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="expression">The expression to group by.</param>
        /// <param name="cssClass">The optional CSS class to add to group header cell.</param>
        /// <returns>
        /// Table builder instance.
        /// </returns>
        public ITableBuilderOptions<TModel> GroupBy<TProperty>(Expression<Func<TModel, TProperty>> expression, string cssClass = null)
        {
            this.Groups = new List<GroupedData<TModel>>();
            var groupingDummyColumn = new TableColumn<TModel, TProperty>(expression, this.Model);
            var groupData = this.Model.GroupBy(expression.Compile());
            foreach (var item in groupData)
            {
                var keyVal = groupingDummyColumn.EvaluateInternal(item.Key, null);
                this.Groups.Add(new GroupedData<TModel> { GroupHeaderText = keyVal, DataRows = item.ToList(), GroupHeaderCss = cssClass });
            }

            return this;
        }

        /// <summary>
        /// Renders the table and outputs as html string.
        /// </summary>
        /// <returns>Html string containing rendered table.</returns>
        private string Render()
        {
            if (this.useColumnsAsRows)
                return this.RenderForColumnsAsRows();

            var colCount = this.columns.Count;
            var hasFooter = this.columns.Any(m => m.FooterExpression != null);

            var theadRows = string.Empty;
            if (!string.IsNullOrWhiteSpace(this.customHeaderRow))
            {
                var rowCount = 1 + this.Model.Count + (hasFooter ? 1 : 0);

                this.customHeaderRow = this.customHeaderRow.Replace("{columnCount}", this.columns.Count.ToString(CultureInfo.InvariantCulture));
                this.customHeaderRow = this.customHeaderRow.Replace("{rowCount}", rowCount.ToString(CultureInfo.InvariantCulture));
                theadRows += this.customHeaderRow;
            }

            var mainTheadRow = string.Empty;
            foreach (var column in this.columns)
            {
                if (column.IsSpacer)
                {
                    var cssClass = column.CssClass != null ? $" class=\"{column.CssClass}\"" : string.Empty;
                    mainTheadRow += $"<td{cssClass}>&nbsp;</th>";
                    continue;
                }

                var headerCssClass = column.HeaderCssClass != null ? $" class=\"{column.HeaderCssClass}\"" : string.Empty;
                mainTheadRow += $"<th{headerCssClass}>{(string.IsNullOrEmpty(column.ColumnTitle) ? "&nbsp;" : column.ColumnTitle)}</th>";
            }

            theadRows += $"<tr>{mainTheadRow}</tr>";
            var bodyRows = string.Empty;
            if (this.Groups != null && this.Groups.Count > 0)
            {
                foreach (var item in this.Groups)
                {
                    bodyRows += GenerateBodyGroupHeader(item.GroupHeaderText, item.GroupHeaderCss);
                    bodyRows += GenerateBodyRows(item.DataRows);
                }
            }
            else
                bodyRows = GenerateBodyRows(this.Model);

            var footer = string.Empty;
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

            var table = $"<table class=\"{this.tableCss}\"><thead>{theadRows}</thead>{footer}<tbody>{bodyRows}</tbody></table>";

            return table;
        }

        private string GenerateBodyGroupHeader(string groupHeaderText, string cssClass)
        {
            var cssString = string.Empty;
            if (!string.IsNullOrEmpty(cssClass))
                cssString = $"class=\"{cssClass}\" ";

            return $"<tr><td {cssString}colspan=\"{this.columns.Count}\">{groupHeaderText}</td></tr>";
        }

        private string GenerateBodyRows(ICollection<TModel> model)
        {
            int rowNum = 0;
            var bodyRows = string.Empty;
            foreach (var row in model)
            {
                var bodyRow = string.Empty;
                foreach (var column in this.columns)
                {
                    var cssClass = column.CssClass;
                    var attributes = string.Empty;
                    if (column.Attributes.Count > 0)
                        foreach (var attribute in column.Attributes)
                        {
                            if (attribute.Name.ToLower() == "class")
                            {
                                cssClass += " " + HttpUtility.HtmlAttributeEncode(attribute.Value(row).ToString());
                                continue;
                            }

                            attributes += $" {attribute.Name}=\"{HttpUtility.HtmlAttributeEncode(attribute.Value(row).ToString())}\"";
                        }

                    cssClass = cssClass != null ? $" class=\"{cssClass.Trim()}\"" : string.Empty;

                    if (column.IsSpacer)
                    {
                        if (rowNum == 0)
                        {
                            var spanCount = model.Count;
                            if (this.columns.Any(m => m.FooterExpression != null))
                                spanCount++;

                            bodyRow += $"<td{cssClass} rowspan=\"{spanCount}\">{column.Evaluate(model.FirstOrDefault())}</th>";
                        }

                        continue;
                    }

                    var cellValue = column.Evaluate(row);
                    if (!string.IsNullOrEmpty(column.Template))
                        cellValue = column.Template.Replace(column.TemplateSpecifier, cellValue);

                    if (column.IsHeader)
                        bodyRow += $"<th{cssClass}{attributes}>{cellValue}</th>";
                    else
                        bodyRow += $"<td{cssClass}{attributes}>{cellValue}</td>";
                }

                bodyRows += $"<tr>{bodyRow}</tr>";
                rowNum++;
            }

            return bodyRows;
        }

        /// <summary>
        /// Renders the table (on columns-as-rows mode) and outputs as html string.
        /// </summary>
        /// <returns>Html string containing rendered table.</returns>
        private string RenderForColumnsAsRows()
        {
            var bodyRows = string.Empty;
            var footerTextAdded = false;
            var footerTextValue = this.footerText;
            var hasFooter = this.columns.Any(m => m.FooterExpression != null);

            string theadRows = null;
            if (!string.IsNullOrWhiteSpace(this.customHeaderRow))
            {
                var rowCount = 1 + this.Model.Count + (hasFooter ? 1 : 0);

                this.customHeaderRow = this.customHeaderRow.Replace("{columnCount}", this.columns.Count.ToString(CultureInfo.InvariantCulture));
                this.customHeaderRow = this.customHeaderRow.Replace("{rowCount}", rowCount.ToString(CultureInfo.InvariantCulture));
                theadRows = $"<thead>{this.customHeaderRow}</thead>";
            }

            for (int i = 0; i < this.columns.Count; i++)
            {
                var column = this.columns[i];
                if (column.IsSpacer)
                {
                    var spanCount = 1 + this.Model.Count;
                    if (this.columns.Any(m => m.FooterExpression != null))
                        spanCount++;

                    var spacerCssClass = column.CssClass != null ? $" class=\"{column.CssClass}\"" : string.Empty;
                    bodyRows += $"<tr><td{spacerCssClass} colspan=\"{spanCount}\">{column.Evaluate(Model.FirstOrDefault())}</td></tr>";
                    continue;
                }

                var headerCssClass = column.HeaderCssClass != null ? $" class=\"{column.HeaderCssClass}\"" : string.Empty;
                var bodyRow = $"<th{headerCssClass}>{(string.IsNullOrEmpty(column.ColumnTitle) ? " &nbsp; " : column.ColumnTitle)}</th>";

                foreach (var row in this.Model)
                {
                    var cssClass = column.CssClass;
                    var cellValue = column.Evaluate(row);
                    if (!string.IsNullOrEmpty(column.Template))
                        cellValue = column.Template.Replace(column.TemplateSpecifier, cellValue);

                    var attributes = string.Empty;
                    if (column.Attributes.Count > 0)
                        foreach (var attribute in column.Attributes)
                        {
                            if (attribute.Name.ToLower() == "class")
                            {
                                cssClass += " " + HttpUtility.HtmlAttributeEncode(attribute.Value(row).ToString());
                                continue;
                            }

                            attributes += $" {attribute.Name}=\"{HttpUtility.HtmlAttributeEncode(attribute.Value(row).ToString())}\"";
                        }

                    cssClass = cssClass != null ? $" class=\"{cssClass.Trim()}\"" : string.Empty;
                    if (column.IsHeader)
                        bodyRow += $"<th{cssClass}{attributes}>{cellValue}</th>";
                    else
                        bodyRow += $"<td{cssClass}{attributes}>{cellValue}</td>";
                }

                if (hasFooter)
                {
                    var footerCssClass = column.FooterCssClass != null ? $" class=\"{column.FooterCssClass}\"" : string.Empty;
                    if (string.IsNullOrEmpty(footerCssClass))
                        footerCssClass = column.CssClass != null ? $" class=\"{column.CssClass}\"" : string.Empty;

                    if (column.FooterExpression != null)
                    {
                        var cellValue = column.EvaluateFooter(this.Model);
                        if (!string.IsNullOrEmpty(column.Template))
                            cellValue = column.Template.Replace(column.TemplateSpecifier, cellValue);

                        bodyRow += $"<td{footerCssClass}>{cellValue}</td>";
                    }
                    else
                    {
                        bodyRow += $"<td{footerCssClass}>{footerTextValue}</td>";

                        if (!footerTextAdded)
                        {
                            footerTextAdded = true;
                            footerTextValue = "&nbsp;";
                        }
                    }
                }

                bodyRows += $"<tr>{bodyRow}</tr>";
            }

            return $"<table class=\"{this.tableCss}\">{theadRows}<tbody>{bodyRows}</tbody></table>";
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

    public class GroupedData<TModel>
    {
        public string GroupHeaderText { get; set; }

        public string GroupHeaderCss { get; set; }

        public ICollection<TModel> DataRows { get; set; }
    }
}