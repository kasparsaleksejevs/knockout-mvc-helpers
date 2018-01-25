using System;

namespace KnockoutHelpers.TableHelper
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
    }
}