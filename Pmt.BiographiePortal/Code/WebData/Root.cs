using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.Common;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.EntrieDatas;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData.TaxonomyDatas;
using Gemelo.Components.Common.IO;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.WebData
{
    public class Rootobject
    {
        #region private Member
        private BiographyStore m_StoreToMigrate;

        private List<Taxonomy> m_Taxonomies;
        private List<Entry> m_Entries;

        private DirectoryInfo m_DirectoryMediaWebSmall;
        private DirectoryInfo m_DirectoryMediaWebFinal;

        #endregion private Member

        #region öffentliche Eigenschaften

        public Taxonomy[] taxonomies { get; set; }
        public Entry[] entries { get; set; }

        #endregion öffentliche Eigenschaften

        #region ctor

        private Rootobject(BiographyStore store)
        {
            m_DirectoryMediaWebSmall = App.Current.EnsureLocalContentDataDirectory(App.ConstMediaWebSmallSizeDirectoryName);
            m_DirectoryMediaWebFinal = App.Current.EnsureLocalContentDataDirectory(App.ConstMediaWebFinalDirectoryName);

            m_StoreToMigrate = store;
            m_Taxonomies = new List<Taxonomy>();
            m_Entries = new List<Entry>();
            Migrate();
            taxonomies = m_Taxonomies.ToArray();
            entries = m_Entries.ToArray();
        }

        #endregion ctor

        #region öffentliche Methoden

        public static Rootobject FromBioStore(BiographyStore store)
        {
            return new Rootobject(store);
        }

        public string ToJson(bool isIndented = true)
        {
            JsonSerializerSettings settings = new()
            {
                NullValueHandling = NullValueHandling.Ignore,

                //ContractResolver = CmsContractResolver.Default
            };
            return JsonConvert.SerializeObject(this, isIndented ? Formatting.Indented : Formatting.None, settings);
        }

        public void SaveToJsonFile(string filePath, bool ensureDirectory = true)
        {
            if (ensureDirectory) Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllText(filePath, ToJson(isIndented: true));
        }

        #endregion öffentliche Methoden

        #region private Methoden


        private void Migrate()
        {
            MigrateTaxonomy();
            MigrateEntries();

            m_DirectoryMediaWebFinal.ClearFilesSecure(catchExceptions: false);

            RenameFileNamesToWebConformAndCopy();

            CreatePostersOfVideo();

        }

        private void CreatePostersOfVideo()
        {
            foreach (Entry entry in m_Entries)
            {
                foreach (Fragment fragment in entry.fragments)
                {
                    if (fragment.mediaType == Entry.FragmentMediaType_Video)
                    {
                        if (fragment.poster == null ||
                           string.IsNullOrEmpty(fragment.poster.filename) ||
                           !m_DirectoryMediaWebFinal.GetFileInfo(fragment.poster.filename).Exists)
                        {
                            fragment.poster = new Poster();
                            fragment.poster.filename = GetPosterFileName(fragment);

                            FileInfo videoFi = m_DirectoryMediaWebFinal.GetFileInfo(fragment.filename);
                            FileInfo posterFi = m_DirectoryMediaWebFinal.GetFileInfo(fragment.poster.filename);

                            CreatePosterWithFfmpeg(videoFi, posterFi);
                        }
                    }
                }
            }
        }

        private void CreatePosterWithFfmpeg(FileInfo videoFi, FileInfo posterFi)
        {
            // https://trac.ffmpeg.org/wiki/Create%20a%20thumbnail%20image%20every%20X%20seconds%20of%20the%20video
            Process p = Process.Start("ffmpeg", $"-i \"{videoFi.FullName}\" -ss 00:00:02.000 -frames:v 1 \"{posterFi.FullName}\"");
            p.WaitForExit();
        }

        private string GetPosterFileName(Fragment fragment)
        {
            string result = Path.GetFileNameWithoutExtension(fragment.filename) + ".poster.jpeg";
            return result;
        }

        private void RenameFileNamesToWebConformAndCopy()
        {
            List<IHasFileName> objectsToRename = new();
            foreach (Entry entry in m_Entries)
            {
                objectsToRename.AddRange(entry.GetObjectsToRename());
            }
            foreach (IHasFileName hasFileName in objectsToRename)
            {
                RenameFileAndInstanceAndCopy(hasFileName);
            }

            // prüfen, ob wirklich alle da sind
            foreach (IHasFileName hasFileName in objectsToRename)
            {
                FileInfo fileInfo = m_DirectoryMediaWebFinal.GetFileInfo(hasFileName.filename);
                if (!fileInfo.Exists)
                {
                    Debugger.Break();
                }
            }
        }

        private void RenameFileAndInstanceAndCopy(IHasFileName hasFileName)
        {
            string orignal;
            try
            {
                orignal = hasFileName.filename;
                string woExtension = Path.GetFileNameWithoutExtension(orignal);
                string extension = Path.GetExtension(orignal);

                FileInfo fileInfoSource = GetMediaWebSmallFileInfoWithDifferentExtensions(woExtension, extension);// m_DirectoryMediaWebSmall.GetFileInfo(orignal);

                if (!fileInfoSource.Exists)
                {
                    // Fehler. noch nicht migriert, File gibts nicht
                    Debugger.Break();
                    Debug.WriteLine(orignal);
                }
                else
                {
                    string renamed = RenameFileName(fileInfoSource.Name);
                    hasFileName.filename = renamed;
                    FileInfo fileInfoTarget = m_DirectoryMediaWebFinal.GetFileInfo(renamed);
                    fileInfoSource.CopyTo(fileInfoTarget, overwrite: true);
                }
            }
            catch (Exception ex)
            {
                Debugger.Break();
                throw;
            }
        }

        private string[] imageExtensions = { ".png", ".jpeg", ".jpg" };

        private FileInfo GetMediaWebSmallFileInfoWithDifferentExtensions(string woExtension, string extension)
        {
            FileInfo fileInfo = m_DirectoryMediaWebSmall.GetFileInfo(woExtension + extension);
            if (fileInfo.Exists) return fileInfo;

            foreach (string imageExtension in imageExtensions)
            {
                fileInfo = m_DirectoryMediaWebSmall.GetFileInfo(woExtension + imageExtension);
                if (fileInfo.Exists) return fileInfo;
            }

            return fileInfo;
        }

        private string RenameFileName(string name)
        {
            string woExtension = Path.GetFileNameWithoutExtension(name);
            string extension = Path.GetExtension(name);

            string result = (Slug.Normalize(woExtension) + extension).ToLower();
            return result;
        }

        private void MigrateEntries()
        {
            int counter = 1_000_000;
            foreach (var t in m_StoreToMigrate.Biographies
                .Where((x) => x.GetTeaserOrFirst()?.MediaContentType == MediaContentType.ImagePerson))
            {
                m_Entries.Add(new Entry(this, t, counter));
                counter++;
            }
            counter = 2_000_000;
            foreach (var t in m_StoreToMigrate.Biographies
                .Where((x) => x.GetTeaserOrFirst()?.MediaContentType == MediaContentType.ImageItem))
            {
                m_Entries.Add(new Entry(this, t, counter));
                counter++;
            }
            counter = 3_000_000;
            foreach (var t in m_StoreToMigrate.Biographies
                .Where((x) => x.GetTeaserOrFirst()?.MediaContentType == MediaContentType.Unknown))
            {
                m_Entries.Add(new Entry(this, t, counter));
                counter++;
            }
        }

        private void MigrateTaxonomy()
        {
            Taxonomy taxonomyHistoricBackgrounds = new Taxonomy("h", "Historische Hintergründe", "Historic Backgrounds", HistoricBackground.StaticTypeColor);
            Taxonomy taxonomyMigrationEffects = new Taxonomy("f", "Folgen", "Migration Effects", MigrationEffect.StaticTypeColor);
            Taxonomy taxonomyMigrationReasons = new Taxonomy("g", "Migrationsgründe", "Migration Reasons", MigrationReason.StaticTypeColor);
            Taxonomy taxonomyMigrationTypes = new Taxonomy("a", "Migrationsarten", "Migration Types", MigrationType.StaticTypeColor);
            Taxonomy taxonomyTimeRanges = new Taxonomy("z", "Zeiträume", "Time Ranges", TimeRange.StaticTypeColor);

            m_Taxonomies.Add(taxonomyMigrationEffects);
            m_Taxonomies.Add(taxonomyMigrationReasons);
            m_Taxonomies.Add(taxonomyMigrationTypes);
            m_Taxonomies.Add(taxonomyTimeRanges);

            foreach (var t in m_StoreToMigrate.HistoricBackgrounds)
            {
                if (HasChilds(t))
                {
                    taxonomyHistoricBackgrounds.AddTerm(Term.Create(t));
                }
            }

            if (taxonomyHistoricBackgrounds.terms.Count > 0)
            {
                m_Taxonomies.Add(taxonomyHistoricBackgrounds);
            }

            foreach (var t in m_StoreToMigrate.MigrationEffects)
            {
                taxonomyMigrationEffects.AddTerm(Term.Create(t));
            }
            foreach (var t in m_StoreToMigrate.MigrationReasons)
            {
                taxonomyMigrationReasons.AddTerm(Term.Create(t));
            }
            foreach (var t in m_StoreToMigrate.MigrationTypes)
            {
                taxonomyMigrationTypes.AddTerm(Term.Create(t));
            }
            foreach (var t in m_StoreToMigrate.TimeRanges)
            {
                taxonomyTimeRanges.AddTerm(Term.Create(t));
            }
        }

        private bool HasChilds(HistoricBackground t)
        {
            foreach (Biography bio in m_StoreToMigrate.Biographies)
            {
                if (bio.HistoricBackgrounds.Contains(t))
                    return true;
            }

            return false;
        }

        public Term GetTaxonomyTerm(string id)
        {
            Term result = null;
            foreach (Taxonomy taxonomy in m_Taxonomies)
            {
                result = taxonomy.GetTaxonomyTerm(id);
                if (result != null) break;
            }
            return result;
        }


        #endregion private Methoden

    }

}
