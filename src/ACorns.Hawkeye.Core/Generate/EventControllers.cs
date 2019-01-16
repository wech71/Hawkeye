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
using System.Collections;
using System.Diagnostics;

namespace ACorns.Hawkeye.Core.Generate
{
	/// <summary>
	/// The list with all registered event controllers.
	/// </summary>
	public class EventControllers
	{
		#region Instance
		private static EventControllers instance = new EventControllers();
		/// <summary>
		/// Singleton instance of the LoggingSystem.
		/// </summary>
		public static EventControllers Instance
		{
			get { return instance; }
		}
		#endregion

		private SortedList eventListeners = new SortedList();

		public EventControllers()
		{
		}

		public void Add(EventController controller)
		{
			eventListeners.Add(controller.Target + "." + controller.EventName + "." + controller.EventAction, controller);
		}
		public void Remove(EventController controller)
		{
			eventListeners.Remove(controller.Target + "." + controller.EventName + "." + controller.EventAction);
		}

		public void RemoveAll()
		{
			SortedList copy = eventListeners.Clone() as SortedList;
			foreach ( EventController controller in copy.Values )
			{
				try
				{
					controller.Detach();
				}
				catch(Exception ex)
				{
					Trace.WriteLine("Could not detach Event listener:" + controller.EventName +". Exception:" + ex.Message);
				}
			}
			eventListeners.Clear();
		}

		public void RemoveAll(object target)
		{
			SortedList copy = eventListeners.Clone() as SortedList;
			foreach ( EventController controller in copy.Values )
			{
				try
				{
					if ( target == controller.Target )
					{
						controller.Detach();
					}
				}
				catch(Exception ex)
				{
					Trace.WriteLine("Could not detach Event listener:" + controller.EventName +". Exception:" + ex.Message);
				}
			}
		}
	}
}
