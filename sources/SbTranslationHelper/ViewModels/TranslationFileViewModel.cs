using SbTranslationHelper.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels
{

    /// <summary>
    /// Translation file
    /// </summary>
    public class TranslationFileViewModel : ObservableObject
    {

        /// <summary>
        /// New translation file
        /// </summary>
        public TranslationFileViewModel(TranslationFile file)
        {
            this.File = file;
            if (this.File.IsNeutral)
            {
                this.Caption = Locales.SR.NeutralLanguageFile_Caption;
            }
            else
            {
                try
                {
                    this.Culture = CultureInfo.GetCultureInfo(file.Language);
                }
                catch { }
                if (this.Culture == null)
                    this.Caption = String.Format(Locales.SR.UnknownLanguageFile_Caption, this.File.Language);
                else
                    this.Caption = this.Culture.DisplayName;
            }
        }

        /// <summary>
        /// File data
        /// </summary>
        public TranslationFile File { get; private set; }

        /// <summary>
        /// Language code
        /// </summary>
        public String Language { get { return File.Language; } }

        /// <summary>
        /// Culture of the file
        /// </summary>
        public CultureInfo Culture { get; private set; }

        /// <summary>
        /// Caption
        /// </summary>
        public String Caption { get; private set; }

    }

}
