using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.ViewModels.DesignTime
{
    public class DtTranslationEditorViewModel : TranslationEditorViewModel
    {
        public DtTranslationEditorViewModel() : base(new DtProjectViewModel().Groups[0].Files[1])
        {
            for (int i = 1; i <= 13; i++)
            {
                Translations.Add(new TranslationViewModel
                {
                    Editor = this,
                    ReferenceGroup = String.Format("Group {0}", (i % 3) + 1),
                    ReferenceCode = String.Format("String {0}", i),
                    Description = String.Format("String number {0}", i),
                    NeutralValue = String.Format("Source {0}", i),
                    TranslatedValue = String.Format("Translation {0}", i)
                });
            }
            CurrentTranslation = Translations[1];
        }
    }
}
