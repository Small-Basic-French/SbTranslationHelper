using System;
using System.Collections.Generic;
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

    }

}
