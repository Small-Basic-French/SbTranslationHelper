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
        /// <summary>
        /// Create a new project
        /// </summary>
        public ProjectViewModel()
        {
            Project = new Model.TranslationProject();
            Groups = new ObservableCollection<GroupViewModel>();
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
                    vmGrp = new GroupViewModel(group.Name);
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
        /// List of the groups
        /// </summary>
        public ObservableCollection<GroupViewModel> Groups { get; private set; }

    }

}
