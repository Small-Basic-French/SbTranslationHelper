﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SbTranslationHelper.ViewModels
{

    /// <summary>
    /// ViewModel for the application
    /// </summary>
    public class AppViewModel : ObservableObject
    {

        Services.IAppService _AppService;

        /// <summary>
        /// Create a new ViewModel
        /// </summary>
        public AppViewModel(Services.IAppService appService)
        {
            _AppService = appService;

            OpenProjectFolderCommand = new RelayCommand(
                async () => await OpenProjectAsync(),
                () => !Loading && !ProjectFolderOpened
                );
            CloseProjectFolderCommand = new RelayCommand(
                async () => await CloseProjectAsync(),
                () => !Loading && ProjectFolderOpened
                );
        }

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
            try
            {
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
        /// Open the project
        /// </summary>
        public async Task<bool> OpenProjectAsync()
        {
            // Open the selecteur
            var newFolder = await _AppService.OpenFolderBrowserAsync(Locales.SR.FolderBrowser_OpenProjectFolder_Title);
            if (String.IsNullOrWhiteSpace(newFolder))
                return false;
            // Close the current folder
            if (ProjectFolderOpened)
                await CloseProjectAsync();
            // Load
            ProjectFolder = newFolder;
            await Load();
            return true;
        }

        /// <summary>
        /// Close the current project folder
        /// </summary>
        public Task<bool> CloseProjectAsync()
        {
            if (!ProjectFolderOpened) return Task.FromResult(false);
            // TODO Save if the folder is dirty
            ProjectFolder = null;
            return Task.FromResult(true);
        }

        /// <summary>
        /// Indicates if the model is loading data
        /// </summary>
        public bool Loading
        {
            get { return _Loading; }
            private set {
                if(SetProperty(ref _Loading, value, () => Loading))
                {
                    OpenProjectFolderCommand.RaiseCanExecuteChanged();
                    CloseProjectFolderCommand.RaiseCanExecuteChanged();
                }
            }
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

        /// <summary>
        /// Folder for the project
        /// </summary>
        public String ProjectFolder
        {
            get { return _ProjectFolder; }
            private set {
                if (SetProperty(ref _ProjectFolder, value, () => ProjectFolder))
                    RaisePropertyChanged(() => ProjectFolderOpened);
            }
        }
        private String _ProjectFolder;

        /// <summary>
        /// Is the project folder opened
        /// </summary>
        public bool ProjectFolderOpened { get { return !String.IsNullOrWhiteSpace(ProjectFolder); } }

        /// <summary>
        /// Command to open the project folder
        /// </summary>
        public RelayCommand OpenProjectFolderCommand { get; private set; }

        /// <summary>
        /// Command to close the project folder
        /// </summary>
        public RelayCommand CloseProjectFolderCommand { get; private set; }
    }

}
