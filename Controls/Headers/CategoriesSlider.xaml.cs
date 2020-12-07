using Gemelo.Applications.Biographieportal.Applications;
using Gemelo.Applications.Biographieportal.Code.Data;
using Pete.Components.WpfExtensions.Extensions;
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
using System.Windows.Threading;

namespace Gemelo.Applications.Biographieportal.Controls.Headers
{
    /// <summary>
    /// Interaktionslogik für CategoriesSlider.xaml
    /// </summary>
    public partial class CategoriesSlider : UserControl
    {
        #region private Member

        private DispatcherTimer m_TimerSliderout;
        private bool m_IsSlidedIn = true;

        #endregion private Member

        public CategoriesSlider()
        {
            InitializeComponent();
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Center;

            m_TimerSliderout = new DispatcherTimer();
            m_TimerSliderout.Interval = App.Current.CategorySliderTimeout;
            m_TimerSliderout.Tick += TimerSliderout_Tick;

            Hide();
        }



        #region öffentliche Methoden

        public void SetContent(IEnumerable<BasePartialData> datas)
        {
            StackPanel horzStack = null;
            foreach (BasePartialData data in datas)
            {
                if (horzStack == null || data.NewLine) horzStack = CreateNewHorzStack();

                DahButton btn = new DahButton();
                btn.SetData(data);
                horzStack.Children.Add(btn);
                //m_PanelContent.Children.Add(btn);
            }
        }


        public void Hide()
        {
            m_TimerSliderout.Stop();
            if (m_IsSlidedIn) this.SlideOut(SlideDirection.Top);

            m_IsSlidedIn = false;
        }

        public void Toggle()
        {
            if (m_IsSlidedIn) Hide();
            else
            {
                this.SlideIn(SlideDirection.Top);
                m_TimerSliderout.Start();
            }

            m_IsSlidedIn = !m_IsSlidedIn;
        }

        #endregion öffentliche Methoden


        private StackPanel CreateNewHorzStack()
        {
            StackPanel stack = new StackPanel();
            stack.Orientation = Orientation.Horizontal;
            stack.HorizontalAlignment = HorizontalAlignment.Center;
            m_StackVertical.Children.Add(stack);
            return stack;
        }


        #region EventHandler

        private void TimerSliderout_Tick(object sender, EventArgs e)
        {
            Hide();
        }

        #endregion EventHandler

    }
}
