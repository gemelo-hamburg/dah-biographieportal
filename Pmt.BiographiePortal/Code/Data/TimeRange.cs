using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class TimeRange : BasePartialData
    {
        public static Color StaticTypeColor => (Color)ColorConverter.ConvertFromString("#FF9B9B9B");
        public override Color TypeColor => StaticTypeColor;

        #region Ctor

        public TimeRange(string id)
            : base(id)
        {

        }

        #endregion Ctor
    }
}
