using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.Model
{
    /// <summary>
    /// Group of translation files
    /// </summary>
    public class TranslationGroupFiles
    {

        /// <summary>
        /// Create a new group
        /// </summary>
        public TranslationGroupFiles()
        {
            Files = new List<TranslationFile>();
        }

        /// <summary>
        /// Add a file in the group
        /// </summary>
        public TranslationFile AddFile(String file)
        {
            var tf = FindFile(file);
            if (tf == null)
            {
                tf = new TranslationFile(file);
                Files.Add(tf);
            }
            return tf;
        }

        /// <summary>
        /// Find a translation file
        /// </summary>
        public TranslationFile FindFile(String file)
        {
            return Files.FirstOrDefault(tf =>
                String.Equals(file, tf.File, StringComparison.OrdinalIgnoreCase)
                || String.Equals(file, tf.FileName, StringComparison.OrdinalIgnoreCase)
            );
        }

        /// <summary>
        /// Name of the goup
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Caption
        /// </summary>
        public String Caption { get; set; }

        /// <summary>
        /// Files in the group
        /// </summary>
        public List<TranslationFile> Files { get; private set; }
    }
}
