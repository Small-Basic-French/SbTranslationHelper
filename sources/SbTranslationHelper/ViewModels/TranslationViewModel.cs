using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels
{

    /// <summary>
    /// Translation
    /// </summary>
    public class TranslationViewModel : ObservableObject
    {

        void RefreshReferenceKey()
        {
            ReferenceKey =  string.Format("{0}.{1}", ReferenceGroup, ReferenceCode);
        }

        /// <summary>
        /// Reference group
        /// </summary>
        public String ReferenceGroup
        {
            get { return _ReferenceGroup; }
            set {
                if (SetProperty(ref _ReferenceGroup, value, () => ReferenceGroup))
                    RefreshReferenceKey();
            }
        }
        private String _ReferenceGroup;

        /// <summary>
        /// Reference code
        /// </summary>
        public String ReferenceCode
        {
            get { return _ReferenceCode; }
            set {
                if (SetProperty(ref _ReferenceCode, value, () => ReferenceCode))
                    RefreshReferenceKey();
            }
        }
        private String _ReferenceCode;

        /// <summary>
        /// Reference key
        /// </summary>
        public String ReferenceKey
        {
            get { return _ReferenceKey; }
            private set { SetProperty(ref _ReferenceKey, value, () => ReferenceKey); }
        }
        private String _ReferenceKey;

        /// <summary>
        /// Description
        /// </summary>
        public String Description
        {
            get { return _Description; }
            set { SetProperty(ref _Description, value, () => Description); }
        }
        private String _Description;

        /// <summary>
        /// Neutral value
        /// </summary>
        public String NeutralValue
        {
            get { return _NeutralValue; }
            set { SetProperty(ref _NeutralValue, value, () => NeutralValue); }
        }
        private String _NeutralValue;

        /// <summary>
        /// Translated value
        /// </summary>
        public String TranslatedValue
        {
            get { return _TranslatedValue; }
            set { SetProperty(ref _TranslatedValue, value, () => TranslatedValue); }
        }
        private String _TranslatedValue;

    }

}
