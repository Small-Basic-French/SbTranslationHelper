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

        }
    }
}
