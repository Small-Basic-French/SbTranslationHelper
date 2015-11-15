using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
        /// Open a content
        /// </summary>
        public TranslationContent OpenContent(String file)
        {
            var tf = FindFile(file);
            if (tf == null) throw new ArgumentException(Locales.SR.Error_FileNotFoundInGroup);
            TranslationFile neutralFile = null;
            TranslationFile transFile = null;
            if (tf.IsNeutral)
            {
                neutralFile = tf;
            }
            else
            {
                neutralFile = Files.FirstOrDefault(f => f.IsNeutral);
                transFile = tf;
            }
            var result = new TranslationContent(this.Name, neutralFile, transFile);
            result.LoadContent();
            return result;
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
