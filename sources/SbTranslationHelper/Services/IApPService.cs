using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.Services
{

    /// <summary>
    /// Application service
    /// </summary>
    public interface IAppService
    {
        /// <summary>
        /// Open the folder browser
        /// </summary>
        /// <param name="title">Title of the browser</param>
        /// <returns>The selected folder or null if canceled</returns>
        Task<String> OpenFolderBrowserAsync(String title);
    }

}
