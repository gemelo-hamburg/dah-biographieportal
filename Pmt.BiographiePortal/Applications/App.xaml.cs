using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Windows;
using Gemelo.Applications.Pmt.BiographiePortal.Code.Settings;
using Gemelo.Applications.Pmt.BiographiePortal.Code.WebData;
using Gemelo.Components.Cms.Data.Base;
using Gemelo.Components.Common.Exhibits.Applications;
using Gemelo.Components.Common.Exhibits.Settings;
using Gemelo.Components.Common.Exhibits.Windows;
using Gemelo.Components.Common.Settings;
using Gemelo.Components.Common.Text;
using Gemelo.Components.Common.Tracing;
using Gemelo.Components.Common.Wpf.Localization;
using Gemelo.Components.Pmt.Applications;

//using Pete.Components.WpfExtensions.Controls;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Gemelo.Applications.Biographieportal.Applications
{
    /*
     * Hardwareinfo:
     * - Touchscreen: Iiyama TF2215MC-B2
            21,5“ (54,6 cm) Open-Frame-Touchscreen mit Edge-to-Edge-Glas
            Panel: IPS
            Touch: 10-Punkt-PCAP
            Auflösung: 1920 x 1080 px
     * 
     * - PC: Fujitsu Esprimo G558
            Professioneller Mini-PC
            Prozessor: Intel Core i3-9100
            Hauptspeicher: 8 GB DDR4
            SSD: 256 GB M.2 NVMe
    *
    * - USO Einhandhörer
    * - Vorverkstärker
     */

    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : PmtApplication
    {
        #region Konstanten

        public const string ConstTraceCategory = "Biographieportal";
        private const string ConstStationDefinitionFilename = "Biographieportal.json";

        public const string LanguageEn = "en"; // Englisch
        public const string LanguageDe = "de"; // Deutsch

        public const bool HideLanguageBtn = true;

        private readonly static List<string> ConstAvailableLanguages = new string[] {
            LanguageDe,
            LanguageEn,
        }.ToList();

        private const string ConstDefaultLanguage = LanguageDe;

        private readonly static string[] ConstFontFamiliesPrerequisite = new string[]
        {
            "Helvetica",
            "Weissenhof Grotesk"
        };

        private const string ConstGlobalSettingsFilename = @"Data\GlobalSettings.json";

        public new const string ConstDataRootDirectoryName = "Data";
        public const string ConstContentDataDirectoryName = "ContentData";
        public const string ConstMediaDirectoryName = "Media";
        public const string ConstContentDefinitionDirectoryName = "ContentDefinition";
        public const string ConstMediaWebSmallSizeDirectoryName = "Media Web Small Size";
        public const string ConstMediaWebFinalDirectoryName = "Media Web Final";
        public const string ConstInfoDirectoryName = "Info";

        #endregion Konstanten

        #region private Member

        private bool m_ContentAutoUpdater_FirstFileScanFinished = true;
        protected Stopwatch m_TimeStopwatch;

        #endregion private Member

        #region öffentliche Properties

        private static App s_Current;
        public new static App Current
        {
            get
            {
                if (s_Current == null) s_Current = Application.Current as App;
                return s_Current;
            }
        }

        public BioportalStationDefinition StationDefinition { get; protected set; }

        protected override CmsStationDefinition CmsStationDefinition => StationDefinition;


        public bool IsMainWindowClosed { get; private set; }

        public bool WaitIfContentReadingError { get; private set; }
        public TimeSpan CategorySliderTimeout { get; private set; }
        public string VisitorMessagesPath { get; private set; }
        public string ServerContentDataPath { get; private set; }
        public string LocalContentDataPath { get; private set; }
        public TimeSpan CheckNewContentInterval { get; private set; }

        public double Time => m_TimeStopwatch.ElapsedMilliseconds;

        public override string StationID { get; protected set; } = ConstTraceCategory;
        public override string CmsStationName => ConstTraceCategory;

        public MainWindow ExhibitMainWindow => (MainWindow)base.MainWindow;

        public BiographyReader BiographyReader { get; private set; }
        public BiographyStore BiographyStore { get; private set; }
        public ContentAutoUpdater ContentAutoUpdater { get; private set; }

        public bool CreateJson { get; private set; } = false;
        public bool CreateJsonForWebsite { get; private set; } = false;

        #endregion öffentliche Properties

        #region Ereignisse

        public event EventHandler LocalLanguageChanged;

        public void OnLocalLanguageChanged()
        {
            LocalLanguageChanged?.Invoke(this, null);
        }

        #endregion Ereignisse       

        #region Konstruktur und Initialisierung

        public App()
        {
            Settings.AddAdditionalFonts(ConstAdditionalFonts);
            InitializeStartArguments();

            InitializeDataStore();

            LocalizationExtensions.UseVisualTree = true;
            LocalizationExtensions.UseLogicalTree = true;
        }

        private void InitializeStartArguments()
        {
            Settings.AddStartArgumentHandler(new StartArgumentHandler
            {
                IsMandatory = false,
                NeedsValue = true,
                MatchingKey = "servercontentdatapath",
                Description = "Path to the Server ContentData",
                ValuePlaceholder = "path",
                ValueAction = (value) =>
                ServerContentDataPath = value.Trim(),
            });
            Settings.AddStartArgumentHandler(new StartArgumentHandler
            {
                IsMandatory = false,
                NeedsValue = true,
                MatchingKey = "visitormessagespath",
                Description = "Path to the Server Visitor Messages",
                ValuePlaceholder = "path",
                ValueAction = (value) => VisitorMessagesPath = value.Trim()
            });

            Settings.AddStartArgumentHandler(new StartArgumentHandler
            {
                IsMandatory = false,
                NeedsValue = false,
                MatchingKey = "createjson",
                Description = "Erzeugt im Folder vom Excelfile ein Json Datenfile",
                ExistsAction = () => CreateJson = true
            });

            Settings.AddStartArgumentHandler(new StartArgumentHandler
            {
                IsMandatory = false,
                NeedsValue = false,
                MatchingKey = "createwebjson",
                Description = "Erzeugt im Folder vom Excelfile ein Json Datenfile für die Bio Portal Website",
                ExistsAction = () => CreateJsonForWebsite = true
            });

        }

        private void InitializeDataStore()
        {
            BiographyStore = new BiographyStore();
            BiographyReader = new BiographyReader();
            BiographyReader.ReassignStore += BiographyReader_ReassignStore;
        }


        private void InitializeProperties()
        {
            RestartTimer.Interval = StationDefinition.RestartInterval;
            WaitIfContentReadingError = StationDefinition.WaitIfContentReadingError;
            CategorySliderTimeout = StationDefinition.CategorySliderTimeout;
            CheckNewContentInterval = StationDefinition.CheckNewContentInterval;

            m_TimeStopwatch = new Stopwatch();
            m_TimeStopwatch.Start();
        }

        private void InitializeDataPaths()
        {
            if (StationDefinition != null)
            {
                if (string.IsNullOrWhiteSpace(VisitorMessagesPath)) VisitorMessagesPath = StationDefinition.VisitorMessagesPath;
                if (string.IsNullOrWhiteSpace(ServerContentDataPath)) ServerContentDataPath = StationDefinition.ContentDataPath;
            }

            LocalContentDataPath = EnsureAppDataDirectory(ConstContentDataDirectoryName).FullName;

            TraceX.WriteImportant("VisitorMessagesPath", category: "App", arguments: VisitorMessagesPath); // neu ab 1.302
            TraceX.WriteImportant("LocalContentDataPath", category: "App", arguments: LocalContentDataPath); // neu ab 1.302
            TraceX.WriteImportant("ServerContentDataPath", category: "App", arguments: ServerContentDataPath); // neu ab 1.302
        }

        private void InitializeContentData()
        {
            ContentAutoUpdater = new ContentAutoUpdater();
            ContentAutoUpdater.NewFilesFound += ContentAutoUpdater_NewFilesFound;
            ContentAutoUpdater.FileScanFinished += ContentAutoUpdater_FileScanFinished;
            ContentAutoUpdater.StartTask();
        }


        #endregion Konstruktur und Initialisierung

        #region öffentliche Methoden

        public DirectoryInfo EnsureAppDataDirectory(string folderName)
        {
            return EnsureDirectory(Directories.AppDataDirectory, folderName);
        }

        public DirectoryInfo EnsureLocalContentDataDirectory(string folderName)
        {
            return EnsureDirectory(LocalContentDataPath, folderName);
        }

        public DirectoryInfo EnsureDirectory(DirectoryInfo baseFolder, string folderName)
        {
            return EnsureDirectory(baseFolder.FullName, folderName);
        }

        public static DirectoryInfo EnsureDirectory(string baseFolder, string folderName)
        {
            DirectoryInfo result = new DirectoryInfo(Path.Combine(baseFolder, folderName));
            if (!result.Exists)
            {
                result.Create();
                result.Refresh();
            }
            return result;
        }

        #endregion öffentliche Methoden

        #region protected Methoden

        protected override ExhibitMainWindow CreateMainWindow()
        {
            var result = base.CreateMainWindow();
            result.Closed += MainWindows_Closed;
            return result;
        }

        protected override async Task UpdateStationDefinitionInternally()
        {
            TraceX.WriteInformational("UpdateStationDefinitionInternally", category: nameof(App));
            string stationJsonFilePath =
                Path.Combine(DataDirectoryPath, ConstStationDefinitionFilename);

            if (!File.Exists(stationJsonFilePath))
            {
                StationDefinition = new BioportalStationDefinition();
                StationDefinition.SaveToJsonFile(stationJsonFilePath);
                await Task.Delay(250);
            }

            StationDefinition =
                BioportalStationDefinition.LoadFromJsonFile(stationJsonFilePath);

            InitializeDataPaths();

            TraceX.WriteInformational("UpdateStationDefinitionInternally finished", category: nameof(App));
        }

        protected override async Task InitializeAfterMainWindowAsync()
        {
            await base.InitializeAfterMainWindowAsync();

            InitializeProperties();
            InitializeDataPaths();

#if (DEBUG)
            WaitIfContentReadingError = true;

            RestartTimer.Interval = TimeSpan.FromSeconds(40);
            CategorySliderTimeout = TimeSpan.FromSeconds(20);
            //VisitorMessagesPath = @"c:\temp\dah\newmessages";
            //ServerContentDataPath = @"C:\Temp\g0498 biographie\Daten 2020-07-23\contentdata";
            //ServerContentDataPath = @"C:\+dev\Expo\Allgemein\G0498 Biographieportal\ContentData";
#endif

            InitializeContentData();
        }

        #endregion protected Methoden

        #region private Methoden

        private async Task UpdateDataStore()
        {
            try
            {
                TraceX.WriteVerbose("UpdateDataStore Start", arguments: $"", category: nameof(App));
                DirectoryInfo dirMedia = EnsureLocalContentDataDirectory(ConstMediaDirectoryName);
                DirectoryInfo dirContentDef = EnsureLocalContentDataDirectory(ConstContentDefinitionDirectoryName);

                BiographyReader.Store = new BiographyStore();
                BiographyReader.Progress += BiographyReader_Progress;
                BiographyReader.ReadingFinished += BiographyReader_ReadingFinished;
                await BiographyReader.AnalyseMediaDirectory(dirMedia);
                TraceX.WriteVerbose("UpdateDataStore Call BiographyReader.OpenNewestAsync", arguments: $"", category: nameof(App));
                BiographyReader.OpenNewestAsync(dirContentDef);
                TraceX.WriteVerbose("UpdateDataStore finished", category: nameof(App));
            }
            catch (Exception ex)
            {
                TraceX.WriteException("Exception at UpdateDataStore ", arguments: $"", category: nameof(App), exception: ex);
                Debugger.Break();
            }
        }

        private void BiographyReaderFinished()
        {
            TraceX.WriteVerbose("BiographyReader_ReadingFinished", category: nameof(App));
            DirectoryInfo dirContentDef = new DirectoryInfo(Path.Combine(LocalContentDataPath, @"ContentDefinition"));

            if (CreateJsonForWebsite)
            {
                Rootobject webData = Rootobject.FromBioStore(BiographyStore);

                Debug.WriteLine(LocalContentDataPath);
                Debug.WriteLine(ServerContentDataPath);

                FileInfo fileJsonWeg = new FileInfo(Path.Combine(LocalContentDataPath, "ContentDefinition.json"));
                webData.SaveToJsonFile(fileJsonWeg.FullName);

                Process.Start("explorer.exe", arguments: LocalContentDataPath);
            }
            TraceX.WriteVerbose("BiographyReader_ReadingFinished finished", category: nameof(App));
        }


        #endregion private Methoden

        #region EventHandler

        private async void ContentAutoUpdater_FileScanFinished(object sender, EventArgs e)
        {
            if (m_ContentAutoUpdater_FirstFileScanFinished)
            {
                await UpdateDataStore();
                m_ContentAutoUpdater_FirstFileScanFinished = false;
            }
        }

        private void ContentAutoUpdater_NewFilesFound(object sender, EventArgs e)
        {
            if (!m_ContentAutoUpdater_FirstFileScanFinished)
            {
                _ = UpdateDataStore();
            }
        }

        private void BiographyReader_ReassignStore(object sender, EventArgs e)
        {
            BiographyStore = BiographyReader.Store;
        }

        private void BiographyReader_Progress(object sender, Components.ClosedXml.Code.EventArguments.ProgressEventArgs e)
        {
            TraceX.WriteVerbose("BiographyReader_Progress", arguments: $"Message:{e?.Message},Index:{e?.Index},ID:{e?.Id}", category: nameof(App));
        }

        private void BiographyReader_ReadingFinished(object sender, Components.ClosedXml.Code.EventArguments.ReadingFinishedEventArgs e)
        {
            BiographyReaderFinished();
        }


        private void MainWindows_Closed(object sender, EventArgs e)
        {
            IsMainWindowClosed = true;
        }


        #endregion EventHandler
    }
}
