﻿using System;
using System.Collections.Generic;

namespace KnockMvc.TableHelper
{
    /// <summary>
    /// Data class to store user-defined attribute.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    public class TableAttributeData<TModel> where TModel : class
    {
        /// <summary>
        /// Gets or sets the name of the attribute.
        /// </summary>
        /// <value>
        /// The name of the attribute.
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value of the attribute.
        /// </summary>
        /// <value>
        /// The value of the attribute.
        /// </value>
        public Func<ICollection<TModel>, object> Value { get; set; }
    }
}