using System.Drawing;

namespace ACorns.Hawkeye.Plugins
{
	/// <summary>
	/// Interface for extenders of Finders. You can implemenet this interface and then register it inside the
	/// ObjectEditor to receive notifications when the finder is moved.
	/// You can then respond with a specific object instance that is the object under the finder.
	/// </summary>
	public interface IFinderExtender
	{
		/// <summary>
		/// Resolve the item at the selected location to an object.
		/// Return the object in the selectedObject.
		/// </summary>
		/// <param name="location"></param>
		/// <param name="lastKnowSelectedObject"></param>
		/// <param name="selectedObject"></param>
		/// <returns></returns>
		bool ResolveSelection(Point location, object lastKnowSelectedObject, ref object selectedObject);

		void Initialize(IHawkeyeHost host);

		void OnSelectedObjectChanged(object newSelection);
	}
}
