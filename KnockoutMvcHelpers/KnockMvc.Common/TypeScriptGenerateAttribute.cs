using System;

namespace KnockMvc.Common
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class TypeScriptGenerateAttribute : Attribute
    {
    }
}
