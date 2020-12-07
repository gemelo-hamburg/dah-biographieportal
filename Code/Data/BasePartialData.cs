using Pete.Components.Extensions.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class BasePartialData
    {
        #region öffentliche Properties

        public string Id { get; private set; }
        public BiographyLocalizationString Text { get; private set; }
        public virtual Color TypeColor { get; } = Colors.Pink;
        public bool NewLine { get;  set; }

        #endregion öffentliche Properties

        public BasePartialData(string id)
        {
            Id = id.ToLower().Trim();
            Text = new BiographyLocalizationString();
        }
    }
}
