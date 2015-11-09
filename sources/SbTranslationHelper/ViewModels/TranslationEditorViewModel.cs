using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels
{

    /// <summary>
    /// Viewmodel for editing a translation
    /// </summary>
    public class TranslationEditorViewModel : ObservableObject
    {
        /// <summary>
        /// Create a new model
        /// </summary>
        /// <param name="file"></param>
        public TranslationEditorViewModel(TranslationFileViewModel file)
        {
            this.Translations = new ObservableCollection<TranslationViewModel>();
            this.File = file;
            MoveToPreviousTranslationCommand = new RelayCommand(
                () => MoveToPreviousTranslation(),
                () =>
                {
                    int idx = Translations.IndexOf(CurrentTranslation);
                    return idx > 0;
                }
                );
            MoveToNextTranslationCommand = new RelayCommand(
                () => MoveToNextTranslation(),
                () =>
                {
                    int idx = Translations.IndexOf(CurrentTranslation);
                    return idx >= 0 && idx < Translations.Count - 1;
                }
                );
        }

        /// <summary>
        /// Load the data
        /// </summary>
        public async Task Load()
        {
            if (IsBusy) return;
            IsBusy = true;
            try
            {
                Translations.Clear();
                var grp = this.File.Group.Project.Project.FindGroup(this.File.Group.Name);
                if (grp != null)
                {
                    var translations = await Task.Run(() =>
                    {
                        return grp.ReadFile(this.File.File.File).ToList();
                    });
                    foreach (var trans in translations)
                    {
                        this.Translations.Add(new TranslationViewModel
                        {
                            Editor = this,
                            ReferenceGroup = trans.ReferenceGroup,
                            ReferenceCode = trans.ReferenceCode,
                            Description = trans.Description,
                            NeutralValue = trans.NeutralValue,
                            TranslatedValue = trans.Translation
                        });
                    }
                }
                CurrentTranslation = Translations.FirstOrDefault();
                IsDirty = false;
            }
            finally
            {
                IsBusy = false;
            }
        }

        /// <summary>
        /// Move to the previous translation
        /// </summary>
        public void MoveToPreviousTranslation()
        {
            int idx = Translations.IndexOf(CurrentTranslation);
            if (idx > 0)
                CurrentTranslation = Translations[idx - 1];
        }

        /// <summary>
        /// Move to the next translation
        /// </summary>
        public void MoveToNextTranslation()
        {
            int idx = Translations.IndexOf(CurrentTranslation);
            if (idx >= 0 && idx < Translations.Count - 1)
                CurrentTranslation = Translations[idx + 1];
        }

        /// <summary>
        /// File in edition
        /// </summary>
        public TranslationFileViewModel File { get; private set; }

        /// <summary>
        /// Loading or writing activity
        /// </summary>
        public bool IsBusy
        {
            get { return _IsBusy; }
            private set { SetProperty(ref _IsBusy, value, () => IsBusy); }
        }
        private bool _IsBusy = false;

        /// <summary>
        /// Translations
        /// </summary>
        public ObservableCollection<TranslationViewModel> Translations { get; private set; }

        /// <summary>
        /// Translation currently editing
        /// </summary>
        public TranslationViewModel CurrentTranslation
        {
            get { return _CurrentTranslation; }
            set { SetProperty(ref _CurrentTranslation, value, () => CurrentTranslation); }
        }
        private TranslationViewModel _CurrentTranslation;

        /// <summary>
        /// Indicates if the translations need to be saved
        /// </summary>
        public bool IsDirty
        {
            get { return _IsDirty; }
            internal set { SetProperty(ref _IsDirty, value, () => IsDirty); }
        }
        private bool _IsDirty;

        /// <summary>
        /// Command to move to the previous translation
        /// </summary>
        public RelayCommand MoveToPreviousTranslationCommand { get; private set; }

        /// <summary>
        /// Command to move to the next translation
        /// </summary>
        public RelayCommand MoveToNextTranslationCommand { get; private set; }
    }

}
