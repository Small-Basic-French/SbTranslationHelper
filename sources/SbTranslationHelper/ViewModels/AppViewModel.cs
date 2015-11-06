using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels
{

    /// <summary>
    /// ViewModel for the application
    /// </summary>
    public class AppViewModel : ObservableObject
    {

        /// <summary>
        /// Initialisation
        /// </summary>
        public void Initialize()
        {
            // Find the current Smal Basic folder
            String pf =
                Environment.Is64BitOperatingSystem
                ? Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86)
                : Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            String folder = Path.Combine(pf, "Microsoft", "Small Basic");
            if (Directory.Exists(folder))
                SmallBasicFolder = folder;
            else
                SmallBasicFolder = String.Empty;
        }

        /// <summary>
        /// Folder where Small Basic is find
        /// </summary>
        public String SmallBasicFolder
        {
            get { return _SmallBasicFolder; }
            set { SetProperty(ref _SmallBasicFolder, value, () => SmallBasicFolder); }
        }
        private String _SmallBasicFolder;

    }

}
