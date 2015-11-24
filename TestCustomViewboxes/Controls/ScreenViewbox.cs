namespace WpfApplication1.Controls
{
	#region

	using System;
	using System.Collections.Generic;

	using Noesis;

	#endregion

	/// <summary>
	///     This control simply scale its content proportionally to the screen size
	/// </summary>
	public class ScreenViewbox : ContentControl
	{
		#region Static Fields

		// just for testing keep weak references on all instantiated screen viewboxes
		public static readonly List<WeakReference> Instances = new List<WeakReference>();

		public static DependencyProperty TargetHeightProperty = DependencyProperty.Register(
			"TargetHeight",
			typeof(float),
			typeof(ScreenViewbox),
			new PropertyMetadata(100f));

		public static DependencyProperty TargetWidthProperty = DependencyProperty.Register(
			"TargetWidth",
			typeof(float),
			typeof(ScreenViewbox),
			new PropertyMetadata(100f));

		#endregion

		#region Fields

		private ContentPresenter contentPresenter;

		private ScaleTransform contentPresenterScaleTransform;

		#endregion

		#region Constructors and Destructors

		static ScreenViewbox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenViewbox), new FrameworkPropertyMetadata(typeof(ScreenViewbox)));
		}

		public ScreenViewbox()
		{
		}

		#endregion

		#region Public Properties

		public float TargetHeight
		{
			get
			{
				return (float)this.GetValue(TargetHeightProperty);
			}
			set
			{
				this.SetValue(TargetHeightProperty, value);
			}
		}

		public float TargetWidth
		{
			get
			{
				return (float)this.GetValue(TargetWidthProperty);
			}
			set
			{
				this.SetValue(TargetWidthProperty, value);
			}
		}

		#endregion

		#region Public Methods and Operators

		public void OnPostInit()
		{
			var templateRoot = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
			this.contentPresenter = (ContentPresenter)templateRoot.FindName("ContentPresenter");
			this.contentPresenterScaleTransform = new ScaleTransform(1f, 1f);
			this.contentPresenter.RenderTransform = this.contentPresenterScaleTransform;

			this.Loaded += this.OnLoaded;

			// just for testing
			Instances.Add(new WeakReference(this));

			this.RefreshViewBoxMargins();
		}

		public void RefreshSize()
		{
			this.RefreshViewBoxMargins();
		}

		#endregion

		#region Methods

		private void RefreshViewBoxMargins()
		{
			var screenWidth = (float)UISizeHelper.ScreenWidth;
			var screenHeight = (float)UISizeHelper.ScreenHeight;

			var targetWidth = this.TargetWidth;
			var targetHeight = this.TargetHeight;

			float scale;
			{
				// calculate scale
				var scaleX = screenWidth / UISizeHelper.DefaultUIWidth;
				var scaleY = screenHeight / UISizeHelper.DefaultUIHeight;

				scale = Math.Min(scaleX, scaleY);

				if (targetWidth * scale > screenWidth)
				{
					// limit scale to not exceed screen boundaries
					scale = screenWidth / targetWidth;
				}

				if (targetHeight * scale > screenHeight)
				{
					// limit scale to not exceed screen boundaries
					scale = screenHeight / targetHeight;
				}
			}

			this.contentPresenter.Width = targetWidth;
			this.contentPresenter.Height = targetHeight;

			// calculate offsets
			var left = (screenWidth - targetWidth * scale) / 2f;
			var top = (screenHeight - targetHeight * scale) / 2f;

			// set offsets
			Canvas.SetLeft(this.contentPresenter, left);
			Canvas.SetTop(this.contentPresenter, top);

			// apply scale
			this.contentPresenterScaleTransform.ScaleX = scale;
			this.contentPresenterScaleTransform.ScaleY = scale;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.RefreshViewBoxMargins();
		}

		#endregion
	}
}