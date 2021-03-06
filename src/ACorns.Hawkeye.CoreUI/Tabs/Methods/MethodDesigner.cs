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
using System.ComponentModel.Design;
using System.Windows.Forms;

namespace ACorns.Hawkeye.Tabs.Methods
{
	/// <summary>
	/// Summary description for MethodDesigner.
	/// </summary>
	internal class MethodDesigner : ComponentDesigner
	{
		private DesignerVerbCollection verbs;

		public MethodDesigner()
		{
			
		}

		public void CreateVerbs()
		{
			verbs = new DesignerVerbCollection(new DesignerVerb[]
				{
					new DesignerVerb("About", new EventHandler(OnSelectThisItem)),
				});
		}

		//public override DesignerVerbCollection Verbs
		//{
		//    get
		//    {
		//        if ( this.verbs == null )
		//            CreateVerbs();
		//        return this.verbs;
		//    }
		//}

		//public override DesignerActionListCollection ActionLists
		//{
		//    get
		//    {
		//        return null;
		//    }
		//}

		private void OnSelectThisItem(object sender, EventArgs e)
		{
		}
	}
}