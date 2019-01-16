/* ****************************************************************************
 *  Hawkeye - The .Net Runtime Object Editor
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
using System.Diagnostics;
using System.Reflection;

namespace ACorns.Hawkeye.Core.Utils.Accessors
{
	/// <summary>
	/// Summary description for MethodAccesor.
	/// </summary>
	public class MethodAccesor
	{
		private readonly string methodName;
		private MethodInfo methodInfo;
		private readonly Type targetType;

		public MethodAccesor(Type targetType, string methodName)
		{
			this.methodName = methodName;
			this.targetType = targetType;

			do
			{
				TryLinkMethod(BindingFlags.NonPublic | BindingFlags.Instance);
				TryLinkMethod(BindingFlags.Public | BindingFlags.Instance);
				TryLinkMethod(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

				if (methodInfo == null)
				{
					this.targetType = this.targetType.BaseType;
					if (this.targetType == typeof(object))
					{ // no chance
						break;
					}
				}

			} while (methodInfo == null);
		}

		private void TryLinkMethod(BindingFlags bindingFlags)
		{
			if (methodInfo != null)
				return;
			methodInfo = targetType.GetMethod(this.methodName, bindingFlags);
		}

		public object Invoke(object target, object[] param)
		{
			return methodInfo.Invoke(target, param);
		}

		public object Invoke(object target)
		{
			return Invoke(target, new object[] { });
		}

		public object Invoke(object target, object oneParam)
		{
			return Invoke(target, new object[] { oneParam });
		}
		public bool IsValid
		{
			get { return methodInfo != null; }
		}
	}
}