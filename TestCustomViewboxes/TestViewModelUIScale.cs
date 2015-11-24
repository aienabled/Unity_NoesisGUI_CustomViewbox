namespace WpfApplication1
{
	#region

	using System;

	using UnityEngine;

	using WpfApplication1.Controls;

	#endregion

	public class TestViewModelUIScale
	{
		#region Fields

		private float sliderScale = 1f;

		#endregion

		#region Constructors and Destructors

		public TestViewModelUIScale()
		{
		}

		#endregion

		#region Public Properties

		public float SliderScale
		{
			get
			{
				return this.sliderScale;
			}
			set
			{
				this.sliderScale = value;
				Debug.LogWarning("Scale changed: " + value);

				// round it
				var val = (float)Math.Round(this.sliderScale * 100) / 100f;

				val = Mathf.Clamp(val, 0.6f, 1.4f);

				UISizeHelper.SetUIScale(val);
			}
		}

		#endregion
	}
}