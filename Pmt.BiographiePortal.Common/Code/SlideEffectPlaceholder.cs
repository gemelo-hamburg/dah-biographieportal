using Gemelo.Components.Common.Wpf.UI.Transitions;
using Gemelo.Components.Common.Wpf.UI.Transitions.Appearance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gemelo.Applications.Pmt.BiographiePortal.Code.Misc
{
    public static class SlideEffectPlaceholder
    {
        public static void SlideIn(this FrameworkElement element, SlideDirection direction)
        {
            //TODO TR: SlideIn implementieren
            element.FadeInIfNotVisible();
        }

        public static void SlideOut(this FrameworkElement element, SlideDirection direction)
        {
            //TODO TR: SlideOut implementieren
            element.FadeOutIfVisible();
        }

        public static void StopWpfEffectAndShow(this FrameworkElement element)
        {
            element.StopTransitionAndShow();
        }

        public static void StopWpfEffectAndHide(this FrameworkElement element)
        {
            element.StopTransitionAndHide();
        }

        public static void StopWpfEffectAndCollapse(this FrameworkElement element)
        {
            element.StopTransitionAndCollapse();
        }

       

    }

    public enum HideEffect
    {

    }

    public enum SlideDirection
    {
        Bottom,
        Top,
        Left
    }
}
