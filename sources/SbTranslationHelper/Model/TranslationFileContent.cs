using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.Model
{

    /// <summary>
    /// Content of a translation file
    /// </summary>
    public class TranslationFileContent
    {

        /// <summary>
        /// Create a new file content
        /// </summary>
        public TranslationFileContent(TranslationFile file, String group)
        {
            if (file == null)
                throw new ArgumentNullException("file");
            this.File = file;
            this.Group = group;
            this.Content = new List<TranslationFileValue>();
        }

        /// <summary>
        /// Load a content
        /// </summary>
        public static TranslationFileContent Load(TranslationFile file, String group)
        {
            TranslationFileContent content = new TranslationFileContent(file, group);
            content.LoadContent();
            return content;
        }

        /// <summary>
        /// Load the content
        /// </summary>
        public void LoadContent()
        {
            Content.Clear();
            Content.AddRange(TranslationsHelper.ReadFile(File, this.Group));
        }

        /// <summary>
        /// Save the content
        /// </summary>
        public bool SaveContent(bool backupExistingFile)
        {
            // If neutral we don't save data
            if (File.IsNeutral) return false;
            // Write the content
            return TranslationsHelper.WriteFile(File, this.Group, Content, backupExistingFile);
        }

        /// <summary>
        /// File
        /// </summary>
        public TranslationFile File { get; private set; }

        /// <summary>
        /// Group
        /// </summary>
        public String Group { get; private set; }

        /// <summary>
        /// List of the translations content
        /// </summary>
        public List<TranslationFileValue> Content { get; private set; }

    }

}
