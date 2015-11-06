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
        /// Load the data from the current folder
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Load()
        {
            if (!Directory.Exists(SmallBasicFolder))
                return false;
            if (Loading) return false;
            try {
                Loading = true;
                // TODO To remove
                await Task.Delay(2000);
                return true;
            }
            finally
            {
                Loading = false;
            }
        }

        /// <summary>
        /// Indicates if the model is loading data
        /// </summary>
        public bool Loading
        {
            get { return _Loading; }
            private set { SetProperty(ref _Loading, value, () => Loading); }
        }
        private bool _Loading = false;

        /// <summary>
        /// Folder where Small Basic is find
        /// </summary>
        public String SmallBasicFolder
        {
            get { return _SmallBasicFolder; }
            private set { SetProperty(ref _SmallBasicFolder, value, () => SmallBasicFolder); }
        }
        private String _SmallBasicFolder;

    }

}
