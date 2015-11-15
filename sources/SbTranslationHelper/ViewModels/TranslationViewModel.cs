using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels
{

    /// <summary>
    /// Translation
    /// </summary>
    public class TranslationViewModel : ObservableObject
    {
        /// <summary>
        /// Create a new model
        /// </summary>
        public TranslationViewModel(TranslationEditorViewModel editor, Model.TranslationContentValue value)
        {
            this.Editor = editor;
            this.ContentValue = value;
        }

        /// <summary>
        /// Editor
        /// </summary>
        public TranslationEditorViewModel Editor { get; set; }

        /// <summary>
        /// Content value
        /// </summary>
        public Model.TranslationContentValue ContentValue { get; private set; }

        /// <summary>
        /// Reference group
        /// </summary>
        public String ReferenceGroup { get { return ContentValue.ReferenceGroup; } }

        /// <summary>
        /// Reference code
        /// </summary>
        public String ReferenceCode { get { return ContentValue.ReferenceCode; } }

        /// <summary>
        /// Reference key
        /// </summary>
        public String ReferenceKey { get { return ContentValue.ReferenceKey; } }

        /// <summary>
        /// Description
        /// </summary>
        public String Description { get { return ContentValue.Description; } }

        /// <summary>
        /// Neutral value
        /// </summary>
        public String NeutralValue { get { return ContentValue.NeutralValue; } }

        /// <summary>
        /// Translated value
        /// </summary>
        public String TranslatedValue
        {
            get { return ContentValue.TranslationValue; }
            set
            {
                if (ContentValue.TranslationValue != value)
                {
                    ContentValue.TranslationValue = value;
                    RaisePropertyChanged(() => TranslatedValue);
                }
            }
        }

    }

}
