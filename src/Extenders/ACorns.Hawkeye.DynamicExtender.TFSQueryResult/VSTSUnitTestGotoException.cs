using System;
using System.Collections.Generic;
using System.Text;
using ACorns.Hawkeye.Public;

namespace ACorns.Hawkeye.DynamicExtender.TFSQueryResult
{
	public class VSTSUnitTestGotoException : IDynamicSubclass
	{
		#region IDynamicSubclass Members

		public void Attach(object target)
		{
			if (target == null)
				return;

			Type targetType = target.GetType();
			if (targetType.FullName == "Microsoft.VisualStudio.TeamFoundation.WorkItemTracking.ResultView")
			{

			}
		}

		#endregion
	}
}
