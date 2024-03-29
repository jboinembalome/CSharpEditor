namespace HighlightingLib.Manager
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// This class is used to prevent stack overflows by representing a 'busy' flag
	/// that prevents reentrance when another call is running.
	/// However, using a simple 'bool busy' is not thread-safe, so we use a
	/// thread-static BusyManager.
	/// </summary>
	internal static class BusyManager
	{
		public struct BusyLock : IDisposable
		{
			public static readonly BusyLock Failed = new(null);

			readonly List<object> _objectList;

			internal BusyLock(List<object> objectList)
			{
				_objectList = objectList;
			}

			public bool Success
			{
				get { return _objectList != null; }
			}

			public void Dispose()
			{
				if (_objectList != null)
				{
					_objectList.RemoveAt(_objectList.Count - 1);
				}
			}
		}

		[ThreadStatic] static List<object> _activeObjects;

		public static BusyLock Enter(object obj)
		{
			List<object> activeObjects = _activeObjects;
			if (activeObjects == null)
				activeObjects = _activeObjects = new List<object>();
			for (int i = 0; i < activeObjects.Count; i++)
			{
				if (activeObjects[i] == obj)
					return BusyLock.Failed;
			}
			activeObjects.Add(obj);
			return new BusyLock(activeObjects);
		}
	}
}
