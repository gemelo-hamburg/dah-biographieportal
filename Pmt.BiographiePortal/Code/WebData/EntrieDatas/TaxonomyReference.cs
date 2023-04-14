using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.EntrieDatas
{
    public class TaxonomyReference
    {
        public string id { get; set; }=string.Empty;
        public List<TermReference> terms { get; set; } = new();     
    }
}
