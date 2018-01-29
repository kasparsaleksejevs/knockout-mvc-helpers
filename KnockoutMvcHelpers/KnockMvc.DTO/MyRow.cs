using KnockMvc.Common;
using System;

namespace KnockMvc.DTO
{
    [TypeScriptGenerate]
    public class MyRow
    {
        public int IntValue { get; set; }

        public double DoubleValue { get; set; }

        public decimal DecimalValue { get; set; }

        public string StringValue { get; set; }

        public bool BoolValue { get; set; }

        public DateTime DateValue { get; set; }

        public MyEnum EnumValue { get; set; }
    }

    public enum MyEnum
    {
        Val1 = 1,
        Val2 = 2
    }
}
