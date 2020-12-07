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
using Gemelo.Applications.Biographieportal.Code.Data;

namespace Gemelo.Applications.Biographieportal.Controls.ContentDetails
{
    /// <summary>
    /// Interaktionslogik für CategoriesDisplay.xaml
    /// </summary>
    public partial class CategoriesDisplay : UserControl
    {
        public CategoriesDisplay(Code.Data.Biography bio)
        {
            InitializeComponent();

            m_PanelContent.Children.Clear();

            foreach (var tr in bio.TimeRanges) AddButton(tr);
            foreach (var tr in bio.MigrationReasons) AddButton(tr);
            foreach (var tr in bio.MigrationEffects) AddButton(tr);
            foreach (var tr in bio.MigrationTypes) AddButton(tr);
        }

        private void AddButton(BasePartialData data)
        {
            DahButton btn = new DahButton();
            btn.FireClickAtTouchDown = true;
            btn.SetData(data);
            m_PanelContent.Children.Add(btn);
        }
    }
}
