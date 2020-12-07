using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Properties;
using Pete.Components.Extensions.Extensions;
using Pete.Components.Extensions.Settings;
using Pete.Components.Extensions.Tracing;
using Pete.Components.WpfExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Gemelo.Applications.Biographieportal.Controls.SendOwnStories
{
    /// <summary>
    /// Interaktionslogik für SendOwnStory.xaml
    /// </summary>
    public partial class SendOwnStory : UserControl
    {
        #region private Member

        private string m_VisitorMessagesPath;

        #endregion private Member

        #region ctor

        public SendOwnStory()
        {
            InitializeComponent();

            //m_EditStory.m_Edit.MinLines = 4;
            //m_EditStory.m_Edit.MaxLines = 4;

            m_EditEmail.AllowEnter = false;
            m_EditFirstname.AllowEnter = false;
            m_EditLastname.AllowEnter = false;
            m_EditWhen.AllowEnter = true;
            m_EditWhy.AllowEnter = true;

            ClearTextboxes();
        }

        #endregion ctor

        #region öffentliche Methoden

        public async Task ShowUp()
        {
            this.FadeInIfNotVisible();

            App.Current.ExhibitMainWindow.RestartTimerInterval = Settings.Default.RestartInterval_SendOwnStory;
            App.Current.ExhibitMainWindow.ResetRestartTimer();

            m_PanelSuccess.StopWpfEffectAndHide();
            m_EditFirstname.Focus();

            await Task.Delay(400);
            m_Keyboard.ShowUp();

            m_Keyboard.SetConnectedTextBox(m_EditFirstname.m_Edit, m_EditFirstname);

            ClearTextboxes();

            m_CheckAccept.IsChecked = false;

            m_BtnClose.IsEnabled = true;
            m_Keyboard.IsEnabled = true;

            m_TxtEnterNames.Hide();
            m_TxtEnterEmail.Hide();
            m_TxtEnterWhen.Hide();
            m_TxtEnterWhy.Hide();
            m_TxtEnterAcceptImpressum.Hide();

            UpdateUi();
        }

        private void ClearTextboxes()
        {
            m_EditFirstname.Text = string.Empty;
            m_EditLastname.Text = string.Empty;
            m_EditEmail.Text = string.Empty;
            m_EditWhen.Text = string.Empty;
            m_EditWhy.Text = string.Empty;
        }

        public void Close()
        {
            App.Current.ExhibitMainWindow.RestartTimerInterval = App.Current.RestartInterval;
            App.Current.MainWindow.ResetRestartTimer();

            this.FadeOutIfVisible();
            m_Keyboard.HideDown();
        }

        #endregion öffentliche Methoden

        #region private Methoden

        private void UpdateUi()
        {
            if (App.Current?.MainWindow != null)
            {
                App.Current.MainWindow.ResetRestartTimer();
                if (m_BtnSend != null)
                {
                    if (IsNameOkay() &&
                        IsEmailOkay() &&
                        IsWhyOkay() &&
                        IsWhenOkay() &&
                        IsAcceptOkay())
                    {
                        m_BtnSend.IsEnabled = true;
                        m_BtnSend.Opacity = 1.0;
                    }
                    else
                    {
                        m_BtnSend.IsEnabled = false;
                        m_BtnSend.Opacity = 0.5;
                    }
                }
            }
        }


        private async Task Send()
        {
            m_BtnSend.IsEnabled = false;
            m_Keyboard.IsEnabled = false;
            m_Keyboard.HideDown();
            m_BtnClose.IsEnabled = false;

            m_PanelSuccess.FadeInIfNotVisible();

            CreateMessage();

            await Task.Delay(Settings.Default.SendOwnStory_SuccessMessageDuration);
            m_PanelSuccess.FadeInIfNotVisible();
            m_PanelSuccess.FadeOutIfVisible();
            Close();

            ClearTextboxes();

        }

        private void CreateMessage()
        {
            try
            {
                StringBuilder message = new StringBuilder();
                DateTime now = DateTime.Now;
                message.AppendLine("Deutsches Auswandererhaus - Biographieportal");
                message.AppendLine("Benutzeranmeldung einer eigenen Geschichte");
                message.AppendLine("ACHTUNG: Vertraulicher Inhalt! Datenschutzbestimmung beachten!");
                message.AppendLine("");
                message.AppendLine("Zeitstempel:");
                message.AppendLine(now.ToString());
                message.AppendLine("Sprache:");
                message.AppendLine(App.Current.CurrentLanguage);
                message.AppendLine("");
                message.AppendLine("Name:");
                message.AppendLine($"{m_EditFirstname.Text} {m_EditLastname.Text}");
                message.AppendLine("Email:");
                message.AppendLine($"{m_EditEmail.Text}");
                message.AppendLine("");
                message.AppendLine("Warum sind Sie bzw. warum ist Ihre Familie ein- oder ausgewandert?");
                message.AppendLine($"{m_EditWhy.Text}");
                message.AppendLine("Wann sind Sie bzw. wann ist Ihre Familie ein- oder ausgewandert?");
                message.AppendLine($"{m_EditWhen.Text}");
                message.AppendLine("");
                message.AppendLine("");
                message.AppendLine("Bitte kontaktieren Sie den Besucher zeitnah.");
                message.AppendLine("");

                CheckMessageFolder();

                string fileName = Path.Combine(m_VisitorMessagesPath, now.ToFileName("txt"));
                File.WriteAllText(fileName, message.ToString());
            }
            catch (Exception exp)
            {
                TraceX.WriteHandledException("HE at CreateMessage", category: "SendOwnStory.xaml", exception: exp);
            }
        }

        private void CheckMessageFolder()
        {
            bool useLocalMessagesPath = false;
            try
            {
                m_VisitorMessagesPath = App.Current.VisitorMessagesPath;
                if (!Directory.Exists(m_VisitorMessagesPath)) Directory.CreateDirectory(m_VisitorMessagesPath);
                Thread.Sleep(200);
                if (!Directory.Exists(m_VisitorMessagesPath)) useLocalMessagesPath = true;
            }
            catch (Exception exp)
            {
                TraceX.WriteHandledException("HE beim erzeugen des VisitorMessagesPath", category: "App", argument: m_VisitorMessagesPath, exception: exp);
                useLocalMessagesPath = true;
            }
            if (useLocalMessagesPath)
            {
                m_VisitorMessagesPath = Path.Combine(Directories.LocalApplicationData.FullName, "newmessages");
                if (!Directory.Exists(m_VisitorMessagesPath)) Directory.CreateDirectory(m_VisitorMessagesPath);
            }
        }

        private bool IsNameOkay()
        {
            return !string.IsNullOrWhiteSpace(m_EditFirstname.Text) || !string.IsNullOrWhiteSpace(m_EditLastname.Text);
        }

        private bool IsEmailOkay()
        {
            return !string.IsNullOrWhiteSpace(m_EditEmail.Text) && EmailValidator.IsValidEmail(m_EditEmail.Text);
        }
        private bool IsWhyOkay()
        {
            return !string.IsNullOrWhiteSpace(m_EditWhy.Text);
        }
        private bool IsWhenOkay()
        {
            return !string.IsNullOrWhiteSpace(m_EditWhen.Text);
        }
        private bool IsAcceptOkay()
        {
            return m_CheckAccept.IsChecked.ToBool();
        }


        #endregion private Methoden

        #region EventHandler

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }



        private void TxtImpressum_MouseDown(object sender, MouseButtonEventArgs e)
        {
            App.Current.MainWindow.ShowImpressum();
        }

        private void TxtImpressum_TouchDown(object sender, TouchEventArgs e)
        {
            App.Current.MainWindow.ShowImpressum();
        }

        private void CheckAccept_Checked(object sender, RoutedEventArgs e)
        {
            if (IsAcceptOkay()) m_TxtEnterAcceptImpressum.Hide();
            UpdateUi();
        }

        private void CheckAccept_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.ShowUp();
            UpdateUi();
        }

        private void BtnSend_Clicked(object sender, EventArgs e)
        {
            Send();
        }

        private void EditBox_EditFocused(object sender, EditBox.EditFocusedEventArgs e)
        {
            App.Current.MainWindow.ResetRestartTimer();
            m_Keyboard.SetConnectedTextBox(e.TextBox, e.EditBox);

            if (e.EditBox == m_EditEmail) GiveHelp(m_EditLastname);
            else if (e.EditBox == m_EditWhy) GiveHelp(m_EditEmail);
            else if (e.EditBox == m_EditWhen) GiveHelp(m_EditWhy);
        }


        private void EditWhen_EditChanged(object sender, EditBox.EditLostFocusEventArgs e)
        {
            GiveHelp(e.EditBox);
        }

        private void EditBox_EditLostFocus(object sender, EditBox.EditLostFocusEventArgs e)
        {
            GiveHelp(e.EditBox);
        }

        private void GiveHelp(EditBox editBox)
        {
            if (editBox == m_EditLastname)
            {
                if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
            }
            else if (editBox == m_EditEmail)
            {
                if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
                if (!IsEmailOkay()) m_TxtEnterEmail.ShowUp();
                //if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.FadeInIfNotVisible();
            }
            else if (editBox == m_EditWhy)
            {
                if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
                if (!IsEmailOkay()) m_TxtEnterEmail.ShowUp();
                if (!IsWhyOkay()) m_TxtEnterWhy.ShowUp();
                //if (!IsWhenOkay()) m_TxtEnterWhen.ShowUp();
                //if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.ShowUp();
            }
            else if (editBox == m_EditWhen)
            {
                if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
                if (!IsEmailOkay()) m_TxtEnterEmail.ShowUp();
                if (!IsWhyOkay()) m_TxtEnterWhy.ShowUp();
                if (!IsWhenOkay()) m_TxtEnterWhen.ShowUp();
                //if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.ShowUp();
            }

            if (IsNameOkay()) m_TxtEnterNames.HideDown();
            if (IsEmailOkay()) m_TxtEnterEmail.HideDown();
            if (IsWhyOkay()) m_TxtEnterWhy.HideDown();
            if (IsWhenOkay()) m_TxtEnterWhen.HideDown();
            if (IsAcceptOkay()) m_TxtEnterAcceptImpressum.HideDown();

            UpdateUi();
        }

        #endregion EventHandler

    }
}
