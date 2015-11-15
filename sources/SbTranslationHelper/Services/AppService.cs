using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SbTranslationHelper.Services
{
    /// <summary>
    /// Application service
    /// </summary>
    public class AppService : IAppService
    {

        /// <summary>
        /// Open the folder browser
        /// </summary>
        /// <param name="title">Title of the browser</param>
        /// <returns>The selected folder or null if canceled</returns>
        public Task<String> OpenFolderBrowserAsync(String title)
        {
            var dlg = new Utils.FolderBrowserDialog();
            if (!String.IsNullOrWhiteSpace(title))
                dlg.Title = title;
            dlg.BrowseShares = true;
            dlg.SelectedPath = SbTranslationHelper.Properties.Settings.Default.LastOpenedFolder;
            if (dlg.ShowDialog(App.Current.MainWindow) == true)
            {
                SbTranslationHelper.Properties.Settings.Default.LastOpenedFolder = dlg.SelectedPath;
                SbTranslationHelper.Properties.Settings.Default.Save();
                return Task.FromResult(dlg.SelectedPath);
            }
            return Task.FromResult<String>(null);
        }

        /// <summary>
        /// Display an exception message
        /// </summary>
        public Task ShowError(Exception ex, String title = null)
        {
            MessageBox.Show(ex.GetBaseException().Message, title ?? Locales.SR.ShowError_DefaultTitle);
            return Task.FromResult(true);
        }

        /// <summary>
        /// Display a dialog to confirm with button Yes and No
        /// </summary>
        /// <param name="message">Message of the confirmation</param>
        /// <param name="title">Title of the dialog</param>
        /// <returns>True if Yes is selected, and False if No is selected.</returns>
        public Task<bool> ConfirmYesNoAsync(String message, String title)
        {
            return Task.FromResult(MessageBox.Show(message, title, MessageBoxButton.YesNo) == MessageBoxResult.Yes);
        }

        /// <summary>
        /// Display a dialog to confirm with button Yes, No and Cancel
        /// </summary>
        /// <param name="message">Message of the confirmation</param>
        /// <param name="title">Title of the dialog</param>
        /// <returns>Null if Cancel selected, True if Yes is selected, and False if No is selected.</returns>
        public Task<bool?> ConfirmYesNoCancelAsync(String message, String title)
        {
            var res = MessageBox.Show(message, title, MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Cancel)
                return Task.FromResult<bool?>(null);
            return Task.FromResult<bool?>(res == MessageBoxResult.Yes);
        }

    }
}
