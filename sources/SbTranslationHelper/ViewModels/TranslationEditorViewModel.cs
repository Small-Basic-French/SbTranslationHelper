using System;
using System.Collections.Generic;
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
            this.File = file;
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
                // TODO Load data
                await Task.Delay(100);
            }
            finally
            {
                IsBusy = false;
            }
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

    }

}
