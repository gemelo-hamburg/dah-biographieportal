using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.TaxonomyDatas
{
    public class Term
    {
        [JsonIgnore]
        public Taxonomy Taxonomy { get; set; }

        public string id { get; set; } = string.Empty;
        public LocalizationString title { get; set; } = new();
        public LocalizationString text { get; set; } = new();
        public Slug slug { get; set; } = new Slug();

        public static Term Create(BasePartialData basePartialData)
        {
            Term term = new Term();

            term.id = basePartialData.Id;

            if (basePartialData is HistoricBackground hb)
            {
                term.text = new LocalizationString(hb.Text);
                term.title = new LocalizationString(hb.Headline);
                term.slug = new Slug(hb.Headline);
            }
            else
            {
                term.title = new LocalizationString(basePartialData.Text);
                term.slug = new Slug(basePartialData.Text);
            }

            return term;
        }
    }

}
