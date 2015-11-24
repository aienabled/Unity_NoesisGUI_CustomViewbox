#region

using UnityEngine;

using WpfApplication1.Controls;

#endregion

public class ScreenSizeWatcher : MonoBehaviour
{
	#region Fields

	private int lastScreenHeight;

	private int lastScreenWidth;

	#endregion

	#region Methods

	private void Awake()
	{
		this.RememberScreenSize();
	}

	private void RememberScreenSize()
	{
		this.lastScreenHeight = UISizeHelper.ScreenHeight;
		this.lastScreenWidth = UISizeHelper.ScreenWidth;
	}

	private void Update()
	{
		if (UISizeHelper.ScreenHeight == this.lastScreenHeight
		    && UISizeHelper.ScreenWidth == this.lastScreenWidth)
		{
			return;
		}

		this.RememberScreenSize();

		UISizeHelper.RefreshRelatedControls();
	}

	#endregion
}