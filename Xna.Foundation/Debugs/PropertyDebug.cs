#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    public sealed class PropertyDebug
    {
        #region Inner classes

        public sealed class Command
        {
            public string CommandName { get; private set; }
            public string PropertyName { get; private set; }

            public Command(string propertyName)
                : this(propertyName, propertyName)
            {
            }

            public Command(string commandName, string propertyName)
            {
                CommandName = commandName;
                PropertyName = propertyName;
            }
        }

        #endregion

        #region Fields and Properties

        object target;
        Command[] commands;

        #endregion

        #region Constructors

        public PropertyDebug(object target, Command[] commands)
        {
            this.target = target;
            this.commands = commands;
        }

        #endregion

        public void EchoAllProperties(IDebugCommandHost host)
        {
            foreach (Command command in commands)
            {
                EchoProperty(host, command);
            }
        }
        
        public bool HandlePropertyCommand(
            IDebugCommandHost host,
            string commandName,
            string value)
        {
            bool handled = false;
            foreach (Command command in commands)
            {
                if (string.Compare(command.CommandName, commandName, true) == 0 ||
                    string.Compare(command.PropertyName, commandName, true) == 0)
                {
                    HandleProperty(host, command, value);
                    handled = true;
                    break;
                }
            }
            return handled;
        }

        void HandleProperty(IDebugCommandHost host, Command command, string value)
        {
            if (value == null)
            {
                EchoProperty(host, command);
            }
            else
            {
                SetProperty(host, command, value);
            }
        }

        void EchoProperty(
            IDebugCommandHost host,
            Command command)
        {
            var propertyInfo = target.GetType().GetProperty(command.PropertyName);
            if (propertyInfo != null)
            {
                var value = propertyInfo.GetValue(target, null);
                host.Echo(string.Format(
                    "[{0}] {1}: {2}",
                    command.CommandName,
                    command.PropertyName,
                    value));
                return;
            }

            var fieldInfo = target.GetType().GetField(command.PropertyName);
            if (fieldInfo != null)
            {
                var value = fieldInfo.GetValue(target);
                host.Echo(string.Format(
                    "[{0}] {1}: {2}",
                    command.CommandName,
                    command.PropertyName,
                    value));
                return;
            }

            host.Echo(string.Format(
                "'{0}' have no property/filed '{1}'",
                target.GetType().Name,
                command.PropertyName));
        }

        void SetProperty(
            IDebugCommandHost host,
            Command command,
            string value)
        {
            try
            {
                var propertyInfo = target.GetType().GetProperty(command.PropertyName);
                if (propertyInfo != null)
                {
                    object convertedValue = ConvertValue(propertyInfo.PropertyType, value);
                    propertyInfo.SetValue(target, convertedValue, null);
                    return;
                }

                var fieldInfo = target.GetType().GetField(command.PropertyName);
                if (fieldInfo != null)
                {
                    object convertedValue = ConvertValue(fieldInfo.FieldType, value);
                    fieldInfo.SetValue(target, convertedValue);
                    return;
                }

                host.Echo(string.Format(
                    "'{0}' have no property/filed '{1}'",
                    target.GetType().Name,
                    command.PropertyName));
            }
            catch (Exception e)
            {
                host.EchoError(e.Message);
            }
        }

        const char vector3ParseSeparator = ',';

        object ConvertValue(Type type, string value)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, value, true);
            }
            else
            {
                try
                {
                    return Convert.ChangeType(value, type);
                }
                catch (InvalidCastException e)
                {
                    if (type == typeof(Vector3))
                    {
                        return Vector3Extension.Parse(value, vector3ParseSeparator);
                    }
                    else
                    {
                        throw e;
                    }
                }
            }
        }
    }
}
