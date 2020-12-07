using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class Biography
    {
        #region öffentliche Properties
        public string Id { get; private set; }
        public bool MetaSet { get; private set; }
        public List<BiographyPart> Parts { get; private set; }
        public TimeRange[] TimeRanges { get; private set; }
        public MigrationType[] MigrationTypes { get; private set; }
        public MigrationReason[] MigrationReasons { get; private set; }
        public MigrationEffect[] MigrationEffects { get; private set; }
        public HistoricBackground[] HistoricBackgrounds { get; private set; }

        #endregion öffentliche Properties

        #region ctor

        public Biography(string id)
        {
            Id = id;
            Parts = new List<BiographyPart>();
        }


        #endregion ctor

        #region öffentliche Methoden

        public void Add(BiographyPart part)
        {
            Parts.Add(part);
        }

        public BiographyPart GetTeaserOrFirst()
        {
            foreach (BiographyPart p in Parts)
            {
                if (p.PartType == Enumerations.BiographyPartType.Teaser)
                    return p;
            }
            if (Parts.Count > 0) return Parts[0];
            else return null;
        }

        public void Add(TimeRange[] timeRanges)
        {
            TimeRanges = timeRanges;
        }

        public void Add(MigrationType[] migrationTypes)
        {
            MigrationTypes = migrationTypes;
        }

        public void Add(MigrationEffect[] migrationEffects)
        {
            MigrationEffects = migrationEffects;
        }

        public void Add(MigrationReason[] migrationReasons)
        {
            MigrationReasons = migrationReasons;
        }

        public void Add(HistoricBackground[] historicBackgrounds)
        {
            HistoricBackgrounds = historicBackgrounds;
        }

        public bool HasCategorie(BasePartialData categorie)
        {
            if (categorie is MigrationEffect mf) return MigrationEffects.Contains(mf);
            else if (categorie is MigrationReason mr) return MigrationReasons.Contains(mr);
            else if (categorie is MigrationType mt) return MigrationTypes.Contains(mt);
            else if (categorie is TimeRange rt) return TimeRanges.Contains(rt);
            else return false;
        }

        public bool HasSimilarCategories(Biography parentBio, int minCount)
        {
            int counter = 0;

            counter += GetSimilarCount(this.TimeRanges, parentBio.TimeRanges);
            counter += GetSimilarCount(this.MigrationEffects, parentBio.MigrationEffects);
            counter += GetSimilarCount(this.MigrationReasons, parentBio.MigrationReasons);
            counter += GetSimilarCount(this.MigrationTypes, parentBio.MigrationTypes);

            return (counter >= minCount);
        }

        public override string ToString()
        {
            return Id;
        }


        #endregion öffentliche Methoden

        #region private Methoden

        private static int GetSimilarCount<DataType>(DataType[] coll1, DataType[] coll2)
        {
            int result = 0;
            foreach (DataType data in coll1)
            {
                if (coll2.Contains(data)) result++;
            }

            return result;
        }

        #endregion private Methoden
    }
}
