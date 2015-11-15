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
    /// Translation read helpers
    /// </summary>
    public static class TranslationReader
    {

        /// <summary>
        /// Read the translations from an executable file
        /// </summary>
        public static IEnumerable<TranslationFileValue> ReadExecutableFile(String group, String file)
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
                                TranslationFileValue tData = new TranslationFileValue
                                {
                                    ReferenceGroup = group,
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

        /// <summary>
        /// Read the translations from a resource file
        /// </summary>
        public static IEnumerable<TranslationFileValue> ReadResourceFile(String group, String file)
        {
            // Load the file
            XDocument xdoc = XDocument.Load(file, LoadOptions.None);
            // Check the root
            if (xdoc.Root.Name != "root")
                throw new InvalidOperationException(Locales.SR.Error_Project_InvalidResourceFile);
            // Search all 'data' tag
            foreach (var dataTag in xdoc.Root.Elements("data"))
            {
                TranslationFileValue tData = new TranslationFileValue
                {
                    ReferenceGroup = group,
                    ReferenceCode = (string)dataTag.Attribute("name"),
                    Description = (string)dataTag.Element("comment"),
                    Translation = (string)dataTag.Element("value")
                };
                yield return tData;
            }
        }

        /// <summary>
        /// Read the translations from a XmlDoc file
        /// </summary>
        public static IEnumerable<TranslationFileValue> ReadXmlDocFile(String file)
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
                    using (var rdr = element.CreateReader())
                    {
                        rdr.MoveToContent();
                        transValue = rdr.ReadInnerXml();
                    }
                    TranslationFileValue tData = new TranslationFileValue
                    {
                        ReferenceGroup = category,
                        ReferenceCode = refObj,
                        Translation = transValue
                    };
                    yield return tData;
                }
            }
        }

        /// <summary>
        /// Read the translations from a translation file
        /// </summary>
        public static IEnumerable<TranslationFileValue> ReadFile(TranslationFile file, String group)
        {
            if (file == null) return null;
            if (String.Equals("dll", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadExecutableFile(group ?? file.FileName, file.File);
            if (String.Equals("exe", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadExecutableFile(group ?? file.FileName, file.File);
            if (String.Equals("resx", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadResourceFile(group ?? file.FileName, file.File);
            if (String.Equals("xml", file.FileType, StringComparison.OrdinalIgnoreCase))
                return ReadXmlDocFile(file.File);
            throw new ArgumentException(Locales.SR.Error_CantReadFileType, file.FileType);
        }

    }
}
