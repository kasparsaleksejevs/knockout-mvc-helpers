using System;

namespace KnockMvc.Web.Helpers.TableHelper
{
    /// <summary>    
    /// Properties and methods used within the TableBuilder class.    
    /// </summary>    
    public interface ITableColumn<TModel> where TModel : class
    {
        string ColumnTitle { get; set; }

        string ColumnName { get; set; }

        Type ColumnType { get; set; }

        FooterEnum FooterData { get; set; }

        string Evaluate(TModel model);

        string Format { get; set; }

        string HeaderCssClass { get; set; }

        string CssClass { get; set; }

        bool IsHeader { get; set; }

        bool NoTitle { get; set; }

        string BooleanTrueValue { get; set; }

        string BooleanFalseValue { get; set; }
    }
}