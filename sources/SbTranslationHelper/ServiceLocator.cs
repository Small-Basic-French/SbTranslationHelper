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
            AppService = new Services.AppService();

            AppViewModel = new ViewModels.AppViewModel(AppService);
        }

        /// <summary>
        /// Application service
        /// </summary>
        public Services.IAppService AppService { get; private set; }

        /// <summary>
        /// Global AppViewModel
        /// </summary>
        public ViewModels.AppViewModel AppViewModel { get; private set; }

    }
}
