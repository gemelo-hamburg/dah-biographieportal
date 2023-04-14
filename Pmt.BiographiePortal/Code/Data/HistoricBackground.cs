using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
   public class HistoricBackground : BasePartialData
    {
        public static Color StaticTypeColor => (Color)ColorConverter.ConvertFromString("#FF9B9B9B");
        public override Color TypeColor => StaticTypeColor;

        #region öffentliche Properties

        public BiographyLocalizationString Headline { get; set; }

        #endregion öffentliche Properties

        #region Ctor

        public HistoricBackground(string id)
            : base(id)
        {
            Headline = new BiographyLocalizationString();
        }

        #endregion Ctor
    }
}
