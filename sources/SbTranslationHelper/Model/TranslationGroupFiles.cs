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

        IEnumerable<TranslationData> ReadExecutableFile(String file)
        {
            // Load the assembly
            Assembly asm = Assembly.LoadFile(file);
            // Explore all resources
            var resourceNames = asm.GetManifestResourceNames().Where(n => n.EndsWith(".resources"));
            foreach (var resName in resourceNames)
            {
                // Cleanup the name
                var cleanResName = resName.Replace(".resources", "");
                // Open the stream for String type
                using (var stream = asm.GetManifestResourceStream(resName))
                {
                    if (stream != null)
                    {
                        using (var reader = new ResourceReader(stream))
                        {
                            foreach (DictionaryEntry item in reader)
                            {
                                if (!(item.Key is string) && !(item.Value is string))
                                    continue;
                                TranslationData tData = new TranslationData
                                {
                                    ReferenceGroup = this.Name,
                                    ReferenceCode = item.Key.ToString(),
                                    Translation = item.Value.ToString()
                                };
                                yield return tData;
                            }
                        }
                    }
                }
            }
            yield break;
        }
        IEnumerable<TranslationData> ReadResourceFile(String file)
        {
            // Load the file
            XDocument xdoc = XDocument.Load(file, LoadOptions.None);
            // Check the root
            if (xdoc.Root.Name != "root")
                throw new InvalidOperationException(Locales.SR.Error_Project_InvalidResourceFile);
            // Search all 'data' tag
            foreach (var dataTag in xdoc.Root.Elements("data"))
            {
                TranslationData tData = new TranslationData
                {
                    ReferenceGroup = this.Name,
                    ReferenceCode = (string)dataTag.Attribute("name"),
                    Description = (string)dataTag.Element("comment"),
                    Translation = (string)dataTag.Element("value")
                };
                yield return tData;
            }
        }
        IEnumerable<TranslationData> ReadXmlDocFile(String file)
        {
            // Load the file
            XDocument xdoc = XDocument.Load(file, LoadOptions.None);
            // Check the root
            if (xdoc.Root.Name != "doc")
                throw new InvalidOperationException(Locales.SR.Error_Project_InvalidXmlDocFile);
            // Search all 'member' tag
            foreach (var memberTag in xdoc.Root.Descendants("member"))
            {
                String category = (string)memberTag.Attribute("name");
                foreach (var element in memberTag.Elements())
                {
                    String refObj =
                        element.Name.ToString()
                        + String.Concat(element.Attributes().OrderBy(a => a.Name).Select(a => String.Format("@{0}:{1}", a.Name, a.Value)));
                    String transValue = String.Empty;
                    using(var rdr = element.CreateReader())
                    {
                        rdr.MoveToContent();
                        transValue = rdr.ReadInnerXml();
                    }
                    TranslationData tData = new TranslationData
                    {
                        ReferenceGroup = category,
                        ReferenceCode = refObj,
                        Translation = transValue
                    };
                    yield return tData;
                }
            }
        }
        IEnumerable<TranslationData> ReadFile(TranslationFile file)
        {
            if (file == null) return null;
            if (String.Equals("dll", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadExecutableFile(file.File);
            if (String.Equals("exe", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadExecutableFile(file.File);
            if (String.Equals("resx", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadResourceFile(file.File);
            if (String.Equals("xml", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadXmlDocFile(file.File);
            throw new ArgumentException(Locales.SR.Error_CantReadFileType, file.FileType);
        }
        IEnumerable<TranslationData> MergeTranslationFiles(IEnumerable<TranslationData> neutralFile, IEnumerable<TranslationData> transFile)
        {
            if (neutralFile == null)
            {
                if (transFile != null)
                {
                    foreach (var trans in transFile)
                        yield return trans;
                }
            }
            else
            {
                var transDict = new Dictionary<string, TranslationData>(StringComparer.OrdinalIgnoreCase);
                if (transFile != null)
                {
                    foreach (var trans in transFile)
                        transDict.Add(trans.ReferenceKey, trans);
                }
                foreach (var ntrans in neutralFile)
                {
                    ntrans.NeutralValue = ntrans.Translation;
                    TranslationData ttrans = null;
                    transDict.TryGetValue(ntrans.ReferenceKey, out ttrans);
                    if (ttrans != null)
                    {
                        ntrans.Translation = ttrans.Translation ?? ttrans.NeutralValue;
                        if (String.IsNullOrWhiteSpace(ntrans.Description))
                            ntrans.Description = ttrans.Description;
                    }
                    yield return ntrans;
                }
            }
        }
        /// <summary>
        /// Read translations from file
        /// </summary>
        public IEnumerable<TranslationData> ReadFile(String file)
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
            return MergeTranslationFiles(ReadFile(neutralFile), ReadFile(transFile));
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
