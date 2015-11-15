using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels.DesignTime
{
    public class DtProjectViewModel : ProjectViewModel
    {
        public DtProjectViewModel() : base(null)
        {
            this.Groups.Add(new GroupViewModel(this, "SmallBasic")
            {
                Caption = "Small Basic"
            });
            this.Groups.Add(new GroupViewModel(this, "SmallBasicLibrary")
            {
                Caption = "Small Basic Library"
            });
            this.Groups.Add(new GroupViewModel(this, "Extension1")
            {
                Caption = "Extension 1"
            });
            this.Groups.Add(new GroupViewModel(this, "Extension2")
            {
                Caption = "Extension 2"
            });
            this.Groups.Add(new GroupViewModel(this, "Extension3")
            {
                Caption = "Extension 3"
            });
            this.Groups.Add(new GroupViewModel(this, "Extension4")
            {
                Caption = "Extension 4"
            });
            foreach (var grp in this.Groups)
            {
                grp.Files.Add(new TranslationFileViewModel(grp, new Model.TranslationFile("C:\\folder\\" + grp.Name + ".xml")));
                grp.Files.Add(new TranslationFileViewModel(grp, new Model.TranslationFile("C:\\folder\\" + grp.Name + ".fr.xml")));
                grp.Files.Add(new TranslationFileViewModel(grp, new Model.TranslationFile("C:\\folder\\" + grp.Name + ".it.xml")));
                grp.Files.Add(new TranslationFileViewModel(grp, new Model.TranslationFile("C:\\folder\\" + grp.Name + ".ex.xml")));
            }

            this.Editors.Add(new TranslationEditorViewModel(Groups[0].Files[1])
            {

            });
            this.Editors.Add(new TranslationEditorViewModel(Groups[1].Files[2])
            {

            });
        }

    }
}
