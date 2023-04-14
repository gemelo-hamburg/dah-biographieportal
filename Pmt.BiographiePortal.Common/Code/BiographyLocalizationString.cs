using Gemelo.Components.Common.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Biographieportal.Code
{
    public class BiographyLocalizationString : LocalizationString
    {
        public string De
        {
            get => LocalizedStrings["de"];
            set => LocalizedStrings["de"] = value;
        }
        public string En
        {
            get => LocalizedStrings["en"];
            set => LocalizedStrings["en"] = value;
        }

        public BiographyLocalizationString()
        {
            De = "";
            En = "";
        }

        public void Normalize()
        {
            if (string.IsNullOrWhiteSpace(De)) De = string.Empty;
#if(DEBUG)
            //if (string.IsNullOrWhiteSpace(En)) En = "to be translated:" + De;
            if (string.IsNullOrWhiteSpace(En)) En = De;
#else
            if (string.IsNullOrWhiteSpace(En)) En = De;
#endif
        }
    }
}
