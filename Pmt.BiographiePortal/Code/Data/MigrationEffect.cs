using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class MigrationEffect : BasePartialData
    {
        public static Color StaticTypeColor => (Color)ColorConverter.ConvertFromString("#FF566C7B");
        public override Color TypeColor => StaticTypeColor;

        #region Ctor

        public MigrationEffect(string id)
            : base(id)
        {

        }

        #endregion Ctor
    }
}
