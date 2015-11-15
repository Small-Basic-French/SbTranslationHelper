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

        /// <summary>
        /// Display a dialog to confirm with button Yes and No
        /// </summary>
        /// <param name="message">Message of the confirmation</param>
        /// <param name="title">Title of the dialog</param>
        /// <returns>True if Yes is selected, and False if No is selected.</returns>
        Task<bool> ConfirmYesNoAsync(String message, String title);

        /// <summary>
        /// Display a dialog to confirm with button Yes, No and Cancel
        /// </summary>
        /// <param name="message">Message of the confirmation</param>
        /// <param name="title">Title of the dialog</param>
        /// <returns>Null if Cancel selected, True if Yes is selected, and False if No is selected.</returns>
        Task<bool?> ConfirmYesNoCancelAsync(String message, String title);

        /// <summary>
        /// Display an exception message
        /// </summary>
        Task ShowError(Exception ex, String title = null);
    }

}
