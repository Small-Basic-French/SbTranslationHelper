using System;
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
                () => !Loading && !ProjectOpened
                );
            CloseProjectFolderCommand = new RelayCommand(
                async () => await CloseProjectAsync(),
                () => !Loading && ProjectOpened
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
                Project = new ProjectViewModel();
                await Project.LoadProject(ProjectFolder);
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
            if (ProjectOpened)
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
            if (!ProjectOpened) return Task.FromResult(false);
            // TODO Save if the project is dirty
            Project = null;
            ProjectFolder = null;
            return Task.FromResult(true);
        }

        /// <summary>
        /// Indicates if the model is loading data
        /// </summary>
        public bool Loading
        {
            get { return _Loading || (Project != null && Project.Loading); }
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
            private set { SetProperty(ref _ProjectFolder, value, () => ProjectFolder); }
        }
        private String _ProjectFolder;

        /// <summary>
        /// Project currently open
        /// </summary>
        public ProjectViewModel Project
        {
            get { return _Project; }
            private set
            {
                var op = _Project;
                if (SetProperty(ref _Project, value, () => Project))
                {
                    if (op != null)
                        op.PropertyChanged += Project_PropertyChanged;
                    if (_Project != null)
                        _Project.PropertyChanged += Project_PropertyChanged;
                    RaisePropertyChanged(() => ProjectOpened);
                }
            }
        }

        private void Project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (String.Equals(e.PropertyName, "Loading", StringComparison.OrdinalIgnoreCase))
            {
                RaisePropertyChanged(() => Loading);
                OpenProjectFolderCommand.RaiseCanExecuteChanged();
                CloseProjectFolderCommand.RaiseCanExecuteChanged();
            }
        }

        private ProjectViewModel _Project;

        /// <summary>
        /// Is the project opened
        /// </summary>
        public bool ProjectOpened { get { return Project != null; } }

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
