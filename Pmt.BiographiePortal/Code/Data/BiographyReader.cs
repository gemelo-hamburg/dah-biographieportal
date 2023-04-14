using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Gemelo.Components.ClosedXml.Code.Common;
using Gemelo.Components.ClosedXml.Code.Enumerations;
using Gemelo.Components.ClosedXml.Code.ExcelReader;


namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class BiographyReader : SimpleExcelReader
    {
        public const string ConstMissingImagePlaceHolder = "platzhalter-dah.png";

        #region Private Variablen

        private int m_NoMasterIdFoundCounter;
        private Biography m_CurrentBiography;
        private DirectoryInfo m_MediaFolder;
        private Dictionary<string, FileInfo> m_MediaFolderContent;


        #endregion Private Variablen

        #region Öffentliche Properties

        public BiographyStore Store { get; set; }

        #endregion Öffentliche Properties

        #region Events


        public event EventHandler<EventArgs> ReassignStore;

        protected void OnReassignStore()
        {
            ReassignStore?.Invoke(this, new EventArgs());
        }


        #endregion Events

        #region Konstruktur und Initialisierungen

        public BiographyReader()
        {
            InitializeThings();
        }

        private void InitializeThings()
        {
            m_MediaFolderContent = new Dictionary<string, FileInfo>();
        }

        #endregion Konstruktur und Initialisierungen

        #region Öffentliche Methoden

        public async Task AnalyseMediaDirectory(DirectoryInfo dirMedia)
        {
            if(!dirMedia.Exists)
            {
                dirMedia.Create();
                dirMedia.Refresh();
            }
            m_MediaFolder = dirMedia;
            await Task.Run(() =>
            {
                m_MediaFolderContent.Clear();
                FileInfo[] files = m_MediaFolder.GetFiles();
                foreach (FileInfo file in files)
                {
                    m_MediaFolderContent[file.Name.ToLower().Trim()] = file;
                }
            });
        }

        #endregion Öffentliche Methoden

        #region protected Methoden

        protected override void AfterOpeningDocument(FileInfo file)
        {
            int counter = 0;
            foreach (var bio in Store)
            {
                foreach (BiographyPart part in bio.Parts)
                {
                    if (part.MediaFile.Type != MediaFileType.NotExist)
                    {
                        this.OnProgress(counter, part.Id, "Erzeuge Mediafiles");
                        if (App.Current.StationDefinition.ImportReader_ShowUnimportantMessages)
                        {
                            AddLog(LogEntry.Create(LogEntryType.Info, "Erzeuge Medias", part.Id));
                        }
                        part.Normalize();
                        counter++;
                    }
                }
            }

            OnReassignStore();
        }

        protected override void DocumentOpened()
        {
            if (ActivateWorksheet("Zeiträume")) ParseRows(2, ExcelSheetName.TimeRanges);
            if (ActivateWorksheet("Migrationsarten")) ParseRows(2, ExcelSheetName.MigrationTypes);
            if (ActivateWorksheet("Migrationsgründe")) ParseRows(2, ExcelSheetName.MigrationReasons);
            if (ActivateWorksheet("Folgen")) ParseRows(2, ExcelSheetName.MigrationEffects);
            if (ActivateWorksheet("Historische Hintergründe")) ParseRows(2, ExcelSheetName.HistoricBackgrounds);

            if (ActivateWorksheet("Haupttabelle")) ParseRows(startRowNumber: 3, ExcelSheetName.Master);

        }

        protected override void RowParsed(int currentRowNumber, string workSheetName, object idTag)
        {
            if (idTag is ExcelSheetName sheet)
            {
                switch (sheet)
                {
                    case ExcelSheetName.Master:
                        ReadMasters();
                        break;
                    case ExcelSheetName.TimeRanges:
                        ReadTimeRanges();
                        break;
                    case ExcelSheetName.MigrationTypes:
                        ReadMigrationTypes();
                        break;
                    case ExcelSheetName.MigrationReasons:
                        ReadMigrationReasons();
                        break;
                    case ExcelSheetName.MigrationEffects:
                        ReadMigrationEffects();
                        break;
                    case ExcelSheetName.HistoricBackgrounds:
                        ReadHistoricBackgrounds();
                        break;
                    default:
                        break;
                }
            }
        }


        #endregion protected Methoden

        #region Private Methoden

        private void ReadMigrationTypes()
        {
            string id = GetTOfCurrentRow<string>("A");
            if (string.IsNullOrWhiteSpace(id)) StopCurrentParsingRows();
            else
            {
                MigrationType mt = new MigrationType(id);
                mt.Text.De = GetTOfCurrentRow<string>("B");
                mt.Text.En = GetTOfCurrentRow<string>("C");
                mt.Text.Normalize();
                mt.NewLine = GetBoolOfCurrentRow("D");
                Store.Add(mt);
            }
        }

        private void ReadMigrationReasons()
        {
            string id = GetTOfCurrentRow<string>("A");
            if (string.IsNullOrWhiteSpace(id)) StopCurrentParsingRows();
            else
            {
                MigrationReason mr = new MigrationReason(id);
                mr.Text.De = GetTOfCurrentRow<string>("B");
                mr.Text.En = GetTOfCurrentRow<string>("C");
                mr.Text.Normalize();
                mr.NewLine = GetBoolOfCurrentRow("D");
                Store.Add(mr);
            }
        }

        private void ReadMigrationEffects()
        {
            string id = GetTOfCurrentRow<string>("A");
            if (string.IsNullOrWhiteSpace(id)) StopCurrentParsingRows();
            else
            {
                MigrationEffect me = new MigrationEffect(id);
                me.Text.De = GetTOfCurrentRow<string>("B");
                me.Text.En = GetTOfCurrentRow<string>("C");
                me.Text.Normalize();
                me.NewLine = GetBoolOfCurrentRow("D");
                Store.Add(me);
            }
        }


        private void ReadHistoricBackgrounds()
        {
            string id = GetTOfCurrentRow<string>("A");
            if (string.IsNullOrWhiteSpace(id)) StopCurrentParsingRows();
            else
            {
                HistoricBackground hb = new HistoricBackground(id);
                hb.Headline.De = GetTOfCurrentRow<string>("B");
                hb.Headline.En = GetTOfCurrentRow<string>("C");
                hb.Headline.Normalize();
                hb.Text.De = GetTOfCurrentRow<string>("D");
                hb.Text.En = GetTOfCurrentRow<string>("E");
                hb.Text.Normalize();
                hb.NewLine = GetBoolOfCurrentRow("F");
                Store.Add(hb);
            }
        }

        private void ReadTimeRanges()
        {
            string id = GetTOfCurrentRow<string>("A");
            if (string.IsNullOrWhiteSpace(id)) StopCurrentParsingRows();
            else
            {
                TimeRange tr = new TimeRange(id);
                tr.Text.De = GetTOfCurrentRow<string>("B");
                tr.Text.En = GetTOfCurrentRow<string>("C");
                tr.Text.Normalize();
                tr.NewLine = GetBoolOfCurrentRow("D");
                Store.Add(tr);
            }
        }



        private void ReadMasters()
        {
            string id = GetTOfCurrentRow<string>("A");
            if (string.IsNullOrWhiteSpace(id))
            {
                m_NoMasterIdFoundCounter++;
                if (m_NoMasterIdFoundCounter > 50)
                {
                    StopCurrentParsingRows();
                }
            }
            else
            {
                m_NoMasterIdFoundCounter = 0;
            }

            if (!string.IsNullOrWhiteSpace(id) &&
                (m_CurrentBiography == null || (m_CurrentBiography.Id != id)))
            {
                m_CurrentBiography = new Biography(id);
                Store.Add(m_CurrentBiography);
            }

            string type = GetTOfCurrentRow<string>("C");
            if (m_CurrentBiography != null && !string.IsNullOrWhiteSpace(type))
            {
                BiographyPartType partType = GetPartType(type);
                switch (partType)
                {
                    case BiographyPartType.Meta:
                        ReadMeta();
                        break;
                    default:
                        ReadOtherMasters(partType);
                        break;
                }
            }
        }

        private void ReadOtherMasters(BiographyPartType partType)
        {
            if (partType != BiographyPartType.Unknown && partType != BiographyPartType.Meta)
            {
                int order = GetTOfCurrentRow<int>("B");
                BiographyPart part = new BiographyPart($"{m_CurrentBiography.Id}-{order}");
                part.ParentBio = m_CurrentBiography;
                part.Order = order;
                part.PartType = partType;
                part.MediaFile = GetMediaFile(part, GetTOfCurrentRow<string>("D"));
                if (!string.IsNullOrWhiteSpace(GetStringOfCurrentRow("E")))
                {
                    part.MediaContentType = GetEnumOfCurrentRow<MediaContentType>("E");
                }
                part.MediaFile.Copyright.De = GetTOfCurrentRow<string>("F");
                part.MediaFile.Copyright.En = part.MediaFile.Copyright.De;
                part.MediaFile.Copyright.Normalize();
                part.Title.De = GetTOfCurrentRow<string>("G");
                part.Title.En = GetTOfCurrentRow<string>("H");
                part.Title.Normalize();
                part.Text.De = GetTOfCurrentRow<string>("I");
                part.Text.En = GetTOfCurrentRow<string>("J");
                part.Text.Normalize();
                m_CurrentBiography.Add(part);
            }
        }

        private BiographyPartType GetPartType(string type)
        {
            switch (type.ToLower().Trim())
            {
                case "meta":
                    return BiographyPartType.Meta;
                case "image":
                    return BiographyPartType.Image;
                case "movie":
                    return BiographyPartType.Movie;
                case "teaser":
                    return BiographyPartType.Teaser;
                default:
                    return BiographyPartType.Unknown;
            }
        }

        private MediaFile GetMediaFile(BiographyPart part, string fileName)
        {
            OnProgress(0, part.Id, fileName);
            FileInfo fi = GetMediaFileInfo(fileName);
            if (fi == null || !fi.Exists)
            {
                AddLog(LogEntry.Create(LogEntryType.Error, "MediaFile existiert nicht", argument: $"{part.Id}: {fileName}"));

                fi = GetMediaFileInfo(ConstMissingImagePlaceHolder);
            }

            MediaFile result = new MediaFile(fi);

            if (result.Type == MediaFileType.Video)
            {
                // PreviewImage suchen

                string previewImageName = Path.GetFileNameWithoutExtension(fileName);
                FileInfo fiPreview = GetMediaFileInfo(previewImageName + ".jpg");
                if (fiPreview == null || !fiPreview.Exists) fiPreview = GetMediaFileInfo(previewImageName + ".jpeg");
                if (fiPreview == null || !fiPreview.Exists) fiPreview = GetMediaFileInfo(previewImageName + ".png");
                if (fiPreview == null || !fiPreview.Exists)
                {
                    AddLog(LogEntry.Create(LogEntryType.Error, "Preview für Video MediaFile existiert nicht", argument: $"{part.Id}: {previewImageName}"));
                    fiPreview = GetMediaFileInfo(ConstMissingImagePlaceHolder);
                }

                result.VideoPreview = fiPreview;
            }

            return result;
        }

        private FileInfo GetMediaFileInfo(string fileName)
        {
            fileName = fileName.ToLower().Trim();
            if (m_MediaFolderContent.ContainsKey(fileName)) return m_MediaFolderContent[fileName];
            else return null;
        }

        private void ReadMeta()
        {
            m_CurrentBiography.Add(GetTimeRanges("K"));
            m_CurrentBiography.Add(GetMigrationTypes("L"));
            m_CurrentBiography.Add(GetMigrationReasons("M"));
            m_CurrentBiography.Add(GetMigrationEffects("N"));
            m_CurrentBiography.Add(GetHistoricBackgrounds("O"));
        }

        private TimeRange[] GetTimeRanges(string column)
        {
            string[] ids = GetMetaIds(column);
            return Store.GetTimeRanges(ids);
        }

        private MigrationType[] GetMigrationTypes(string column)
        {
            string[] ids = GetMetaIds(column);
            return Store.GetMigrationTypes(ids);
        }

        private MigrationReason[] GetMigrationReasons(string column)
        {
            string[] ids = GetMetaIds(column);
            return Store.GetMigrationReasons(ids);
        }

        private MigrationEffect[] GetMigrationEffects(string column)
        {
            string[] ids = GetMetaIds(column);
            return Store.GetMigrationEffects(ids);
        }

        private HistoricBackground[] GetHistoricBackgrounds(string column)
        {
            string[] ids = GetMetaIds(column);
            return Store.GetHistoricBackgrounds(ids);
        }


        private string[] GetMetaIds(string column)
        {
            string idString = GetTOfCurrentRow<string>(column);
            if (string.IsNullOrWhiteSpace(idString)) return new string[0];
            else return idString.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
        }



        #endregion Private Methoden

        #region Eventhandler

        #endregion Eventhandler


    }


}
