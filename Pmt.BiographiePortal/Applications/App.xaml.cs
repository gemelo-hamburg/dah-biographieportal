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

        //TODO: SMTP-Account anlegen und Passwort eintragen
        private const string ConstTraceSmtpServer = "smtp.gemelo.de";
        private const string ConstTraceSmtpUser = "auswandererhaus-trace-send@gemelo.de";
        private const string ConstTraceSmtpPassword = "79nygn18Psx)";
        private const string ConstTraceSmtpSender = "auswandererhaus-trace-send@gemelo.de";
        private const string ConstTraceSmtpRecipient = "auswandererhaus-trace-receive@gemelo.de";

        private const string ConstGlobalSettingsFilename = @"Data\GlobalSettings.json";


        public const string ConstDataRootDirectoryName = "Data";
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

        //public new MainWindow MainWindow { get; private set; }
        //public TimeSpan RestartInterval { get; internal set; }
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

            InitDataStore();

            LocalizationExtensions.UseVisualTree = true;
            LocalizationExtensions.UseLogicalTree = true;
        }

        protected override async Task UpdateStationDefinitionInternally()
        {
            TraceX.WriteInformational("UpdateStationDefinitionInternally", category: nameof(App));
            string stationJsonFilePath =
                Path.Combine(DataDirectoryPath, ConstStationDefinitionFilename);

            //TODO: Löschen der StationDef rausnehmen wenn CMS läuft

            //if (File.Exists(stationJsonFilePath))
            //{
            //    File.Delete(stationJsonFilePath);
            //    await Task.Delay(500);
            //}

            if (!File.Exists(stationJsonFilePath))
            {
                StationDefinition = new BioportalStationDefinition();
                StationDefinition.SaveToJsonFile(stationJsonFilePath);
                await Task.Delay(250);
            }

            StationDefinition =
                BioportalStationDefinition.LoadFromJsonFile(stationJsonFilePath);

            InitializeDataPaths();

            //CreateWindowViewModels();
            //CreateEntryViewModels();

            //AddDebugLog("Station Definition geladen");
        }

        private void InitDataStore()
        {
            BiographyStore = new BiographyStore();
            BiographyReader = new BiographyReader();
            BiographyReader.ReassignStore += BiographyReader_ReassignStore;
        }

        //        private void SetDefaultTraceMailSettings()
        //        {
        //            TraceX.WriteImportant("SetDefaultTraceMailSettings() ...", ConstTraceCategory);
        //            TraceX.ApplicationName = ApplicationName;
        //            //TraceX.SmtpServer = ConstTraceSmtpServer;
        //            //TraceX.SmtpAuthUser = ConstTraceSmtpUser;
        //            //TraceX.SmtpAuthPassword = ConstTraceSmtpPassword;
        //            //TraceX.SmtpFrom = ConstTraceSmtpSender;
        //            //TraceX.SmtpReceipientsInfo = new string[] { ConstTraceSmtpRecipient };
        //            //TraceX.SmtpReceipientsWarning = new string[] { ConstTraceSmtpRecipient };
        //            //TraceX.SmtpReceipientsException = new string[] { ConstTraceSmtpRecipient };
        //#if (DEBUG)
        //            //TraceX.SmtpSendTraceMails = false;
        //#else
        //            //TraceX.SmtpSendTraceMails = true;
        //#endif
        //}

        #endregion Konstruktur und Initialisierung

        #region öffentliche Methoden

        #endregion öffentliche Methoden

        #region protected Methoden

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

        private string GetPathFromCommandLineArg(string arg)
        {
            int firstQuationMark = arg.IndexOf(':');
            if (firstQuationMark > 0)
            {
                string path = arg.SecureSubstring(firstQuationMark + 1);
                if (path.StartsWith("\"")) path = path.SecureSubstring(1);
                if (path.EndsWith("\"")) path = path.SecureSubstring(0, path.Length - 1);
                return path;
            }
            else return null;
        }

        //protected override void InitializeBeforeMainWindow()
        //{
        //    base.InitializeBeforeMainWindow();
        //    try
        //    {
        //        //TouchButton.FireClickAtTouchDownGlobal = true;
        //    }
        //    catch (Exception exp)
        //    {
        //        TraceX.WriteException($"BeforeMainWindowCreate", $"{nameof(App)}", exception: exp);
        //        throw;
        //    }
        //}

        //protected override ExhibitMainWindow CreateMainWindow()
        //{
        //    MainWindow = new MainWindow();
        //    return MainWindow;
        //}

        protected override ExhibitMainWindow CreateMainWindow()
        {
            var result = base.CreateMainWindow();
            result.Closed += MainWindows_Closed;
            return result;
        }


        private void MainWindows_Closed(object sender, EventArgs e)
        {
            IsMainWindowClosed = true;
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

            //SetDefaultTraceMailSettings();
            //LocalizationString.DefaultLanguage = DefaultLanguage;

            //TextBlockExtension.UseRecursiveVersion = true;

            InitializeContentData();
        }

        private void InitializeContentData()
        {
            ContentAutoUpdater = new ContentAutoUpdater();
            ContentAutoUpdater.NewFilesFound += ContentAutoUpdater_NewFilesFound;
            ContentAutoUpdater.FileScanFinished += ContentAutoUpdater_FileScanFinished;
            //ReadContent();
            ContentAutoUpdater.StartTask();
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

        public DirectoryInfo EnsureAppDataDirectory(string folderName)
        {
            return EnsureDirectory(Directories.AppDataDirectory, folderName);
        }

        public DirectoryInfo EnsureLocalContentDataDirectory(string folderName)
        {
            return EnsureDirectory(LocalContentDataPath, folderName);
        }

        private DirectoryInfo EnsureDirectory(DirectoryInfo baseFolder, string folderName)
        {
            return EnsureDirectory(baseFolder.FullName, folderName);
        }

        private DirectoryInfo EnsureDirectory(string baseFolder, string folderName)
        {
            DirectoryInfo result = new DirectoryInfo(Path.Combine(baseFolder, folderName));
            if (!result.Exists)
            {
                result.Create();
                result.Refresh();
            }
            return result;
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

        #endregion protected Methoden

        #region private Methoden

        private async Task UpdateDataStore()
        {
            //DirectoryInfo dirMedia = new DirectoryInfo(Path.Combine(Directories.ApplicationProgramDirectory, @"Data\Media"));
            //DirectoryInfo dirContentDef = new DirectoryInfo(Path.Combine(Directories.ApplicationProgramDirectory, @"Data\ContentDefinition"));

            DirectoryInfo dirMedia = EnsureLocalContentDataDirectory(ConstMediaDirectoryName);// new DirectoryInfo(Path.Combine(LocalContentDataPath, @"Media"));
            DirectoryInfo dirContentDef = EnsureLocalContentDataDirectory(ConstContentDefinitionDirectoryName); //new DirectoryInfo(Path.Combine(LocalContentDataPath, @"ContentDefinition"));

            BiographyReader.Store = new BiographyStore();
            BiographyReader.ReadingFinished += BiographyReader_ReadingFinished;
            await BiographyReader.AnalyseMediaDirectory(dirMedia);
            BiographyReader.OpenNewestAsync(dirContentDef);
        }

        private void BiographyReader_ReadingFinished(object sender, Components.ClosedXml.Code.EventArguments.ReadingFinishedEventArgs e)
        {
            DirectoryInfo dirContentDef = new DirectoryInfo(Path.Combine(LocalContentDataPath, @"ContentDefinition"));

            //if (CreateJson)
            //{
            //    FileInfo fileJson = new FileInfo(Path.Combine(LocalContentDataPath, "ContentDefinition.json"));
            //    BiographyStore.SaveToJsonFile(fileJson.FullName);
            //}

            if (CreateJsonForWebsite)
            {
                Rootobject webData = Rootobject.FromBioStore(BiographyStore);

                Debug.WriteLine(LocalContentDataPath);
                Debug.WriteLine(ServerContentDataPath);

                FileInfo fileJsonWeg = new FileInfo(Path.Combine(LocalContentDataPath, "ContentDefinition.json"));
                webData.SaveToJsonFile(fileJsonWeg.FullName);

                Process.Start("explorer.exe", arguments: LocalContentDataPath);
            }
        }


        //private void ReadContent()
        //{
        //    DirectoryInfo contentDir = new DirectoryInfo(Path.Combine(Directories.ApplicationProgramDirectory, @"Data\ContentDefintion"));
        //    BiographyReader.OpenNewestAsync(contentDir);
        //    //BiographyReader.Open(file);
        //}


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


        #endregion EventHandler
    }
}
