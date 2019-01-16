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
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Reflector;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Tools.Reflector
{
	/// <summary>
	/// Summary description for ReflectorRouter.
	/// </summary>
	internal class ReflectorRouter
	{
		#region Instance
		private static ReflectorRouter instance = new ReflectorRouter();
		/// <summary>
		/// Singleton instance of the ReflectorRouter.
		/// </summary>
		public static ReflectorRouter Instance
		{
			get { return instance; }
		}
		#endregion

		private ReflectorRouter()
		{
		}

		public void ShowSourceCode(PropertyDescriptor descriptor)
		{
			Cursor.Current = Cursors.WaitCursor;
			try
			{
				if ( !RemoteController.Available )
				{
					MessageBox.Show("Lutz Roeder's .NET Reflector is not started or it is a version older than 4.0. Hawkeye can not show you the source code for the selected item.\r\nPlease open .NET Reflector to use this feature.", SystemUtils.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				IShowSourceCodeHandler showSourceCodeHandler = descriptor as IShowSourceCodeHandler;
				if ( showSourceCodeHandler != null )
				{
					showSourceCodeHandler.ShowSourceCode();
				}
				else
				{
					// maybe we know how to show this
					if ( descriptor != null )
					{
						ShowProperty(descriptor.ComponentType, descriptor.Name);
					}
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}


		public void ShowEvent(Type type, EventInfo eventInfo)
		{
			LoadAssembly(type);

			string name = type.FullName + "." + eventInfo.Name;

			RemoteController.SelectEventDeclaration("E:" + name);			
		}
		public void ShowField(Type type, FieldInfo fieldInfo)
		{
			LoadAssembly(type);

			string name = type.FullName + "." + fieldInfo.Name;

			RemoteController.SelectFieldDeclaration("F:" + name);			
		}
		public void ShowProperty(Type type, PropertyInfo propertyInfo)
		{
			ShowProperty(type, propertyInfo.Name);
		}
		public void ShowProperty(Type type, string propertyName)
		{
			LoadAssembly(type);

			string name = type.FullName + "." + propertyName;

			RemoteController.SelectPropertyDeclaration("P:" + name);
		}
		public void ShowMethod(Type type, MethodInfo methodInfo)
		{
			LoadAssembly(type);

			StringBuilder builder = new StringBuilder();
			builder.Append(type.FullName + "." + methodInfo.Name + "(");

			bool hasParams = false;
			// we also have to add all the parameter info
			foreach(ParameterInfo param in methodInfo.GetParameters())
			{
				builder.Append(param.ParameterType.FullName);
				builder.Append(",");
				hasParams = true;
			}
			if ( hasParams )
			{
				builder.Remove(builder.Length-1,1);
				builder.Append(")");
			}
			else
			{
				builder.Remove(builder.Length-1,1);
			}

			RemoteController.SelectMethodDeclaration("M:" + builder);
		}

		private void LoadAssembly(Type type)
		{
			string fullName = type.Module.FullyQualifiedName;
			RemoteController.LoadAssembly(fullName);
		}
	}
}
