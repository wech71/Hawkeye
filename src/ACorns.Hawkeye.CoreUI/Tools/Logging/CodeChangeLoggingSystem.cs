/* ****************************************************************************
 *  Hawkeye - The .Net Runtime Object Editor - Loader
 * 
 * Copyright (c) 2005 Corneliu I. Tusnea
 * 
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the author be held liable for any damages arising from 
 * the use of this software.
 * Permission to use, copy, modify, distribute and sell this software for any 
 * purpose is hereby granted without fee, provided that the above copyright 
 * notice appear in all copies and that both that copyright notice and this 
 * permission notice appear in supporting documentation.
 * 
 * Corneliu I. Tusnea (corneliutusnea@yahoo.com.au)
 * http://www.acorns.com.au/hawkeye/
 * ****************************************************************************/

using System;
using ACorns.Hawkeye.Core.Utils;
using ACorns.Hawkeye.Core.Utils.Visitor;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Tools.Logging
{
    /// <summary>
    /// Summary description for LoggingSystem.
    /// </summary>
    internal class CodeChangeLoggingSystem
    {
        #region Instance
        private static CodeChangeLoggingSystem instance = new CodeChangeLoggingSystem();
        /// <summary>
        /// Singleton instance of the LoggingSystem.
        /// </summary>
        public static CodeChangeLoggingSystem Instance
        {
            get { return instance; }
        }
        #endregion

        internal event TextAddedHandler TextAdded;

        public void LogSet(string owner, string property, object value)
        {
            string valueAsString = PrepareValueToLog(value);
            string propInfo;
            if (owner != null && owner.Length > 0)
            {
                propInfo = owner + "." + property;
            }
            else
            {
                propInfo = property;
            }
            Log(propInfo + " = " + valueAsString + ";");
        }

        public void Log(string text)
        {
            if (TextAdded != null)
            {
                TextAdded(text + "\r\n");
            }
        }

        public string PrepareValueToLog(object value)
        {
            if (value == null)
                return "null";

            object realValue = value;
            if (value is IRealValueHolder)
                realValue = (value as IRealValueHolder).RealValue;

            if (realValue is System.ValueType && value is IStringValueHolder)
            {	// non primitive value (struct)
                string stringValue = (value as IStringValueHolder).OriginalString;
                return " new " + realValue.GetType().Name + "(" + stringValue + ")";
            }

            return ObjectLogVisitors.Instance.ConvertValue(realValue);
        }
    }
}
