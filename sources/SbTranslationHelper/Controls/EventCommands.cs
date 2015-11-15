using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SbTranslationHelper.Controls
{
    public static class EventCommands
    {

        #region Double click event to command

        public static ICommand GetDoubleClickToCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DoubleClickToCommandProperty);
        }
        public static void SetDoubleClickToCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DoubleClickToCommandProperty, value);
        }
        public static readonly DependencyProperty DoubleClickToCommandProperty =
            DependencyProperty.RegisterAttached("DoubleClickToCommand", typeof(ICommand), typeof(EventCommands), new PropertyMetadata(null, DoubleClickToCommandPropertyChanged));
        private static void DoubleClickToCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control ctrl = d as Control;
            if (ctrl != null)
            {
                if (e.OldValue != null)
                    ctrl.MouseDoubleClick -= Ctrl_MouseDoubleClick;
                if (e.NewValue != null)
                    ctrl.MouseDoubleClick += Ctrl_MouseDoubleClick;
            }
        }
        private static void Ctrl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject dobj = sender as DependencyObject;
            if (dobj != null)
            {
                var command = GetDoubleClickToCommand(dobj);
                if (command != null)
                {
                    var prm = GetDoubleClickToCommandParameter(dobj);
                    if (command.CanExecute(prm))
                        command.Execute(prm);
                }
            }
        }


        public static Object GetDoubleClickToCommandParameter(DependencyObject obj)
        {
            return (Object)obj.GetValue(DoubleClickToCommandParameterProperty);
        }
        public static void SetDoubleClickToCommandParameter(DependencyObject obj, Object value)
        {
            obj.SetValue(DoubleClickToCommandParameterProperty, value);
        }
        public static readonly DependencyProperty DoubleClickToCommandParameterProperty =
            DependencyProperty.RegisterAttached("DoubleClickToCommandParameter", typeof(Object), typeof(EventCommands), new PropertyMetadata(null));


        #endregion

    }

}
