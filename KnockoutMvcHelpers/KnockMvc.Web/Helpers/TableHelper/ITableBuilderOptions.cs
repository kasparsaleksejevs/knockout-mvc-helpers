﻿using System;
using System.Web;

namespace KnockMvc.Web.Helpers.TableHelper
{
    public interface ITableBuilderOptions<TModel> : IHtmlString where TModel : class
    {
        ITableBuilderOptions<TModel> Columns(Action<ColumnBuilder<TModel>> builder);

        ITableBuilderOptions<TModel> Attributes(object htmlAttributes);
    }
}