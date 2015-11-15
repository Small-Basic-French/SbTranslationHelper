using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    public static class TranslationsHelper
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

        static void BackupFile(String file)
        {
            if (!File.Exists(file)) return;
            String backup = file + ".bak";
            if (File.Exists(backup))
                File.Delete(backup);
            File.Move(file, backup);
        }

        /// <summary>
        /// Write the translations in a resource file
        /// </summary>
        public static void WriteResourceFile(TranslationFile file, IEnumerable<TranslationFileValue> translations)
        {
            XDocument xdoc = XDocument.Parse(Properties.Resources.BaseXmlResourceFile);
            foreach (var trans in translations)
            {
                var xTrans = new XElement(
                    "data",
                    new XAttribute("name", trans.ReferenceCode),
                    new XAttribute(XNamespace.Xml + "space", "preserve"),
                    new XElement("value", trans.Translation)
                    );
                if (!String.IsNullOrWhiteSpace(trans.Description))
                    xTrans.Add(new XElement("comment", trans.Description));
                xdoc.Root.Add(xTrans);
            }
            xdoc.Save(file.File);
        }

        /// <summary>
        /// Write the translations in a XmlDoc file
        /// </summary>
        public static void WriteXmlDocFile(TranslationFile file, IEnumerable<TranslationFileValue> translations)
        {
            XElement members = new XElement("members");
            XDocument xdoc = new XDocument(
                new XDeclaration("1.0", "UTF-8", null),
                new XElement("doc",
                    new XElement("assembly", new XElement("name", file.FileName)),
                    members
                )
                );
            foreach (var gTranslations in translations.GroupBy(m=>m.ReferenceGroup))
            {
                XElement member = new XElement("member", new XAttribute("name", gTranslations.Key));
                foreach (var trans in gTranslations)
                {
                    XElement xtrans;
                    var parts = trans.ReferenceCode.Split('@');
                    try
                    {
                        xtrans = XElement.Parse(String.Format("<{0}>{1}</{0}>", parts[0], trans.Translation));
                    }
                    catch {
                        xtrans = new XElement(parts[0], trans.Translation);
                    }
                    for (int i = 1; i < parts.Length; i++)
                    {
                        var aparts = parts[1].Split(new char[] { ':' }, 2);
                        xtrans.Add(new XAttribute(aparts[0], aparts[1]));
                    }
                    member.Add(xtrans);
                }
                members.Add(member);
            }
            xdoc.Save(file.File);
        }

        /// <summary>
        /// Write the translation in a file
        /// </summary>
        public static bool WriteFile(TranslationFile file, String group, IEnumerable<TranslationFileValue> values, bool backupExistingFile)
        {
            if (file == null) return false;
            if (String.Equals("dll", file.FileType, StringComparison.OrdinalIgnoreCase))
                return false;
            if (String.Equals("exe", file.FileType, StringComparison.OrdinalIgnoreCase))
                return false;
            if (String.Equals("resx", file.FileType, StringComparison.OrdinalIgnoreCase))
            {
                if (backupExistingFile) BackupFile(file.File);
                WriteResourceFile(file, values);
                return true;
            }
            if (String.Equals("xml", file.FileType, StringComparison.OrdinalIgnoreCase))
            {
                if (backupExistingFile) BackupFile(file.File);
                WriteXmlDocFile(file, values);
                return true;
            }
            throw new ArgumentException(Locales.SR.Error_CantWriteFileType, file.FileType);
        }

    }
}
