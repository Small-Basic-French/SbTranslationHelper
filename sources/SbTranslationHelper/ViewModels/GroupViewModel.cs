using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SbTranslationHelper.Model;
using System.Collections.ObjectModel;

namespace SbTranslationHelper.ViewModels
{
    /// <summary>
    /// ViewModel for a group of files
    /// </summary>
    public class GroupViewModel : ObservableObject
    {
        /// <summary>
        /// Create a new group
        /// </summary>
        public GroupViewModel(String name)
        {
            this.Name = name;
            Files = new ObservableCollection<TranslationFileViewModel>();
        }

        internal void UpdateFiles(IEnumerable<TranslationFile> files)
        {
            var currentFiles = Files.ToList();
            foreach (var file in files)
            {
                var cFile = currentFiles.FirstOrDefault(f => String.Equals(file.File, f.File.File, StringComparison.OrdinalIgnoreCase));
                if(cFile== null)
                {
                    cFile = new TranslationFileViewModel(file);
                    this.Files.Add(cFile);
                }
                currentFiles.Remove(cFile);
            }
            NeutralFile = Files.FirstOrDefault(f => f.File.IsNeutral);
        }

        /// <summary>
        /// Name
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Caption of the group
        /// </summary>
        public String Caption
        {
            get { return _Caption; }
            set { SetProperty(ref _Caption, value, () => Caption); }
        }
        private String _Caption;

        /// <summary>
        /// File for the neutral file
        /// </summary>
        public TranslationFileViewModel NeutralFile
        {
            get { return _NeutralFile; }
            private set { SetProperty(ref _NeutralFile, value, () => NeutralFile); }
        }
        private TranslationFileViewModel _NeutralFile;

        /// <summary>
        /// List of files availables
        /// </summary>
        public ObservableCollection<TranslationFileViewModel> Files { get; private set; }

    }
}
