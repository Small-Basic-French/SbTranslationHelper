using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper
{
    /// <summary>
    /// Service locator
    /// </summary>
    public class ServiceLocator
    {

        /// <summary>
        /// Create the service locator
        /// </summary>
        public ServiceLocator()
        {
            AppViewModel = new ViewModels.AppViewModel();
        }

        /// <summary>
        /// Global AppViewModel
        /// </summary>
        public ViewModels.AppViewModel AppViewModel { get; private set; }
    }
}
