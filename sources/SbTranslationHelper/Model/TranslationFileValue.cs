using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbTranslationHelper.Model
{
    /// <summary>
    /// Value of a translation content
    /// </summary>
    public class TranslationFileValue
    {
        /// <summary>
        /// Reference group
        /// </summary>
        public string ReferenceGroup { get; set; }
        /// <summary>
        /// Reference code
        /// </summary>
        public string ReferenceCode { get; set; }
        /// <summary>
        /// Reference key
        /// </summary>
        public string ReferenceKey { get { return string.Format("{0}.{1}", ReferenceGroup, ReferenceCode); } }
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Translation
        /// </summary>
        public string Translation { get; set; }
    }
}
