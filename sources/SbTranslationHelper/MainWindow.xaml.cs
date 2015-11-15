using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SbTranslationHelper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Initialize the ViewModel
            ViewModel.Initialize();
        }

        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ViewModel != null && ViewModel.ProjectOpened)
            {
                e.Cancel = !(await ViewModel.CloseProjectAsync());
            }
        }

        /// <summary>
        /// Current ViewModel
        /// </summary>
        public ViewModels.AppViewModel ViewModel { get { return (ViewModels.AppViewModel)DataContext; } }
    }
}
