using SbTranslationHelper.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels
{

    /// <summary>
    /// Project
    /// </summary>
    public class ProjectViewModel : ObservableObject
    {
        IAppService _AppService;

        /// <summary>
        /// Create a new project
        /// </summary>
        public ProjectViewModel(IAppService appService)
        {
            this._AppService = appService;
            Project = new Model.TranslationProject();
            Groups = new ObservableCollection<GroupViewModel>();
            Editors = new ObservableCollection<TranslationEditorViewModel>();
            OpenTranslationCommand = new RelayCommand<TranslationFileViewModel>(
                async file => {
                    Exception error = null;
                    try
                    {
                        await OpenTranslation(file);
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                    if (error != null)
                        await _AppService.ShowError(error);
                },
                file => file != null
                );
            CloseEditorCommand = new RelayCommand<TranslationEditorViewModel>(
                editor => CloseEditor(editor),
                editor => editor != null
                );
        }

        /// <summary>
        /// Update the data in the viewmodels
        /// </summary>
        void UpdateProjectData()
        {
            var vmGrps = Groups.ToList();
            foreach (var group in Project.Groups)
            {
                var vmGrp = vmGrps.FirstOrDefault(g => String.Equals(g.Name, group.Name, StringComparison.OrdinalIgnoreCase));
                if (vmGrp == null)
                {
                    vmGrp = new GroupViewModel(this, group.Name);
                    Groups.Add(vmGrp);
                }
                vmGrp.Caption = group.Caption;
                vmGrp.UpdateFiles(group.Files);
                vmGrps.Remove(vmGrp);
            }
            // Remove the viewmodel groups not found in the project data
            foreach (var grp in vmGrps)
                Groups.Remove(grp);
        }

        private async Task InternalLoadProjectASync(string folder)
        {
            await Task.Run(() => Project.ScanFolder(folder));
            ProjectFolder = folder;
            UpdateProjectData();
        }

        /// <summary>
        /// Load the project
        /// </summary>
        public async Task<bool> LoadProjectAsync(String folder)
        {
            if (Loading) return false;
            try
            {
                Loading = true;
                await InternalLoadProjectASync(folder);
            }
            finally
            {
                Loading = false;
            }
            return true;
        }

        async Task CopyFileAsync(bool overrideFiles, DirectoryInfo iDir, Model.TranslationFile iFile)
        {
            String siFilePath = iFile.File.Substring(iDir.FullName.Length).Trim('\\', '/');
            String diFilePath = Path.Combine(ProjectFolder, siFilePath);
            String diFolderPath = Path.GetDirectoryName(diFilePath);
            if (!Directory.Exists(diFolderPath))
                Directory.CreateDirectory(diFolderPath);
            if (!File.Exists(diFilePath) || overrideFiles)
            {
                if (File.Exists(diFilePath)) File.Delete(diFilePath);
                using (var sStr = File.OpenRead(iFile.File))
                using (var dStr = File.Create(diFilePath))
                    await sStr.CopyToAsync(dStr);
            }
        }

        /// <summary>
        /// Import the files to the project folder from an another folder
        /// </summary>
        public async Task<bool> ImportFolderAsync(String folder, bool overrideFiles)
        {
            if (String.IsNullOrWhiteSpace(ProjectFolder)) return false;
            if (Loading) return false;
            try
            {
                Loading = true;
                DirectoryInfo iDir = new DirectoryInfo(folder);
                var iProject = new Model.TranslationProject();
                iProject.ScanFolder(folder);
                foreach (var iFile in iProject.Groups.SelectMany(g => g.Files))
                {
                    // If neutral XML find the DLL
                    if (iFile.IsNeutral && String.Equals(iFile.FileType, "xml", StringComparison.OrdinalIgnoreCase))
                    {
                        var dllFile = new Model.TranslationFile(iFile.Folder, iFile.FileName, "dll");
                        if (!iProject.Groups.Any(g => g.FindFile(dllFile.File) != null))
                            await CopyFileAsync(overrideFiles, iDir, dllFile);
                    }
                    await CopyFileAsync(overrideFiles, iDir, iFile);
                }
                Project.Groups.Clear();
                await InternalLoadProjectASync(ProjectFolder);
                return true;
            }
            finally
            {
                Loading = false;
            }
        }

        /// <summary>
        /// Open a translation
        /// </summary>
        public async Task<TranslationEditorViewModel> OpenTranslation(TranslationFileViewModel file)
        {
            if (file == null) return null;
            TranslationEditorViewModel trans = Editors.FirstOrDefault(e => e.File == file);
            if (trans == null)
            {
                if (Loading) return null;
                Loading = true;
                try
                {
                    trans = new TranslationEditorViewModel(file);
                    Editors.Add(trans);
                    await trans.Load();
                }
                finally
                {
                    Loading = false;
                }
            }
            CurrentEditor = trans;
            return trans;
        }

        /// <summary>
        /// Close an editor
        /// </summary>
        public void CloseEditor(TranslationEditorViewModel editor)
        {
            var idx = Editors.IndexOf(editor);
            if (idx < 0) return;
            Editors.Remove(editor);
            idx = Math.Min(idx, Editors.Count - 1);
            CurrentEditor = idx >= 0 ? Editors[idx] : null;
        }

        /// <summary>
        /// Data
        /// </summary>
        public Model.TranslationProject Project { get; private set; }

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
        /// Folder for the project
        /// </summary>
        public String ProjectFolder
        {
            get { return _ProjectFolder; }
            private set { SetProperty(ref _ProjectFolder, value, () => ProjectFolder); }
        }
        private String _ProjectFolder;

        /// <summary>
        /// Current editor
        /// </summary>
        public TranslationEditorViewModel CurrentEditor
        {
            get { return _CurrentEditor; }
            set { SetProperty(ref _CurrentEditor, value, () => CurrentEditor); }
        }
        private TranslationEditorViewModel _CurrentEditor;

        /// <summary>
        /// List of the groups
        /// </summary>
        public ObservableCollection<GroupViewModel> Groups { get; private set; }

        /// <summary>
        /// List of the opened editors
        /// </summary>
        public ObservableCollection<TranslationEditorViewModel> Editors { get; private set; }

        /// <summary>
        /// Command to open a translation file
        /// </summary>
        public RelayCommand<TranslationFileViewModel> OpenTranslationCommand { get; private set; }

        /// <summary>
        /// Command to close an editor
        /// </summary>
        public RelayCommand<TranslationEditorViewModel> CloseEditorCommand { get; private set; }

    }

}
