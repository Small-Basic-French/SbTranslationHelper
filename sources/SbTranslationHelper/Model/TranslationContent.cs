using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.Model
{

    /// <summary>
    /// Content of translation file with neutral reference
    /// </summary>
    public class TranslationContent
    {
        private TranslationFileContent _NeutralFileContent, _TranslationsFileContent;

        /// <summary>
        /// Create a new content
        /// </summary>
        public TranslationContent(String group, TranslationFile neutral, TranslationFile translations)
        {
            if (neutral == null)
                throw new ArgumentNullException("neutral");
            this.Group = group;
            this.Content = new List<TranslationContentValue>();
            this.NeutralFile = neutral;
            this.TranslationsFile = translations;
        }

        /// <summary>
        /// Load the content
        /// </summary>
        public void LoadContent()
        {
            Content.Clear();
            _NeutralFileContent = TranslationFileContent.Load(NeutralFile, this.Group);
            if (TranslationsFile == null)
            {
                _TranslationsFileContent = null;
            }
            else
            {
                _TranslationsFileContent = TranslationFileContent.Load(TranslationsFile, this.Group);
            }
            // Merge the values
            foreach (var nValue in _NeutralFileContent.Content)
            {
                TranslationFileValue tValue = null;
                if (_TranslationsFileContent != null)
                {
                    int pos = Content.Count(c => String.Equals(c.ReferenceKey, nValue.ReferenceKey, StringComparison.OrdinalIgnoreCase));
                    var tValues = _TranslationsFileContent.Content.Where(c => String.Equals(c.ReferenceKey, nValue.ReferenceKey, StringComparison.OrdinalIgnoreCase)).ToList();

                    tValue = tValues.Skip(pos).FirstOrDefault();
                    if (tValue == null)
                    {
                        tValue = new TranslationFileValue
                        {
                            ReferenceGroup = nValue.ReferenceGroup,
                            ReferenceCode = nValue.ReferenceCode,
                            Description = nValue.Description
                        };
                        _TranslationsFileContent.Content.Add(tValue);
                    }
                }
                TranslationContentValue cValue = new TranslationContentValue(this, nValue, tValue);
                Content.Add(cValue);
            }
        }

        /// <summary>
        /// Save the content
        /// </summary>
        public bool SaveContent(bool backupExistingFile)
        {
            if (_TranslationsFileContent == null) return false;
            return _TranslationsFileContent.SaveContent(backupExistingFile);
        }

        /// <summary>
        /// Group
        /// </summary>
        public String Group { get; private set; }

        /// <summary>
        /// File of the neutral values
        /// </summary>
        public TranslationFile NeutralFile { get; private set; }

        /// <summary>
        /// File of the translations values
        /// </summary>
        public TranslationFile TranslationsFile { get; private set; }

        /// <summary>
        /// Content
        /// </summary>
        public List<TranslationContentValue> Content { get; private set; }

    }

    /// <summary>
    /// Value content
    /// </summary>
    public class TranslationContentValue
    {
        internal TranslationContentValue(TranslationContent content, TranslationFileValue neutral, TranslationFileValue translation)
        {
            this.Content = content;
            this.Neutral = neutral;
            this.Translation = translation;
            this.ReferenceGroup = this.Neutral.ReferenceGroup;
            this.ReferenceCode = this.Neutral.ReferenceCode;
            this.ReferenceKey = this.Neutral.ReferenceKey;
            this.Description = this.Neutral.Description;
            if (String.IsNullOrWhiteSpace(this.Description))
                this.Description = this.Translation != null ? this.Translation.Description : String.Empty;
            this.NeutralValue = this.Neutral.Translation;
        }
        internal TranslationContent Content { get; private set; }
        internal TranslationFileValue Neutral { get; private set; }
        internal TranslationFileValue Translation { get; private set; }

        /// <summary>
        /// Indicates if we have a translation
        /// </summary>
        /// <remarks>
        /// If false the it's a neutral only content.
        /// </remarks>
        public bool HasTranslation { get { return Translation != null; } }

        /// <summary>
        /// Reference group
        /// </summary>
        public string ReferenceGroup { get; private set; }

        /// <summary>
        /// Reference code
        /// </summary>
        public string ReferenceCode { get; private set; }

        /// <summary>
        /// Reference key
        /// </summary>
        public string ReferenceKey { get; private set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Neutral value
        /// </summary>
        public string NeutralValue { get; private set; }

        /// <summary>
        /// Translation
        /// </summary>
        public string TranslationValue
        {
            get { return HasTranslation ? this.Translation.Translation : null; }
            set
            {
                if (HasTranslation)
                    this.Translation.Translation = value;
            }
        }

    }

}
