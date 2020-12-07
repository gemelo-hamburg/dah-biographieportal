using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.EntrieDatas
{
    /// <summary>
    /// Haupt Daten Klasse, in der jeweils ein kompletten Datensatz gespeichert ist.
    /// </summary>
    public class Entry
    {
        #region Konstanten

        public const string MediaType_Image = "image";
        public const string FragmentType_Text = "text";
        public const string FragmentType_Headline = "headline";
        public const string FragmentType_Media = "media";
        public const string FragmentMediaType_Image = "image";
        public const string FragmentMediaType_Video = "video";

        #endregion Konstanten

        #region private Members

        private Rootobject m_Rootobject;

        #endregion private Members

        #region öffentliche Eigenschaften

        public string id { get; set; } = string.Empty;
        public int order { get; set; } = 0;
        public LocalizationString title { get; set; } = new();
        public Slug slug { get; set; } = new();
        public List<TaxonomyReference> taxonomies { get; set; } = new();
        public Preview preview { get; set; } = new();
        public List<Fragment> fragments { get; set; } = new();

        #endregion öffentliche Eigenschaften

        #region ctors

        public Entry(Rootobject rootobject, Biography t, int orderId)
        {
            m_Rootobject = rootobject;

            id = t.Id;
            order = orderId;

            MigrationTaxonomies(t.MigrationEffects);
            MigrationTaxonomies(t.MigrationReasons);
            MigrationTaxonomies(t.MigrationTypes);
            MigrationTaxonomies(t.TimeRanges);
            MigrationTaxonomies(t.HistoricBackgrounds);

            MigrateTitleAndPreview(t);

            MigrateFragements(t.Parts);
        }

        private void MigrateFragements(List<BiographyPart> parts)
        {
            foreach (BiographyPart part in parts)
            {
                MigrateFragement(part);
            }
        }

        private void MigrateFragement(BiographyPart part)
        {
            fragments.AddRange(Fragment.CreateFromPart(part));
        }

        private void MigrateTitleAndPreview(Biography t)
        {
            BiographyPart teaser = t.GetTeaserOrFirst();

            title = new LocalizationString(teaser.Title);
            slug = new Slug(teaser.Title);
            preview = new Preview(teaser);
        }

        private void MigrationTaxonomies(BasePartialData[] basePartialDatas)
        {
            foreach (BasePartialData basePartialData in basePartialDatas)
            {
                TaxonomyDatas.Term term = m_Rootobject.GetTaxonomyTerm(basePartialData.Id);
                if (term != null)
                {
                    string taxId = term.Taxonomy.id;

                    TaxonomyReference taxRef = GetOrCreateTaxReference(taxId);
                    taxRef.terms.Add(new TermReference { id = term.id });
                }
                else
                {
                    Debugger.Break();
                }
            }
        }



        private TaxonomyReference GetOrCreateTaxReference(string taxId)
        {
            foreach (TaxonomyReference taxonomyReference in taxonomies)
            {
                if (taxonomyReference.id == taxId)
                {
                    return taxonomyReference;
                }
            }

            TaxonomyReference result = new TaxonomyReference { id = taxId };
            taxonomies.Add(result);
            return result;
        }

        #endregion ctors

        #region öffentliche Eigenschaften

        public IEnumerable<IHasFileName> GetObjectsToRename()
        {
            List<IHasFileName> objectsToRename = new();

            if (!string.IsNullOrWhiteSpace(preview.filename)) objectsToRename.Add(preview);

            foreach (Fragment fragment in fragments)
            {
                if (!string.IsNullOrWhiteSpace(fragment.filename))
                {
                    objectsToRename.Add(fragment);
                }
                if (fragment.poster != null && !string.IsNullOrWhiteSpace(fragment.poster.filename))
                {
                    objectsToRename.Add(fragment.poster);
                }
            }

            return objectsToRename;
        }


        #endregion öffentliche Eigenschaften

    }
}
