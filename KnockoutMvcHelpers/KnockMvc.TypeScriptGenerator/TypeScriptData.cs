namespace KnockMvc.TypeScriptGenerator
{
    /// <summary>
    /// Class to hold generated TS data.
    /// </summary>
    public class TypeScriptData
    {
        public string Namespace { get; set; }

        public string Name { get; set; }

        public string Contents { get; set; }

        public TypeScriptDataContentTypeEnum ContentType { get; set; }
    }
}
