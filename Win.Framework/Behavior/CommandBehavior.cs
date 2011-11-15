﻿#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

#endregion

namespace Willcraftia.Win.Framework.Behavior
{
    public class CommandBehavior
    {
        #region Behavior

        /// <summary>
        /// Behavior Attached Dependency Property
        /// </summary>
        private static readonly DependencyProperty BehaviorProperty =
            DependencyProperty.RegisterAttached(
                "Behavior", typeof(CommandBehaviorBinding), typeof(CommandBehavior),
                new FrameworkPropertyMetadata((CommandBehaviorBinding)null));

        /// <summary>
        /// Gets the Behavior property. 
        /// </summary>
        private static CommandBehaviorBinding GetBehavior(DependencyObject d)
        {
            return d.GetValue(BehaviorProperty) as CommandBehaviorBinding;
        }

        /// <summary>
        /// Sets the Behavior property.  
        /// </summary>
        private static void SetBehavior(DependencyObject d, CommandBehaviorBinding value)
        {
            d.SetValue(BehaviorProperty, value);
        }

        #endregion

        #region Command

        /// <summary>
        /// Command Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached(
                "Command", typeof(ICommand), typeof(CommandBehavior),
                new FrameworkPropertyMetadata((ICommand) null, new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Gets the Command property.  
        /// </summary>
        public static ICommand GetCommand(DependencyObject d)
        {
            return d.GetValue(CommandProperty) as ICommand;
        }

        /// <summary>
        /// Sets the Command property. 
        /// </summary>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Handles changes to the Command property.
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var binding = FetchOrCreateBinding(d);
            binding.Command = e.NewValue as ICommand;
        }

        #endregion

        #region Action

        /// <summary>
        /// Action Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty ActionProperty =
            DependencyProperty.RegisterAttached(
                "Action", typeof(Action<object>), typeof(CommandBehavior),
                new FrameworkPropertyMetadata((Action<object>) null, new PropertyChangedCallback(OnActionChanged)));

        /// <summary>
        /// Gets the Action property.  
        /// </summary>
        public static Action<object> GetAction(DependencyObject d)
        {
            return d.GetValue(ActionProperty) as Action<object>;
        }

        /// <summary>
        /// Sets the Action property. 
        /// </summary>
        public static void SetAction(DependencyObject d, Action<object> value)
        {
            d.SetValue(ActionProperty, value);
        }

        /// <summary>
        /// Handles changes to the Action property.
        /// </summary>
        private static void OnActionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var binding = FetchOrCreateBinding(d);
            binding.Action = e.NewValue as Action<object>;
        }

        #endregion

        #region CommandParameter

        /// <summary>
        /// CommandParameter Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.RegisterAttached(
                "CommandParameter", typeof(object), typeof(CommandBehavior),
                new FrameworkPropertyMetadata((object) null, new PropertyChangedCallback(OnCommandParameterChanged)));

        /// <summary>
        /// Gets the CommandParameter property.  
        /// </summary>
        public static object GetCommandParameter(DependencyObject d)
        {
            return d.GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Sets the CommandParameter property. 
        /// </summary>
        public static void SetCommandParameter(DependencyObject d, object value)
        {
            d.SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Handles changes to the CommandParameter property.
        /// </summary>
        private static void OnCommandParameterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var binding = FetchOrCreateBinding(d);
            binding.CommandParameter = e.NewValue;
        }

        #endregion

        #region Event

        /// <summary>
        /// Event Attached Dependency Property
        /// </summary>
        public static readonly DependencyProperty EventProperty =
            DependencyProperty.RegisterAttached(
                "Event", typeof(string), typeof(CommandBehavior),
                new FrameworkPropertyMetadata(String.Empty, new PropertyChangedCallback(OnEventChanged)));

        /// <summary>
        /// Gets the Event property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static string GetEvent(DependencyObject d)
        {
            return d.GetValue(EventProperty) as string;
        }

        /// <summary>
        /// Sets the Event property.  This dependency property 
        /// indicates ....
        /// </summary>
        public static void SetEvent(DependencyObject d, string value)
        {
            d.SetValue(EventProperty, value);
        }

        /// <summary>
        /// Handles changes to the Event property.
        /// </summary>
        private static void OnEventChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var binding = FetchOrCreateBinding(d);
            //check if the Event is set. If yes we need to rebind the Command to the new event and unregister the old one
            if (binding.Event != null && binding.Owner != null)
            {
                binding.Dispose();
            }
            //bind the new event to the command
            binding.BindEvent(d, e.NewValue.ToString());
        }

        #endregion

        #region Helpers
        //tries to get a CommandBehaviorBinding from the element. Creates a new instance if there is not one attached
        private static CommandBehaviorBinding FetchOrCreateBinding(DependencyObject d)
        {
            var binding = CommandBehavior.GetBehavior(d);
            if (binding == null)
            {
                binding = new CommandBehaviorBinding();
                CommandBehavior.SetBehavior(d, binding);
            }
            return binding;
        }
        #endregion

    }
}
