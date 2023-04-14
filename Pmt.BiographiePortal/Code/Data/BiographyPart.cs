using Gemelo.Applications.Biographieportal.Code.Enumerations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class BiographyPart : BasePartialData
    {
        #region öffentliche Properties
        
        public Biography ParentBio { get; set; }
        public int Order { get; set; }
        public BiographyPartType PartType { get; set; }
        public MediaFile MediaFile { get; set; }

        public MediaContentType MediaContentType { get; set; }

        //public BiographyLocalizationString Copyright { get; set; }
        public BiographyLocalizationString Title { get; set; }

        #endregion öffentliche Properties

        #region Ctor

        public BiographyPart(string id)
            : base(id)
        {
            //Copyright = new BiographyLocalizationString();
            Title = new BiographyLocalizationString();
            MediaFile = MediaFile.CreateEmpty();
        }

        #endregion Ctor

        #region öffentliche Methoden

        public void Normalize()
        {
            if (MediaFile != null) MediaFile.Normalize();
        }

        public override string ToString()
        {
            return $"{ParentBio}-{this.Id}";
        }

        #endregion öffentliche Methoden
    }
}
