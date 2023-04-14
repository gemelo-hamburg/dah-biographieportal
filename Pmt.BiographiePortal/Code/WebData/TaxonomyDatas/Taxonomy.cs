using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.TaxonomyDatas
{
    public class Taxonomy
    {
        #region öffentliche Eigenschaften

        public string id { get; set; } = string.Empty;
        public LocalizationString title { get; set; } = new();
        public Slug slug { get; set; } = new();
        public Meta meta { get; set; } = new();
        public List<Term> terms { get; set; }

        #endregion öffentliche Eigenschaften

        #region ctors

        public Taxonomy(string id, string titleDe, string titleEn, Color color)
        {
            terms = new List<Term>();
            this.id = id;
            title = new LocalizationString(titleDe, titleEn);
            slug = new Slug(titleDe, titleEn);
            meta = new Meta { color = ToStringWithOutAlpha(color) };
        }


        #endregion ctors

        #region öffentliche Methoden

        public static string ToStringWithOutAlpha(Color color)
        {
            return $"#{color.R:X}{color.G:X}{color.B:X}";
        }

        public Term GetTaxonomyTerm(string id)
        {
            foreach (Term term in terms)
            {
                if (term.id == id) return term;
            }
            return null;
        }

        public void AddTerm(Term t)
        {
            terms.Add(t);
            t.Taxonomy = this;
        }

        #endregion öffentliche Methoden

    }

}
