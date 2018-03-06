using System;
using System.Collections.Generic;

namespace KnockMvc.TableHelper
{
    /// <summary>    
    /// Properties and methods used within the TableBuilder class.    
    /// </summary>    
    public interface ITableColumn<TModel> where TModel : class
    {
        /// <summary>
        /// Gets or sets the column title to display in the table.
        /// </summary>
        /// <value>
        /// The column title.
        /// </value>
        string ColumnTitle { get; set; }

        /// <summary>
        /// Gets or sets the name of the column.
        /// </summary>
        /// <value>
        /// The name of the column.
        /// </value>
        string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the type of the column.
        /// </summary>
        /// <value>
        /// The type of the column.
        /// </value>
        Type ColumnType { get; set; }

        /// <summary>
        /// Gets or sets the format to apply to the cell value (e.g., ToString("format")).
        /// </summary>
        /// <value>
        /// The format.
        /// </value>
        string Format { get; set; }

        /// <summary>
        /// Gets or sets the header CSS class.
        /// </summary>
        /// <value>
        /// The header CSS class.
        /// </value>
        string HeaderCssClass { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for the table cell.
        /// </summary>
        /// <value>
        /// The CSS class.
        /// </value>
        string CssClass { get; set; }

        /// <summary>
        /// Gets or sets the footer CSS class.
        /// If footer CSS class is not specified, the CssClass property value is applied.
        /// </summary>
        /// <value>
        /// The footer CSS class.
        /// </value>
        string FooterCssClass { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is a 'header' - the <c>th</c> tag will be applied to the table cell instead of regular <c>td</c> tag.
        /// </summary>
        /// <value>
        ///   <c>True</c> if this column is to be displayed as header; otherwise, <c>false</c>.
        /// </value>
        bool IsHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this column is a 'spacer' column.
        /// This can be useful mainly in horizontal flow columns, where this can create table 'split'.
        /// </summary>
        /// <value>
        ///   <c>True</c> if this column is a spacer; otherwise, <c>false</c>.
        /// </value>
        bool IsSpacer { get; set; }

        /// <summary>
        /// Gets or sets the template to apply to the formatted cell value.
        /// By default the string '{value}' defines where the formatted cell value will be put in the resulting string.
        /// </summary>
        /// <value>
        /// The template.
        /// </value>
        /// <example>col.Bind(m => m.DecimalValue).Format("N2").Template("{value}x"); - will display value like '123.34x'.</example>
        string Template { get; set; }

        /// <summary>
        /// Gets or sets the template specifier, which will be later replaced with the actual cell value.
        /// This should be changed only for edge cases where the resulting text must contain actual string '{value}'.
        /// </summary>
        /// <value>
        /// The template specifier (default is '{value}').
        /// </value>
        string TemplateSpecifier { get; set; }

        /// <summary>
        /// Gets or sets the boolean true value representation as string (e.g., "Yes", "Ok", etc).
        /// Used only for boolean type columns.
        /// </summary>
        /// <value>
        /// The boolean true value.
        /// </value>
        string BooleanTrueValue { get; set; }

        /// <summary>
        /// Gets or sets the boolean false value representation as string (e.g., "No", "Declined", etc).
        /// Used only for boolean type columns.
        /// </summary>
        /// <value>
        /// The boolean false value.
        /// </value>
        string BooleanFalseValue { get; set; }

        /// <summary>    
        /// Get the property value from a model object.    
        /// </summary>    
        /// <param name="model">Model to get the property value from.</param>    
        /// <returns>Property value from the model.</returns>    
        string Evaluate(TModel model);

        /// <summary>
        /// Gets or sets the footer expression.
        /// </summary>
        /// <value>
        /// The footer expression.
        /// </value>
        Func<ICollection<TModel>, object> FooterExpression { get; set; }

        /// <summary>
        /// Gets or sets the HTML attributes to apply to the column.
        /// </summary>
        /// <value>
        /// The HTML attributes.
        /// </value>
        IList<AttributeData<TModel>> Attributes { get; set; }

        string EvaluateFooter(ICollection<TModel> model);

        Func<TModel, TExpression> GetValueExpression<TExpression>();
    }
}