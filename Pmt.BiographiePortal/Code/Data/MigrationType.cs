using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class MigrationType : BasePartialData
    {

        public static Color StaticTypeColor => (Color)ColorConverter.ConvertFromString("#FFA33749");
        public override Color TypeColor => StaticTypeColor;

        #region Ctor

        public MigrationType(string id)
            : base(id)
        {

        }

        #endregion Ctor
    }
}
