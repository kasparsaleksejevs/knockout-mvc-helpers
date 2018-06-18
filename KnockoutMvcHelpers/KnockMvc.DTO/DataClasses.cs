using KnockMvc.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KnockMvc.DTO
{
    [TypeScriptGenerate]
    public class CustomClassData
    {
        public readonly string ReadonlyStr = "Readonly String";

        public const string ConstStr = "Constant String";

        public static readonly int ReadonlyStaticInt = 4200;

        public readonly int ReadonlyInt = 42;

        public readonly DateTime ReadonlyDate = DateTime.Now;

        public readonly decimal? ReadonlyDecNull = null;

        public readonly decimal ReadonlyDecOk = 42.56m;

        public readonly bool ReadonlyBool = true;

        public readonly List<string> ReadonlyStringList = new List<string> { "test1", "test2", "test3" };

        public readonly List<bool> ReadonlyBoolList = new List<bool> { true, false, true, true };

        public const int ConstInt = 420;

        public int SomeInteger { get; set; }

        public double? NiceDouble { get; set; }
    }

    [TypeScriptGenerate]
    public class OtherCustomData
    {
        public int SomeIntValue { get; set; }

        public double? SomeDoubleValue { get; set; }

        public string SomeString { get; set; }

        public DateTime SomeDate { get; set; }

        public int Kek { get; set; }
    }
}

namespace KnockMvc.DTO.MyStuff
{
    [TypeScriptGenerate]
    public class Person
    {
        public string FirstName { get; set; }

        public string Lastname { get; set; }

        public int Age { get; set; }
    }

    [TypeScriptGenerate]
    public class RowGeneratorTestData
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
}