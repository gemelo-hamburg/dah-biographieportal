using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class MigrationReason : BasePartialData
    {
        public static Color StaticTypeColor => (Color)ColorConverter.ConvertFromString("#FFBD832A");
        public override Color TypeColor => StaticTypeColor;

        #region Ctor

        public MigrationReason(string id)
            : base(id)
        {

        }

        #endregion Ctor
    }
}
