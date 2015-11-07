using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.Model
{
    /// <summary>
    /// Informations about a translation file
    /// </summary>
    public class TranslationFile
    {
        /// <summary>
        /// Create a new translation file from a full file name
        /// </summary>
        public TranslationFile(String file)
        {
            File = file;
            Folder = Path.GetDirectoryName(file);
            FileType = Path.GetExtension(file);
            if (!String.IsNullOrWhiteSpace(FileType) && FileType.StartsWith("."))
                FileType = FileType.Substring(1);
            else
                FileType = null;
            FileName = Path.GetFileNameWithoutExtension(file);
            Language = Path.GetExtension(FileName);
            if (!String.IsNullOrWhiteSpace(Language) && Language.StartsWith("."))
            {
                Language = Language.Substring(1);
                FileName = Path.GetFileNameWithoutExtension(FileName);
            }
            else
            {
                Language = null;
            }
        }

        /// <summary>
        /// Create a new translation file from his elements
        /// </summary>
        public TranslationFile(String folder, String file, String fileType, String language = null)
        {
            this.Folder = folder;
            this.FileName = file;
            this.FileType = fileType;
            this.Language = language;
            StringBuilder sb = new StringBuilder();
            sb.Append(Path.Combine(Folder, FileName));
            if (!String.IsNullOrWhiteSpace(Language))
                sb.Append(".").Append(Language);
            if (!String.IsNullOrWhiteSpace(FileType))
                sb.Append(".").Append(FileType);
            this.File = sb.ToString();
        }

        /// <summary>
        /// Create a new translation file for an another language
        /// </summary>
        /// <param name="language">Language code or null if neutral.</param>
        public TranslationFile MakeFileFromLanguage(String language)
        {
            return new TranslationFile(this.Folder, this.FileName, this.FileType, language);
        }

        /// <summary>
        /// Full name of the file
        /// </summary>
        public String File { get; private set; }
        /// <summary>
        /// Folder
        /// </summary>
        public String Folder { get; private set; }
        /// <summary>
        /// File name without folder and extension
        /// </summary>
        public String FileName { get; private set; }
        /// <summary>
        /// File type (extension without .)
        /// </summary>
        public String FileType { get; private set; }
        /// <summary>
        /// Language code or null if neutral
        /// </summary>
        public String Language { get; private set; }

        /// <summary>
        /// Indicates if this file is neutral
        /// </summary>
        public bool IsNeutral { get { return String.IsNullOrWhiteSpace(Language); } }
    }

}
