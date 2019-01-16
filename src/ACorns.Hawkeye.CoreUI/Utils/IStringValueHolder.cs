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

namespace ACorns.Hawkeye.Utils
{
	/// <summary>
	/// Holder for the string representation of an object or method call
	/// </summary>
	internal interface IStringValueHolder
	{
		string OriginalString { get; }
	}
}