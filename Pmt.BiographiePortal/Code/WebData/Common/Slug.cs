using Gemelo.Applications.Biographieportal.Code;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common
{
    public class Slug
    {
        public string de { get; set; } = string.Empty;
        public string en { get; set; } = string.Empty;

        public Slug(string de, string en)
            : this()
        {
            this.de = Normalize(de);
            this.en = Normalize(en);
        }

        public Slug(BiographyLocalizationString text)
            : this(text.De, text.En)
        {
        }

        public Slug()
        {

        }

        public static string Normalize(string t)
        {
            //if (t.Contains("Ausreise"))
            //{
            //    Debugger.Break();
            //}

            t = t.Trim();

            string result = t.ToLower()
                .Replace("   ", " ")
                .Replace("  ", " ")
                .Replace(":", " ")
                .Replace("ä", "ae")
                .Replace("ü", "ue")
                .Replace("ö", "oe")
                .Replace("Ä", "Ae")
                .Replace("Ü", "Ue")
                .Replace("Ö", "Oe")
                .Replace("ß", "ss")
                .Replace(".", " ")
                .Replace("_", " ")
                .Replace(",", " ")
                .Replace("(", " ")
                .Replace(")", " ")
                .Trim();

            string[] ts= result.Split(new string[] {" "}, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb=new StringBuilder();   
            foreach(string s in ts)
            {
                sb.Append(s);
                sb.Append("-");
            }
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();

            //string result2 = result
            //    .Replace(" ", "-")
            //    .Replace("---–--", "-")
            //    .Replace("--–--", "-")
            //    .Replace("-–--", "-")
            //    .Replace("-–-", "-")
            //    .Replace("-–", "-")
            //    .Replace("-–", "-")
            //    .Replace("-–", "-")
            //    .Replace("-–", "-")
            //    .Replace("–", "-");

            //string result3 = result2
            //    .Replace("-", " ");

            //return result3;
        }

        public override string ToString()
        {
            return $"Slug:{de}|{en}";
        }
    }

}
