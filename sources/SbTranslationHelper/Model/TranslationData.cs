namespace SbTranslationHelper.Model
{
    /// <summary>
    /// Translation data
    /// </summary>
    public class TranslationData
    {
        public string ReferenceGroup { get; set; }
        public string ReferenceCode { get; set; }
        public string ReferenceKey { get { return string.Format("{0}.{1}", ReferenceGroup, ReferenceCode); } }
        public string Description { get; set; }
        public string NeutralValue { get; set; }
        public string Translation { get; set; }
    }
}