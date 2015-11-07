using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.Model
{

    /// <summary>
    /// Data for translations
    /// </summary>
    public class TranslationProject
    {

        /// <summary>
        /// Create a new project
        /// </summary>
        public TranslationProject()
        {
            Groups = new List<TranslationGroupFiles>();
        }

        /// <summary>
        /// Find or create a group
        /// </summary>
        TranslationGroupFiles FindOrCreateGroup(String name, String caption)
        {
            var group = FindGroup(name);
            if (group == null)
            {
                group = new TranslationGroupFiles
                {
                    Name = name,
                    Caption = caption
                };
                Groups.Add(group);
            }
            return group;
        }

        /// <summary>
        /// Scan for Small Basic translations
        /// </summary>
        bool ScanSmallBasicFolder(String folder)
        {
            if (!Directory.Exists(folder)) return false;
            // Search StringResources.dll and Strings.*.resx
            var files = Directory.GetFiles(folder, "Strings.*.resx", SearchOption.TopDirectoryOnly);
            String srFileName = Path.Combine(folder, "StringResources.dll");
            if (files.Length == 0 || !File.Exists(srFileName))
                return false;
            // Find or create the Small Basic group
            var group = FindOrCreateGroup("SmallBasic", Locales.SR.TransGroup_SmallBasic_Caption);
            // Add files
            group.AddFile(srFileName);
            foreach (var file in files)
                group.AddFile(file);
            return true;
        }

        /// <summary>
        /// Scan for Small Basic Library translations
        /// </summary>
        bool ScanSmallBasicLibraryFolder(String folder)
        {
            if (!Directory.Exists(folder)) return false;
            // Search SmallBasicLibrary*.xml
            var files = Directory.GetFiles(folder, "SmallBasicLibrary*.xml", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return false;
            // Find or create the Small Basic Library group
            var group = FindOrCreateGroup("SmallBasicLibrary", Locales.SR.TransGroup_SmallBasicLibrary_Caption);
            // Add files
            foreach (var file in files)
                group.AddFile(file);
            return true;
        }

        /// <summary>
        /// Scan for extensions translations
        /// </summary>
        int ScanForExtensionsInFolder(String folder)
        {
            int result = 0;
            if (!Directory.Exists(folder)) return result;
            // Search all .xml files with a .dll 
            foreach (var dllFile in Directory.GetFiles(folder, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var dllTf = new TranslationFile(dllFile);
                // Exclude known DLL
                if (String.Equals("SmallBasicLibrary", dllTf.FileName, StringComparison.OrdinalIgnoreCase))
                    continue;
                // Search XML files for this DLL
                var files = Directory.GetFiles(folder, dllTf.FileName + "*.xml", SearchOption.TopDirectoryOnly);
                if (files.Length == 0) continue;
                // Find or create the extension group
                var group = FindOrCreateGroup(dllTf.FileName, String.Format(Locales.SR.TransGroup_Extension_Caption, dllTf.FileName));
                // Add files
                foreach (var file in files)
                    group.AddFile(file);
                result++;
            }
            return result;
        }

        /// <summary>
        /// Load groups from a folder
        /// </summary>
        public void ScanFolder(String folder)
        {
            if (!Directory.Exists(folder)) return;
            // Scan for Small Basic groups
            ScanSmallBasicFolder(folder);
            ScanSmallBasicLibraryFolder(folder);
            // Scan for extensions in a Small Basic folder
            ScanForExtensionsInFolder(Path.Combine(folder, "lib"));
            // Scan for other cases
            ScanForExtensionsInFolder(folder);
        }

        /// <summary>
        /// Find a group
        /// </summary>
        public TranslationGroupFiles FindGroup(String name)
        {
            return Groups.FirstOrDefault(g => String.Equals(name, g.Name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// List of the groups
        /// </summary>
        public List<TranslationGroupFiles> Groups { get; private set; }

    }

}
