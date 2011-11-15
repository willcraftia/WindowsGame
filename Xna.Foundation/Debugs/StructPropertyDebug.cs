#region Using

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

#endregion

namespace Willcraftia.Xna.Foundation.Debugs
{
    public sealed class StructPropertyDebug
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
        string structPropertyName;
        Command[] commands;

        #endregion

        #region Constructors

        public StructPropertyDebug(
            object target,
            string structPropertyName,
            Command[] commands)
        {
            this.target = target;
            this.structPropertyName = structPropertyName;
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
            bool parentExist = false;
            var parentPropertyInfo = target.GetType().GetProperty(structPropertyName);
            if (parentPropertyInfo != null)
            {
                parentExist = true;
                var parent = parentPropertyInfo.GetValue(target, null);
                if (EchoProperty(host, command, parent))
                {
                    return;
                }
            }

            var parentFieldInfo = target.GetType().GetField(structPropertyName);
            if (parentFieldInfo != null)
            {
                parentExist = true;
                var parent = parentFieldInfo.GetValue(target);
                if (EchoProperty(host, command, parent))
                {
                    return;
                }
            }

            if (!parentExist)
            {
                host.Echo(string.Format(
                    "'{0}' have no property/filed '{1}'",
                    target.GetType().Name,
                    structPropertyName));
            }
            else
            {
                host.Echo(string.Format(
                    "'{0}.{1}' have no property/filed '{2}'",
                    target.GetType().Name,
                    structPropertyName,
                    command.PropertyName));
            }
        }

        bool EchoProperty(
            IDebugCommandHost host,
            Command command,
            object parent)
        {
            //var propertyInfo = parent.GetType().GetProperty(command.PropertyName);
            //if (propertyInfo != null)
            //{
            //    var value = propertyInfo.GetValue(parent, null);
            //    host.Echo(string.Format(
            //        "[{0}] {1}: {2}",
            //        command.CommandName,
            //        command.PropertyName,
            //        value));
            //    return true;
            //}

            var fieldInfo = parent.GetType().GetField(command.PropertyName);
            if (fieldInfo != null)
            {
                var value = fieldInfo.GetValue(parent);
                host.Echo(string.Format(
                    "[{0}] {1}: {2}",
                    command.CommandName,
                    command.PropertyName,
                    value));
                return true;
            }

            return false;
        }

        void SetProperty(
            IDebugCommandHost host,
            Command command,
            string value)
        {
            bool parentExist = false;
            var parentPropertyInfo = target.GetType().GetProperty(structPropertyName);
            if (parentPropertyInfo != null)
            {
                parentExist = true;
                var parent = parentPropertyInfo.GetValue(target, null);
                var newParent = SetProperty(host, command, value, parent);
                if (newParent != null)
                {
                    parentPropertyInfo.SetValue(target, newParent, null);
                    return;
                }
            }

            var parentFieldInfo = target.GetType().GetField(structPropertyName);
            if (parentFieldInfo != null)
            {
                parentExist = true;
                var parent = parentFieldInfo.GetValue(target);
                var newParent = SetProperty(host, command, value, parent);
                if (newParent != null)
                {
                    parentFieldInfo.SetValue(target, newParent);
                    return;
                }
            }

            if (!parentExist)
            {
                host.Echo(string.Format(
                    "'{0}' have no property/filed '{1}'",
                    target.GetType().Name,
                    structPropertyName));
            }
            else
            {
                host.Echo(string.Format(
                    "'{0}.{1}' have no property/filed '{2}'",
                    target.GetType().Name,
                    structPropertyName,
                    command.PropertyName));
            }
        }

        object SetProperty(
            IDebugCommandHost host,
            Command command,
            string value,
            object parent)
        {
            try
            {
                //var propertyInfo = parent.GetType().GetProperty(command.PropertyName);
                //if (propertyInfo != null)
                //{
                //    object convertedValue = ConvertValue(propertyInfo.PropertyType, value);
                //    propertyInfo.SetValue(parent, convertedValue, null);
                //    return parent;
                //}

                var fieldInfo = parent.GetType().GetField(command.PropertyName);
                if (fieldInfo != null)
                {
                    object convertedValue = ConvertValue(fieldInfo.FieldType, value);
                    fieldInfo.SetValue(parent, convertedValue);
                    return parent;
                }
            }
            catch (Exception e)
            {
                host.EchoError(e.Message);
            }

            return null;
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
