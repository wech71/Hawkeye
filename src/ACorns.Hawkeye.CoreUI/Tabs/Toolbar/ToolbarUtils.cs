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
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using ACorns.Hawkeye.Utils;

namespace ACorns.Hawkeye.Tabs.Toolbar
{
	/// <summary>
	/// Summary description for ToolbarUtils.
	/// </summary>
	internal sealed class ToolbarUtils
	{
		private ToolbarUtils()
		{}

		public static void AddDelimiter(ToolBar toolbar)
		{
			ToolBarButton delim = new ToolBarButton("|");
			delim.Style = ToolBarButtonStyle.Separator;
			toolbar.Buttons.Add(delim);
		}

		public static ToolBarButton AddButton(ToolBar toolbar, string text, string imageName, EventHandler eventHandler)
		{
			return AddButton(-1, toolbar, text, imageName, eventHandler);
		}
		public static ToolBarButton AddButton(int index, ToolBar toolbar, string text, string imageName, EventHandler eventHandler)
		{
			Image loadImage = SystemUtils.LoadBitmap(imageName);
			return AddButton(index, toolbar, text, loadImage, eventHandler);
		}
		public static ToolBarButton AddButton(int index, ToolBar toolbar, string text, Image loadImage, EventHandler eventHandler)
		{
			toolbar.ImageList.Images.Add(loadImage);
			int imageIndex = toolbar.ImageList.Images.Count - 1;

			ToolBarButton barButton = new ToolBarButton("");
			barButton.Tag = eventHandler;
			barButton.ImageIndex = imageIndex;
			barButton.ToolTipText = text;

			if (index == -1)
			{
				toolbar.Buttons.Add(barButton);
			}
			else
			{
				toolbar.Buttons.Insert(index, barButton);
			}
			return barButton;
		}

		public static void DelButton(ToolBar bar, string text)
		{
			for ( int i = 0; i < bar.Buttons.Count; i++ )
			{
				ToolBarButton button = bar.Buttons[i];
				if ( button.ToolTipText == text )
				{
					bar.Buttons.RemoveAt(i);
					if ( bar.Buttons[i].Style == ToolBarButtonStyle.Separator )
						bar.Buttons.RemoveAt(i);
					break;
				}
			}
		}
	}
}
