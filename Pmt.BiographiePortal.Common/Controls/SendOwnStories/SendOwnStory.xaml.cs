using Gemelo.Applications.Pmt.BiographiePortal.Controls.SendOwnStories;
using Gemelo.Components.Common.Base;
using Gemelo.Components.Common.Exhibits.Settings;
using Gemelo.Components.Common.Localization;
using Gemelo.Components.Common.Settings;
using Gemelo.Components.Common.Text;
using Gemelo.Components.Common.Tracing;
using Gemelo.Components.Common.Wpf.UI.Transitions;
using Gemelo.Components.Common.Wpf.UI.Transitions.Appearance;

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

        #region öffentliche Eigenschaften

        //public TimeSpan RestartInterval_Dialog { get; set; }
        //public TimeSpan RestartInterval_Default { get; set; }
        public string VisitorMessagesPath { get; set; }

        #endregion öffentliche Eigenschaften

        #region Events


        public event EventHandler<EventArgs> HidingDown;

        protected void OnHidingDown()
        {
            HidingDown?.Invoke(this, new EventArgs());
        }


        #endregion Events

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

            m_Impressum.Visibility = Visibility.Collapsed;
            ClearTextboxes();
        }

        #endregion ctor

        #region öffentliche Methoden

        public async Task ShowUp()
        {
            this.FadeInIfNotVisible();
            IsHitTestVisible = true;

            //RestartTimer.Interval = RestartInterval_Dialog; // App.Current.StationDefinition.RestartInterval_SendOwnStory;
            //App.Current.ExhibitMainWindow.RestartTimerInterval = Settings.Default.RestartInterval_SendOwnStory;
            RestartTimer.Restart();

            m_PanelSuccess.StopTransitionAndHide();
            m_EditFirstname.Focus();

            await Task.Delay(400);
            m_Keyboard.ShowUp();

            m_Keyboard.SetConnectedTextBox(m_EditFirstname.m_Edit, m_EditFirstname);

            ClearTextboxes();
            m_ComboInOrOut.SelectedData = null;
            m_ComboGift.SelectedData = null;
            m_ComboWho.SelectedData = null;

            m_CheckAccept.IsChecked = false;

            m_BtnClose.IsEnabled = true;
            m_Keyboard.IsEnabled = true;

            m_Impressum.HideDown();

            //m_TxtEnterNames.Hide();
            //m_TxtEnterEmail.Hide();
            //m_TxtEnterWhen.Hide();
            //m_TxtEnterWhy.Hide();
            //m_TxtEnterAcceptImpressum.Hide();

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

        public void HideDown()
        {
            IsHitTestVisible = false;
            //RestartTimer.Interval = RestartInterval_Default;
            //App.Current.ExhibitMainWindow.RestartTimerInterval = App.Current.RestartInterval;
            RestartTimer.Restart();
            ClearTextboxes();
            this.FadeOutIfVisible();
            //m_Keyboard.HideDown();
            OnHidingDown();
        }

        #endregion öffentliche Methoden

        #region private Methoden

        private void UpdateUi()
        {
            RestartTimer.Restart();
            if (m_BtnSend != null)
            {
                if (IsAllDataOkay())
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

        private bool IsAllDataOkay()
        {
            return
                IsNameOkay() &&
                IsEmailOkay() &&
                IsWhyOkay() &&
                IsWhenOkay() &&
                IsComboInOutOkay() &&
                IsComboWhoOkay() &&
                IsComboGiftOkay() &&
                IsAcceptOkay();
        }

        private async Task Send()
        {
            if (IsAllDataOkay())
            {
                m_BtnSend.IsEnabled = false;
                m_Keyboard.IsEnabled = false;
                m_Keyboard.HideDown();
                m_BtnClose.IsEnabled = false;

                m_PanelSuccess.FadeInIfNotVisible();

                await Task.Delay(1000);

                CreateMessage();

                //await Task.Delay(App.Current.StationDefinition.SendOwnStory_SuccessMessageDuration);
                //m_PanelSuccess.FadeInIfNotVisible();
                //m_PanelSuccess.FadeOutIfVisible();
                //Close();

                //ClearTextboxes();
            }
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
                message.AppendLine(Languages.Current);
                message.AppendLine("");
                message.AppendLine("Name:");
                message.AppendLine($"{m_EditFirstname.Text} {m_EditLastname.Text}");
                message.AppendLine("Email:");
                message.AppendLine($"{m_EditEmail.Text}");
                message.AppendLine("");
                message.AppendLine("Warum sind Sie bzw. warum ist Ihre Familie ein- oder ausgewandert?");
                message.AppendLine($"{m_EditWhy.Text}");
                message.AppendLine("");
                message.AppendLine("Wann sind Sie bzw. wann ist Ihre Familie ein- oder ausgewandert?");
                message.AppendLine($"{m_EditWhen.Text}");
                message.AppendLine("");
                message.AppendLine("Handelt es sich um eine Einwanderungsgeschichte oder eine Auswanderungsgeschichte?");
                message.AppendLine($"{GetComboText(m_ComboInOrOut)}");
                message.AppendLine("");
                message.AppendLine("Wer ist ein- bzw. ausgewandert?");
                message.AppendLine($"{GetComboText(m_ComboWho)}");
                message.AppendLine("");
                message.AppendLine("Können Sie sich vorstellen, dem Deutschen Auswandererhaus Erinnerungsobjekte oder Dokumente für seine Sammlung zu schenken?");
                message.AppendLine($"{GetComboText(m_ComboGift)}");
                message.AppendLine("");

                message.AppendLine("");
                message.AppendLine("Bitte kontaktieren Sie den Besucher zeitnah.");
                message.AppendLine("");

                CheckMessageFolder();

                string fileName = $"{GetEwOrAw()}_{now.ToFileName(extension: "", separator: "-")}_{GetLastFirstNameAsFileName()}.txt";

                string fullFileName = Path.Combine(m_VisitorMessagesPath, fileName);
                File.WriteAllText(fullFileName, message.ToString());
            }
            catch (Exception exp)
            {
                TraceX.WriteHandledException("HE at CreateMessage", category: "SendOwnStory.xaml", exception: exp);
            }
        }

        private string GetComboText(SosComboBox combo)
        {
            return combo.SelectedData?.Text["de"];
        }

        private string GetLastFirstNameAsFileName()
        {
            string result = $"{m_EditLastname.Text}_{m_EditFirstname.Text}";
            result = result.SecureSubstring(0, 15);
            result = ReplaceInvalidChars(result);
            return result;
        }

        private const string ValidFileNameChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ01234567890_-";

        public string ReplaceInvalidChars(string filename)
        {
            filename = filename
                .Replace("ä", "ae")
                .Replace("ü", "ue")
                .Replace("ö", "oe")
                .Replace("Ä", "Ae")
                .Replace("Ü", "Ue")
                .Replace("Ö", "Oe")
                .Replace("ß", "ss");

            StringBuilder sb = new StringBuilder();

            foreach (char c in filename)
            {
                if (ValidFileNameChars.Contains(c)) sb.Append(c);
            }

            return sb.ToString();
        }

        private string GetEwOrAw()
        {
            if (m_ComboInOrOut.SelectedData?.Id == 0) return "AW";
            else if (m_ComboInOrOut.SelectedData?.Id == 1) return "EW";
            else return "SO";
        }


        private void CheckMessageFolder()
        {
            bool useLocalMessagesPath = false;
            try
            {
                m_VisitorMessagesPath = VisitorMessagesPath;// App.Current.VisitorMessagesPath;
                if (!Directory.Exists(m_VisitorMessagesPath)) Directory.CreateDirectory(m_VisitorMessagesPath);
                Thread.Sleep(200);
                if (!Directory.Exists(m_VisitorMessagesPath)) useLocalMessagesPath = true;
            }
            catch (Exception exp)
            {
                TraceX.WriteHandledException("HE beim erzeugen des VisitorMessagesPath", category: "App", arguments: m_VisitorMessagesPath, exception: exp);
                useLocalMessagesPath = true;
            }
            if (useLocalMessagesPath)
            {
                m_VisitorMessagesPath = Path.Combine(Directories.AppDataDirectory.FullName, "newmessages");
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

        private bool IsComboInOutOkay()
        {
            return m_ComboInOrOut.SelectedData != null;
        }

        private bool IsComboWhoOkay()
        {
            return m_ComboWho.SelectedData != null;
        }

        private bool IsComboGiftOkay()
        {
            return m_ComboGift.SelectedData != null;
        }

        private void GiveHelp(FrameworkElement editBox)
        {
            UpdateUi();
            //if (m_TxtEnterNames != null)
            //{

            //    if (editBox == m_EditLastname)
            //    {
            //        if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
            //    }
            //    else if (editBox == m_EditEmail)
            //    {
            //        if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
            //        if (!IsEmailOkay()) m_TxtEnterEmail.ShowUp();
            //        //if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.FadeInIfNotVisible();
            //    }
            //    else if (editBox == m_EditWhy)
            //    {
            //        if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
            //        if (!IsEmailOkay()) m_TxtEnterEmail.ShowUp();
            //        if (!IsWhyOkay()) m_TxtEnterWhy.ShowUp();
            //        //if (!IsWhenOkay()) m_TxtEnterWhen.ShowUp();
            //        //if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.ShowUp();
            //    }
            //    else if (editBox == m_EditWhen)
            //    {
            //        if (!IsNameOkay()) m_TxtEnterNames.ShowUp();
            //        if (!IsEmailOkay()) m_TxtEnterEmail.ShowUp();
            //        if (!IsWhyOkay()) m_TxtEnterWhy.ShowUp();
            //        if (!IsWhenOkay()) m_TxtEnterWhen.ShowUp();
            //        //if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.ShowUp();
            //    }

            //    if (IsNameOkay()) m_TxtEnterNames.HideDown();
            //    if (IsEmailOkay()) m_TxtEnterEmail.HideDown();
            //    if (IsWhyOkay()) m_TxtEnterWhy.HideDown();
            //    if (IsWhenOkay()) m_TxtEnterWhen.HideDown();
            //    if (IsAcceptOkay()) m_TxtEnterAcceptImpressum.HideDown();

            //    UpdateUi();
            //}
        }

        #endregion private Methoden

        #region EventHandler

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            HideDown();
        }

        private void TxtImpressum_MouseDown(object sender, MouseButtonEventArgs e)
        {
            m_Impressum.ShowUp();
        }

        private void TxtImpressum_TouchDown(object sender, TouchEventArgs e)
        {
            m_Impressum.ShowUp();
        }

        private void CheckAccept_Checked(object sender, RoutedEventArgs e)
        {
            UpdateUi();
            //if (m_TxtEnterAcceptImpressum != null)
            //{
            //    if (IsAcceptOkay()) m_TxtEnterAcceptImpressum.Hide();
            //    UpdateUi();
            //}
        }

        private void CheckAccept_Unchecked(object sender, RoutedEventArgs e)
        {
            UpdateUi();
            //if (m_TxtEnterAcceptImpressum != null)
            //{
            //    if (!IsAcceptOkay()) m_TxtEnterAcceptImpressum.ShowUp();
            //    UpdateUi();
            //}
        }

        private void BtnSend_Clicked(object sender, EventArgs e)
        {
            Send();
        }

        private void EditBox_EditFocused(object sender, EditBox.EditFocusedEventArgs e)
        {
            RestartTimer.Restart();
            m_Keyboard.SetConnectedTextBox(e.TextBox, e.EditBox);
            UpdateUi();

            //if (e.EditBox == m_EditEmail) GiveHelp(m_EditLastname);
            //else if (e.EditBox == m_EditWhy) GiveHelp(m_EditEmail);
            //else if (e.EditBox == m_EditWhen) GiveHelp(m_EditWhy);
        }


        private void EditWhen_EditChanged(object sender, EditBox.EditLostFocusEventArgs e)
        {
            GiveHelp(e.EditBox);
        }

        private void EditBox_EditLostFocus(object sender, EditBox.EditLostFocusEventArgs e)
        {
            GiveHelp(e.EditBox);
        }

        private void ComboBoxGift_SelectionChanged(object sender, Pmt.BiographiePortal.Controls.SendOwnStories.SosComboBox.SelectionChangedEventArgs e)
        {
            UpdateUi();
        }

        private void ComboBoxWho_SelectionChanged(object sender, Pmt.BiographiePortal.Controls.SendOwnStories.SosComboBox.SelectionChangedEventArgs e)
        {
            UpdateUi();
        }

        private void ComboBoxInOrOut_SelectionChanged(object sender, SosComboBox.SelectionChangedEventArgs e)
        {
            UpdateUi();
        }

        private void BtnBackToPortal_Clicked(object sender, EventArgs e)
        {
            HideDown();
        }

        #endregion EventHandler

    }
}
