using Gemelo.Components.Common.Wpf.UI.Transitions.Appearance;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gemelo.Applications.Biographieportal.Controls
{
    /// <summary>
    /// Interaktionslogik für Impressum.xaml
    /// </summary>
    public partial class Impressum : UserControl
    {
        public Impressum()
        {
            InitializeComponent();

            Loaded += Impressum_Loaded;
        }

        private void Impressum_Loaded(object sender, RoutedEventArgs e)
        {
            System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(typeof(Impressum));
            //var names = a.GetManifestResourceNames();
            //foreach (var name in names) Debug.WriteLine(name);
            string resName = @"Gemelo.Applications.Pmt.BiographiePortal.Resources.Texts.Impressum.rtf";

            using (Stream resFilestream = a.GetManifestResourceStream(resName))
            {
                m_RtfText.Selection.Load(resFilestream, DataFormats.Rtf);
            }
        }

        public void ShowUp()
        {
            this.FadeInIfNotVisible();
            this.IsHitTestVisible = true;
        }

        public void HideDown()
        {
            this.FadeOutIfVisible();
            this.IsHitTestVisible = false;
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            HideDown();
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
