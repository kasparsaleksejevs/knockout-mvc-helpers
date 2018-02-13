﻿using System;
using System.Collections.Generic;

namespace KnockMvc.TableHelper
{
    /// <summary>    
    /// Properties and methods used within the TableBuilder class.    
    /// </summary>    
    public interface ITableColumn<TModel> where TModel : class
    {
        string ColumnTitle { get; set; }

        string ColumnName { get; set; }

        Type ColumnType { get; set; }

        string Format { get; set; }

        string HeaderCssClass { get; set; }

        string CssClass { get; set; }

        bool IsHeader { get; set; }

        string Template { get; set; }

        string BooleanTrueValue { get; set; }

        string BooleanFalseValue { get; set; }

        string Evaluate(TModel model);

        string EvaluateFooter(ICollection<TModel> model);

        Func<ICollection<TModel>, object> FooterExpression { get; set; }

        Func<TModel, TExpression> GetValueExpression<TExpression>();
    }
}