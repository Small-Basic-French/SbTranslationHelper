using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels.DesignTime
{
    public class DtProjectViewModel : ProjectViewModel
    {
        public DtProjectViewModel()
        {
            this.Groups.Add(new GroupViewModel("SmallBasic")
            {
                Caption = "Small Basic"
            });
            this.Groups.Add(new GroupViewModel("SmallBasicLibrary")
            {
                Caption = "Small Basic Library"
            });
            this.Groups.Add(new GroupViewModel("Extension1")
            {
                Caption = "Extension 1"
            });
            this.Groups.Add(new GroupViewModel("Extension2")
            {
                Caption = "Extension 2"
            });
            this.Groups.Add(new GroupViewModel("Extension3")
            {
                Caption = "Extension 3"
            });
            this.Groups.Add(new GroupViewModel("Extension4")
            {
                Caption = "Extension 4"
            });
            foreach (var grp in this.Groups)
            {
                grp.Files.Add(new TranslationFileViewModel(new Model.TranslationFile("C:\\folder\\" + grp.Name + ".xml")));
                grp.Files.Add(new TranslationFileViewModel(new Model.TranslationFile("C:\\folder\\" + grp.Name + ".fr.xml")));
                grp.Files.Add(new TranslationFileViewModel(new Model.TranslationFile("C:\\folder\\" + grp.Name + ".it.xml")));
                grp.Files.Add(new TranslationFileViewModel(new Model.TranslationFile("C:\\folder\\" + grp.Name + ".ex.xml")));
            }
        }
    }
}
