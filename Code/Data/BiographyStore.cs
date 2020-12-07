using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pete.Components.Extensions.Extensions;
using Pete.Components.WpfExtensions.Extensions;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class BiographyStore : IEnumerable<Biography>
    {
        #region Private Variablen

        private List<Biography> m_ContentList;

        private Dictionary<string, HistoricBackground> m_HistoricBackgrounds;
        private Dictionary<string, MigrationEffect> m_MigrationEffects;
        private Dictionary<string, MigrationReason> m_MigrationReasons;
        private Dictionary<string, MigrationType> m_MigrationTypes;
        private Dictionary<string, TimeRange> m_TimeRanges;

        #endregion Private Variablen

        #region Öffentliche Properties

        #endregion Öffentliche Properties

        #region Events

        #endregion Events

        #region Konstruktur und Initialisierungen

        public BiographyStore()
        {
            InitializeThings();
        }

        private void InitializeThings()
        {
            m_ContentList = new List<Biography>();
            m_HistoricBackgrounds = new Dictionary<string, HistoricBackground>();
            m_MigrationEffects = new Dictionary<string, MigrationEffect>();
            m_MigrationReasons = new Dictionary<string, MigrationReason>();
            m_MigrationTypes = new Dictionary<string, MigrationType>();
            m_TimeRanges = new Dictionary<string, TimeRange>();
        }

        #endregion Konstruktur und Initialisierungen

        #region Öffentliche Methoden

        public void Add(Biography bio)
        {
            m_ContentList.Add(bio);
        }
        public void Add(TimeRange timeRange)
        {
            m_TimeRanges.Add(timeRange.Id, timeRange);
        }
        public void Add(HistoricBackground hb)
        {
            m_HistoricBackgrounds.Add(hb.Id, hb);
        }
        public void Add(MigrationEffect me)
        {
            m_MigrationEffects.Add(me.Id, me);
        }
        public void Add(MigrationReason mr)
        {
            m_MigrationReasons.Add(mr.Id, mr);
        }
        public void Add(MigrationType mt)
        {
            m_MigrationTypes.Add(mt.Id, mt);
        }


        public IEnumerable<BasePartialData> GetAllTimeRanges()
        {
            return m_TimeRanges.Values;
        }
        public IEnumerable<BasePartialData> GetAllMigrationEffects()
        {
            return m_MigrationEffects.Values;
        }
        public IEnumerable<BasePartialData> GetAllMigrationReasons()
        {
            return m_MigrationReasons.Values;
        }
        public IEnumerable<BasePartialData> GetAllMigrationTypes()
        {
            return m_MigrationTypes.Values;
        }


        public TimeRange[] GetTimeRanges(string[] ids)
        {
            return GetMetas<TimeRange>(ids, m_TimeRanges);
        }
        public HistoricBackground[] GetHistoricBackgrounds(string[] ids)
        {
            return GetMetas<HistoricBackground>(ids, m_HistoricBackgrounds);
        }
        public MigrationEffect[] GetMigrationEffects(string[] ids)
        {
            return GetMetas<MigrationEffect>(ids, m_MigrationEffects);
        }
        public MigrationType[] GetMigrationTypes(string[] ids)
        {
            return GetMetas<MigrationType>(ids, m_MigrationTypes);
        }
        public MigrationReason[] GetMigrationReasons(string[] ids)
        {
            return GetMetas<MigrationReason>(ids, m_MigrationReasons);
        }

      

        #endregion Öffentliche Methoden

        #region IEnumerable
        public IEnumerator<Biography> GetEnumerator()
        {
            return m_ContentList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_ContentList.GetEnumerator();
        }

        #endregion IEnumerable

        #region Private Methoden


        private T[] GetMetas<T>(string[] ids, Dictionary<string, T> dict)
        {
            List<T> result = new List<T>();
            foreach (string id in ids)
            {
                string idNormed = id.Trim().ToLower();
                if (dict.ContainsKey(idNormed)) result.Add(dict[idNormed]);
            }
            return result.ToArray();
        }


        #endregion Private Methoden

        #region Eventhandler

        #endregion Eventhandler


    }


}
