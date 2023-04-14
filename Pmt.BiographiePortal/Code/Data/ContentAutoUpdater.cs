using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Components.Common.Tracing;
using Gemelo.Components.FileUpdate;

namespace Gemelo.Applications.Biographieportal.Code.Data
{
    public class ContentAutoUpdater
    {
        #region Private Variablen

        private bool m_IsInUpdate = false;


        #endregion Private Variablen

        #region Öffentliche Properties

        #endregion Öffentliche Properties

        #region Events


        public event EventHandler<EventArgs> NewFilesFound;

        protected void OnNewFilesFound()
        {
            NewFilesFound?.Invoke(this, new EventArgs());
        }


        public event EventHandler<EventArgs> FileScanFinished;

        protected void OnFileScanFinished()
        {
            FileScanFinished?.Invoke(this, new EventArgs());
        }


        #endregion Events

        #region Konstruktur und Initialisierungen

        public ContentAutoUpdater()
        {
            InitializeThings();
        }

        private void InitializeThings()
        {
        }

        #endregion Konstruktur und Initialisierungen

        #region Öffentliche Methoden

        #endregion Öffentliche Methoden

        #region Private Methoden

        public void DoUpdate()
        {
            if (!m_IsInUpdate)
            {
                m_IsInUpdate = true;
                try
                {
                    FileListSettings createListsettings = new FileListSettings();

                    FileListCreator flc = new FileListCreator(App.Current.ServerContentDataPath, createListsettings);
                    flc.Process();
                    var files = flc.GetDirectoryAndFiles();

                    if (!Directory.Exists(App.Current.LocalContentDataPath)) Directory.CreateDirectory(App.Current.LocalContentDataPath);

                    FileListUpdateSettings updateSettings = new FileListUpdateSettings
                    {
                        DeleteObsoleteFiles = true,
                        DeleteObsoleteDirectoriesAndFiles = true,
                        //TODO TR: Setting fehlt: RemoteReadOnlyFlag = true,
                        IgnoreDeleteExceptions = true,
                        IgnoreUpdateExceptions = true,
                    };
                    FileListUpdater flu = new FileListUpdater()
                    {
                        Settings = updateSettings,
                        SourceDirectory = App.Current.ServerContentDataPath,
                        DestinationDirectory = App.Current.LocalContentDataPath,
                    };
                    flu.Progress += Flu_Progress;

                    flu.StartUpdate(files);

                    if (flu.NewFilesCount > 0 || flu.UpdatedFilesCount > 0)
                    {
                        OnNewFilesFound();
                    }
                }
                catch (Exception exp)
                {
                    Debugger.Break();
                    TraceX.WriteHandledException("HE by DoUpdate", category: nameof(ContentAutoUpdater), exception: exp);
                }
                finally
                {
                    OnFileScanFinished();
                    m_IsInUpdate = false;
                }
            }
        }

        private void Flu_Progress(object sender, FileListProgressEventArgs e)
        {
            Debug.WriteLine(e.Element);
        }

        public void StartTask()
        {
            Task.Run(async () =>
            {
                while (!App.Current.IsMainWindowClosed)
                {
                    DoUpdate();
                    await Task.Delay(App.Current.CheckNewContentInterval);
                }
            });
        }

        #endregion Private Methoden

        #region Eventhandler

        #endregion Eventhandler


    }


}
