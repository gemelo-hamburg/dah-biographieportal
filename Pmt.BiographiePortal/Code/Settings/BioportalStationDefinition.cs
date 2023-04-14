using Gemelo.Components.Cms.Data.Base;
using Gemelo.Components.Cms.Data.Json;
using Gemelo.Components.Cms.Data.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.Settings
{
    public class BioportalStationDefinition : CmsStationDefinition
    {
        public CmsTimeSpan RestartInterval { get; } = new CmsTimeSpan(TimeSpan.FromMinutes(2));

        public CmsBool WaitIfContentReadingError { get; } = new CmsBool(true);
        public CmsTimeSpan CategorySliderTimeout { get; } = new CmsTimeSpan(TimeSpan.FromSeconds(20));
        public CmsString VisitorMessagesPath { get; } = new CmsString("y:\\Messages");
        public CmsString ContentDataPath { get; } = new CmsString("y:\\ContentData");
        public CmsTimeSpan CheckNewContentInterval { get; } = new CmsTimeSpan(TimeSpan.FromMinutes(2));
        //public CmsTimeSpan SendOwnStory_SuccessMessageDuration { get; } = new CmsTimeSpan(TimeSpan.FromSeconds(5));
        public CmsTimeSpan RestartInterval_SendOwnStory { get; } = new CmsTimeSpan(TimeSpan.FromMinutes(5));
        public CmsBool ImportReader_ShowUnimportantMessages { get; } = new CmsBool(false);

        public static BioportalStationDefinition FromJson(string json, bool errorOnMissingMembers = true)
        {
            JsonSerializerSettings settings = new()
            {
                ContractResolver = CmsContractResolver.Default,
                TypeNameHandling = TypeNameHandling.All,
                MissingMemberHandling = errorOnMissingMembers ?
                    MissingMemberHandling.Error : MissingMemberHandling.Ignore
            };
            return JsonConvert.DeserializeObject<BioportalStationDefinition>(json, settings);
        }

        public static BioportalStationDefinition LoadFromJsonFile(string filePath, bool errorOnMissingMembers = true)
        {
            return FromJson(File.ReadAllText(filePath, encoding: Encoding.UTF8), errorOnMissingMembers);
        }
    }
}
