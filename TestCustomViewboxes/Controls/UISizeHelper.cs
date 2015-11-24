namespace WpfApplication1.Controls
{
	#region

	using System;

	using UnityEngine;

	#endregion

	public static class UISizeHelper
	{
		#region Static Fields

		public static float DefaultUIHeight = 768;

		public static float DefaultUIWidth = 1024;

		#endregion

		#region Public Properties

		public static float UIScale { get; private set; }

		#endregion

		#region Public Methods and Operators

		public static void RefreshRelatedControls()
		{
			foreach (var weakReference in ScreenViewbox.Instances)
			{
				var target = weakReference.Target;
				if (target == null)
				{
					// reference is dead
					continue;
				}

				((ScreenViewbox)target).RefreshSize();
			}

			foreach (var weakReference in ScreenAutoViewbox.Instances)
			{
				var target = weakReference.Target;
				if (target == null)
				{
					// reference is dead
					continue;
				}

				((ScreenAutoViewbox)target).RefreshSize();
			}
		}

		public static void SetUIScale(float scale)
		{
			if (Math.Abs(UIScale - scale) < float.Epsilon)
			{
				return;
			}

			UIScale = scale;
			DefaultUIWidth = 1024f / scale;
			DefaultUIHeight = 768f / scale;

			Debug.Log(
				string.Format("UI base size changed: {0}x{1}; scale={2:F2}", (int)DefaultUIWidth, (int)DefaultUIHeight, scale));

			RefreshRelatedControls();
		}

		#endregion

		#region Methods

		#endregion

		#region Static Fields

		public static int ScreenWidth
		{
			get
			{
				// returns Unity screen width (better to cache this somewhere)
				return Screen.width;
			}
		}

		public static int ScreenHeight
		{
			get
			{
				// returns Unity screen height (better to cache this somewhere)
				return Screen.height;
			}

			#endregion
		}
	}
}