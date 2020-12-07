using Gemelo.Applications.Biographieportal.Code.Enumerations;
using Pete.Components.WpfExtensions.Localization;
using System;
using System.Collections.Generic;
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

namespace Gemelo.Applications.Biographieportal.Controls.ContentDetails
{
    /// <summary>
    /// Interaktionslogik für AdditionalParts.xaml
    /// </summary>
    public partial class AdditionalParts : UserControl
    {
        public AdditionalParts(Code.Data.BiographyPart additional)
        {
            InitializeComponent();

            m_TxtTitle.SetTextsFromLocalizationString(additional.Title);
            m_TxtText.SetTextsFromLocalizationString(additional.Text);

            if (additional.MediaFile.Type != MediaFileType.NotExist)
            {
                ContentImage ci = new ContentImage(additional.MediaFile, small: true);
                m_GridImage.Children.Add(ci);
            }

        }
    }
}
