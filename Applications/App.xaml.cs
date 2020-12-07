using Gemelo.Applications.Biographieportal.Code.Data;
using Gemelo.Applications.Biographieportal.Properties;
using Gemelo.Applications.Biographieportal.Windows;
using Gemelo.Components.Exhibits.App;
using Gemelo.Components.Exhibits.Controls;

using Pete.Components.Extensions.Classes;
using Pete.Components.Extensions.Extensions;
using Pete.Components.Extensions.Settings;
using Pete.Components.Extensions.Tracing;
using Pete.Components.WpfExtensions.Controls;
using Pete.Components.WpfExtensions.Extensions;
using Pete.Components.WpfExtensions.Localization;

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
    * 
    * 
    * 
    * 
    * 
     */



    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : LocalizationExhibitApplication
    {
        #region Konstanten

        public const string ConstTraceCategory = "Biographieportal";

        public const string LanguageEn = "en"; // Englisch
        public const string LanguageDe = "de"; // Deutsch

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
        public const string ConstMediaDirectoryName = "Media";
        public const string ConstInfoDirectoryName = "Info";

        #endregion Konstanten

        #region private Member

        private bool m_ContentAutoUpdater_FirstFileScanFinished = true;

        #endregion private Member

        #region öffentliche Properties

        public bool WaitIfContentReadingError { get; private set; }
        public TimeSpan CategorySliderTimeout { get; private set; }
        public string VisitorMessagesPath { get; private set; }
        public string ServerContentDataPath { get; private set; }
        public string LocalContentDataPath { get; private set; }
        public TimeSpan CheckNewContentInterval { get; private set; }

        protected Stopwatch m_TimeStopwatch;

        public double Time => m_TimeStopwatch.ElapsedMilliseconds;

        public override string ApplicationName => "Biographieportal";

        public override List<string> AvailableLanguages
        {
            get { return ConstAvailableLanguages; }
        }

        public override string DefaultLanguage
        {
            get { return ConstDefaultLanguage; }
        }

        public override string[] FontFamiliesPrerequisite
        {
            get
            {
                List<string> fonts = new List<string>();
                fonts.AddRange(base.FontFamiliesPrerequisite);
                fonts.AddRange(ConstFontFamiliesPrerequisite);
                return fonts.ToArray();
            }
        }


        private static App s_Current;
        public new static App Current
        {
            get
            {
                if (s_Current == null) s_Current = Application.Current as App;
                return s_Current;
            }
        }

        public new MainWindow MainWindow { get; private set; }
        public TimeSpan RestartInterval { get; internal set; }

        public BiographyReader BiographyReader { get; private set; }
        public BiographyStore BiographyStore { get; private set; }
        public ContentAutoUpdater ContentAutoUpdater { get; private set; }

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
            // BeforeMainWindowCreate wird vorher aufgerufen
        }

        private void InitializeThings()
        {
            LocalizationExtension.UseVisualTree = true;
            LocalizationExtension.UseLogicalTree = true;

            RestartInterval = Settings.Default.RestartInterval;
            WaitIfContentReadingError = Settings.Default.WaitIfContentReadingError;
            CategorySliderTimeout = Settings.Default.CategorySliderTimeout;
            CheckNewContentInterval = Settings.Default.CheckNewContentInterval;
            
            if (string.IsNullOrWhiteSpace(VisitorMessagesPath)) VisitorMessagesPath = Settings.Default.VisitorMessagesPath;
            if (string.IsNullOrWhiteSpace(ServerContentDataPath)) ServerContentDataPath = Settings.Default.ContentDataPath;
            
            LocalContentDataPath = Path.Combine(Directories.LocalApplicationData.FullName, "ContentData");
            if (!Directory.Exists(LocalContentDataPath)) Directory.CreateDirectory(LocalContentDataPath);

            TraceX.WriteImportant("VisitorMessagesPath", category: "App", argument: VisitorMessagesPath); // neu ab 1.302
            TraceX.WriteImportant("LocalContentDataPath", category: "App", argument: LocalContentDataPath); // neu ab 1.302
            TraceX.WriteImportant("ServerContentDataPath", category: "App", argument: ServerContentDataPath); // neu ab 1.302

#if (DEBUG)
            WaitIfContentReadingError = true;
            RestartInterval = TimeSpan.FromSeconds(40);
            CategorySliderTimeout = TimeSpan.FromSeconds(20);
            //VisitorMessagesPath = @"c:\temp\dah\newmessages";
            //ServerContentDataPath = @"C:\Temp\g0498 biographie\Daten 2020-07-23\contentdata";
            ServerContentDataPath = @"C:\+dev\Expo\Allgemein\G0498 Biographieportal\ContentData";
#endif
            m_TimeStopwatch = new Stopwatch();
            m_TimeStopwatch.Start();
            SetDefaultTraceMailSettings();
            LocalizationString.DefaultLanguage = DefaultLanguage;

            TextBlockExtension.UseRecursiveVersion = true;

            ContentAutoUpdater = new ContentAutoUpdater();
            ContentAutoUpdater.NewFilesFound += ContentAutoUpdater_NewFilesFound;
            ContentAutoUpdater.FileScanFinished += ContentAutoUpdater_FileScanFinished;

            InitDataStore();
        }

        private void InitDataStore()
        {
            BiographyStore = new BiographyStore();
            BiographyReader = new BiographyReader();
            BiographyReader.ReassignStore += BiographyReader_ReassignStore;
        }

        private void SetDefaultTraceMailSettings()
        {
            TraceX.WriteLine(TraceXLevel.Important, "SetDefaultTraceMailSettings() ...", ConstTraceCategory);
            TraceX.ApplicationName = ApplicationName;
            TraceX.SmtpServer = ConstTraceSmtpServer;
            TraceX.SmtpAuthUser = ConstTraceSmtpUser;
            TraceX.SmtpAuthPassword = ConstTraceSmtpPassword;
            TraceX.SmtpFrom = ConstTraceSmtpSender;
            TraceX.SmtpReceipientsInfo = new string[] { ConstTraceSmtpRecipient };
            TraceX.SmtpReceipientsWarning = new string[] { ConstTraceSmtpRecipient };
            TraceX.SmtpReceipientsException = new string[] { ConstTraceSmtpRecipient };
#if (DEBUG)
            TraceX.SmtpSendTraceMails = false;
#else
            TraceX.SmtpSendTraceMails = true;
#endif
        }

        #endregion Konstruktur und Initialisierung

        #region öffentliche Methoden

        #endregion öffentliche Methoden

        #region protected Methoden

        public override string CommandLineHelpString
        {
            get
            {
                string baseString = base.CommandLineHelpString;

                baseString += "\n-servercontentdatapath:\"<Path>\" : Path to the Server ContentData\n\n";
                baseString += "-visitormessagespath:\"<Path>\" : Path to the Server Visitor Messages\n\n";

                return baseString;
            }
        }

        /// <summary>
        /// -servercontentdatapath:"y:\content" -visitormessagespath:"y:\Messages"
        /// </summary>
        /// <param name="argOriginal"></param>
        /// <param name="argWithoutLeadingSwitch"></param>
        /// <returns></returns>
        protected override bool CheckUnkownCommandLineSwitch(string argOriginal, string argWithoutLeadingSwitch, ref string errorHelp)
        {
            argWithoutLeadingSwitch = argWithoutLeadingSwitch.ToLower();
            if (argWithoutLeadingSwitch.StartsWith("servercontentdatapath"))
            {
                ServerContentDataPath = GetPathFromCommandLineArg(argWithoutLeadingSwitch);
                if (!string.IsNullOrWhiteSpace(ServerContentDataPath)) return true;
            }
            else if (argWithoutLeadingSwitch.StartsWith("visitormessagespath"))
            {
                VisitorMessagesPath = GetPathFromCommandLineArg(argWithoutLeadingSwitch);
                if (!string.IsNullOrWhiteSpace(VisitorMessagesPath)) return true;
            }

            return base.CheckUnkownCommandLineSwitch(argOriginal, argWithoutLeadingSwitch);
        }

        private string GetPathFromCommandLineArg(string arg)
        {
            int firstQuationMark = arg.IndexOf(':');
            if(firstQuationMark>0)
            {
                string path = arg.SecureSubstring(firstQuationMark + 1);
                if (path.StartsWith("\"")) path = path.SecureSubstring(1);
                if (path.EndsWith("\"")) path = path.SecureSubstring(0, path.Length - 1);
                return path;
            }
            else return null;
        }

        protected override void BeforeMainWindowCreate()
        {
            base.BeforeMainWindowCreate();
            try
            {
                //TouchButton.FireClickAtTouchDownGlobal = true;
                InitializeThings();
            }
            catch (Exception exp)
            {
                TraceX.WriteException($"BeforeMainWindowCreate", $"{nameof(App)}", exception: exp);
                throw;
            }
        }

        protected override ExhibitMainWindow CreateMainWindow()
        {
            MainWindow = new MainWindow();
            return MainWindow;
        }

        protected override void AfterMainWindowCreate()
        {
            base.AfterMainWindowCreate();

            //ReadContent();
            ContentAutoUpdater.StartTask();

        }

        #endregion protected Methoden

        #region private Methoden

        private async Task UpdateDataStore()
        {
            //DirectoryInfo dirMedia = new DirectoryInfo(Path.Combine(Directories.ApplicationProgramDirectory, @"Data\Media"));
            //DirectoryInfo dirContentDef = new DirectoryInfo(Path.Combine(Directories.ApplicationProgramDirectory, @"Data\ContentDefinition"));

            DirectoryInfo dirMedia = new DirectoryInfo(Path.Combine(LocalContentDataPath, @"Media"));
            DirectoryInfo dirContentDef = new DirectoryInfo(Path.Combine(LocalContentDataPath, @"ContentDefinition"));

            BiographyReader.Store = new BiographyStore();
            await BiographyReader.AnalyseMediaDirectory(dirMedia);
            BiographyReader.OpenNewestAsync(dirContentDef);
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
                UpdateDataStore();
            }
        }

        private void BiographyReader_ReassignStore(object sender, EventArgs e)
        {
            BiographyStore = BiographyReader.Store;
        }


        #endregion EventHandler
    }
}
