using SbTranslationHelper.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Load the project
        /// </summary>
        public async Task<bool> LoadProject(String folder)
        {
            if (Loading) return false;
            try
            {
                Loading = true;
                await Task.Run(() => Project.ScanFolder(folder));
                UpdateProjectData();
            }
            finally
            {
                Loading = false;
            }
            return true;
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
    }

}
