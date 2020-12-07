using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.EntrieDatas
{

    public class Preview: IHasFileName
    {
        #region öffentliche Eigenschaften

        public string type { get; set; } = string.Empty;
        public LocalizationString copyright { get; set; } = new();
        public string filename { get; set; } = string.Empty;

        #endregion öffentliche Eigenschaften

        #region ctor

        public Preview()
        {

        }

        public Preview(BiographyPart teaser)
            : this()
        {
            if (Fragment.GetMediaExists(teaser.MediaFile))
            {
                copyright = new LocalizationString(teaser.MediaFile.Copyright);
                type = Fragment.GetWebMediaTypeString(teaser.MediaFile.Type);
                filename = teaser.MediaFile.FileInfo.Name;
            }
        }

        #endregion ctor


    }

}
