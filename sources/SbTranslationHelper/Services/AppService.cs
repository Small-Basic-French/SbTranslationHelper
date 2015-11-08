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
            if (dlg.ShowDialog(App.Current.MainWindow) == true)
            {
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

    }
}
