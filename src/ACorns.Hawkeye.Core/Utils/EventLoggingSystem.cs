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


using System.Text;

namespace ACorns.Hawkeye.Core.Utils
{
	public delegate void TextAddedHandler(string newText);

	/// <summary>
	/// Summary description for LoggingSystem.
	/// </summary>
	public class EventLoggingSystem
	{
		#region Instance

		private static EventLoggingSystem instance = new EventLoggingSystem();

		/// <summary>
		/// Singleton instance of the LoggingSystem.
		/// </summary>
		public static EventLoggingSystem Instance
		{
			get { return instance; }
		}

		#endregion

		public event TextAddedHandler TextAdded;

		public void Log(object target, string eventName, object[] parameters, string[] parameterNames)
		{
			if (TextAdded != null)
			{
				StringBuilder builder = new StringBuilder();
				for (int i = 0; i < parameterNames.Length; i++)
				{
					if (i != 0)
						builder.Append(", ");

					if (parameterNames[i] == "sender" && parameters[i] == target)
					{
						builder.Append("sender");
					}
					else
					{
						builder.Append(parameterNames[i] + " = " + EventHandlerTypeVisitors.Instance.Convert(parameters[i]));
					}
				}

				TextAdded(HawkeyeUtils.GetControlName2OrToString(target) + eventName + "( " + builder.ToString() + ")\r\n");
			}
		}
	}
}