using Gemelo.Applications.Biographieportal.Code;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common
{
    public class LocalizationString
    {

        public LocalizationString(BiographyLocalizationString text)
            : this(text.De, text.En)
        {
        }

        public LocalizationString(string de, string en)
            : this()
        {
            this.de = de;
            this.en = en;
        }

        public LocalizationString()
        {
            de = string.Empty;
            en = string.Empty;
        }

        public string de { get; set; }
        public string en { get; set; }


        public override string ToString()
        {
            return $"String:{de}|{en}";
        }
    }
}
