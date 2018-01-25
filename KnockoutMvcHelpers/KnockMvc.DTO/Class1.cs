using KnockMvc.Common;
using System;

namespace KnockMvc.DTO
{
    [TypeScriptGenerate]
    public class CustomClassData
    {
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
    public class MyStuff
    {
        public string FirstName { get; set; }

        public string Lastname { get; set; }

        public int Age { get; set; }
    }
}