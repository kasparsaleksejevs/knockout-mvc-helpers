using KnockMvc.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KnockMvc.DTO
{
    [TypeScriptGenerate]
    public class MyRow
    {
        [Display(Name = "INT Value")]
        public int IntValue { get; set; }

        [Display(Name = "A Person")]
        public Person Person { get; set; }

        [Display(Name = "Int Arr")]
        public int[] IntArr { get; set; }

        [Display(Name = "Int List")]
        public List<int> IntList { get; set; }

        [Display(Name = "Nullable Int List")]
        public List<int?> NullableIntList { get; set; }

        [Display(Name = "Double Value")]
        [DisplayFormat(DataFormatString = "N3")]
        public double DoubleValue { get; set; }

        [Display(Name = "Decimal...")]
        public decimal DecimalValue { get; set; }

        [Display(Name = "Nullable Dec")]
        public decimal? NullableDecimalValue { get; set; }

        [Display(Name = "String!")]
        public string StringValue { get; set; }

        [Display(Name = "Bool")]
        public bool BoolValue { get; set; }

        [Display(Name = "Date")]
        public DateTime DateValue { get; set; }

        [Display(Name = "Enum Value")]
        public MyEnum EnumValue { get; set; }
    }

    [TypeScriptGenerate]
    public class Person
    {
        public string FirstName { get; set; }

        public int Age { get; set; }
    }

    public enum MyEnum
    {
        [Display(Name = "Value 1")]
        [Description("Value 1 (Descr)")]
        Val1 = 1,

        [Description("Other (Descr)")]
        Val2 = 2
    }
}
