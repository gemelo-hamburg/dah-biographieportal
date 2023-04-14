using Gemelo.Applications.Pmt.BiographiePortal.Code;
using Gemelo.Components.Common.Localization;
using Gemelo.Components.Common.Wpf.Localization;
using Gemelo.Components.Common.Wpf.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gemelo.Applications.Pmt.BiographiePortal.Controls.SendOwnStories
{
    /// <summary>
    /// Interaktionslogik für SosComboBox.xaml
    /// </summary>
    //[DefaultProperty("Children")]
    [ContentProperty("Items")]
    public partial class SosComboBox : UserControl
    {
        #region öffentliche Eigenschaften

        #region DependencyProperty SelectedData

        public static readonly DependencyProperty SelectedDataProperty =
           DependencyProperty.Register(
           "SelectedData", typeof(ComboBoxData),
           typeof(SosComboBox),
           new PropertyMetadata(null, new PropertyChangedCallback(OnSelectedDataChanged)));

        [Description("SelectedData")]
        [Category("gemelo")]
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public ComboBoxData SelectedData
        {
            get { return ((ComboBoxData)(GetValue(SosComboBox.SelectedDataProperty))); }
            set { SetValue(SosComboBox.SelectedDataProperty, value); }
        }

        private static void OnSelectedDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SosComboBox ctrl)
            {
                ComboBoxData newValue = (ComboBoxData)e.NewValue;
                ComboBoxData oldValue = (ComboBoxData)e.OldValue;

                ctrl.m_Combo.SelectedItem = newValue;
                ctrl.UpdateUi();
            }
        }

        #endregion DependencyProperty SelectedData

        #region DependencyProperty Items

        [Description("Items")]
        [Category("gemelo")]
        [Browsable(true)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public static readonly DependencyPropertyKey ItemsProperty = DependencyProperty.RegisterReadOnly(
           nameof(Items),
           typeof(List<ComboBoxData>),
           typeof(SosComboBox),
           new PropertyMetadata());

        public List<ComboBoxData> Items
        {
            get { return (List<ComboBoxData>)GetValue(ItemsProperty.DependencyProperty); }
            private set { SetValue(ItemsProperty, value); }
        }

        #endregion DependencyProperty Items

        #region DependencyProperty Caption

        public static readonly DependencyProperty CaptionProperty =
           DependencyProperty.Register(
           "Caption", typeof(LocalizationString),
           typeof(SosComboBox),
           new PropertyMetadata(null, new PropertyChangedCallback(OnCaptionChanged)));

        [System.ComponentModel.Description("Caption")]
        [System.ComponentModel.Category("gemelo")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public LocalizationString Caption
        {
            get { return ((LocalizationString)(GetValue(SosComboBox.CaptionProperty))); }
            set { SetValue(SosComboBox.CaptionProperty, value); }
        }

        private static void OnCaptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SosComboBox ctrl)
            {
                LocalizationString newValue = (LocalizationString)e.NewValue;
                LocalizationString oldValue = (LocalizationString)e.OldValue;

                ctrl.m_TxtCaption.SetLocalizedText(newValue);
            }
        }

        #endregion DependencyProperty Caption

        #region DependencyProperty WatermarkText

        public static readonly DependencyProperty WatermarkTextProperty =
           DependencyProperty.Register(
           "WatermarkText", typeof(LocalizationString),
           typeof(SosComboBox),
           new PropertyMetadata(null, new PropertyChangedCallback(OnWatermarkTextChanged)));

        [System.ComponentModel.Description("WatermarkText")]
        [System.ComponentModel.Category("gemelo")]
        [System.ComponentModel.Browsable(true)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Visible)]
        public LocalizationString WatermarkText
        {
            get { return ((LocalizationString)(GetValue(SosComboBox.WatermarkTextProperty))); }
            set { SetValue(SosComboBox.WatermarkTextProperty, value); }
        }

        private static void OnWatermarkTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SosComboBox ctrl)
            {
                LocalizationString newValue = (LocalizationString)e.NewValue;
                LocalizationString oldValue = (LocalizationString)e.OldValue;

                ctrl.m_TxtWatermark.SetLocalizedText(newValue);
            }
        }

        #endregion DependencyProperty WatermarkText

        #endregion öffentliche Eigenschaften

        #region Events


        #region Event SelectionChanged

        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        public class SelectionChangedEventArgs : EventArgs
        {
            public ComboBoxData Data { get; private set; }

            public SelectionChangedEventArgs(ComboBoxData _Data)
            {
                Data = _Data;
            }
        }

        protected void OnSelectionChanged(ComboBoxData _Data)
        {
            SelectionChanged?.Invoke(this, new SelectionChangedEventArgs(_Data));
        }

        #endregion Event SelectionChanged

        #endregion Events

        public SosComboBox()
        {
            Items = new List<ComboBoxData>();

            InitializeComponent();

            m_Combo.ItemsSource = Items;

            UpdateUi();
        }

        #region private Methoden

        private void UpdateUi()
        {
            if (m_Combo != null)
            {
                bool hasSelected = (m_Combo.SelectedItem != null);

                m_TxtWatermark.Visibility = (!hasSelected).ToVisibility();

                SelectedData = m_Combo.SelectedItem as ComboBoxData;
                OnSelectionChanged(SelectedData);
            }
        }

        #endregion private Methoden

        private void Combo_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            UpdateUi();
        }
    }
}
