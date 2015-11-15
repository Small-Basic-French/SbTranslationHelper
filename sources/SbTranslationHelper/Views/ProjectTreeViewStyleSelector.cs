using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SbTranslationHelper.Views
{
    public class ProjectTreeViewStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ViewModels.GroupViewModel)
                return GroupStyle;
            if (item is ViewModels.TranslationFileViewModel)
                return FileStyle;
            return base.SelectStyle(item, container);
        }
        public Style GroupStyle { get; set; }
        public Style FileStyle { get; set; }
    }
}
