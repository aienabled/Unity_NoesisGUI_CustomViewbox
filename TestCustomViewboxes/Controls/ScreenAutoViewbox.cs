namespace WpfApplication1.Controls
{
	#region

	using System;
	using System.Collections.Generic;

	using Noesis;

	#endregion

	/// <summary>
	///     This control scale its content proportionally to the screen size and with accordance to its multiplier factor.
	///		It will also properly align control according to Horizontal/Vertical alignment and takes margins into account
	/// </summary>
	public class ScreenAutoViewbox : ContentControl
	{
		#region Static Fields

		// just for testing keep weak references on all instantiated screen viewboxes
		public static readonly List<WeakReference> Instances = new List<WeakReference>();

		public static DependencyProperty ScaleMultProperty = DependencyProperty.Register(
			"ScaleMult",
			typeof(float),
			typeof(ScreenAutoViewbox),
			new PropertyMetadata(1f));

		#endregion

		#region Fields

		private ContentPresenter contentPresenter;

		private ScaleTransform contentPresenterScaleTransform;

		private FrameworkElement currentChild;

		private float lastMeasuredHeight;

		private float lastMeasuredWidth;

		#endregion

		#region Constructors and Destructors

		static ScreenAutoViewbox()
		{
			DefaultStyleKeyProperty.OverrideMetadata(
				typeof(ScreenAutoViewbox),
				new FrameworkPropertyMetadata(typeof(ScreenAutoViewbox)));
		}

		public ScreenAutoViewbox()
		{
		}

		#endregion

		#region Public Properties

		public float ScaleMult
		{
			get
			{
				return (float)this.GetValue(ScaleMultProperty);
			}
			set
			{
				this.SetValue(ScaleMultProperty, value);
			}
		}

		#endregion

		#region Public Methods and Operators

		public override Size GetMeasureConstraint()
		{
			return new Size(this.lastMeasuredWidth, this.lastMeasuredHeight);
		}

		public void OnPostInit()
		{
			var templateRoot = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
			this.contentPresenter = (ContentPresenter)templateRoot.FindName("ContentPresenter");
			this.contentPresenterScaleTransform = new ScaleTransform(1f, 1f);
			this.contentPresenter.RenderTransform = this.contentPresenterScaleTransform;

			this.Loaded += this.OnLoaded;
			this.Unloaded += this.OnUnloaded;

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

		private void OnCurrentChildOnSizeChanged(object o, SizeChangedEventArgs e)
		{
			this.RefreshViewBoxMargins();
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			this.SizeChanged += this.ScreenAutoViewboxSizeChangedHandler;
			this.RefreshViewBoxMargins();
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			this.SizeChanged -= this.ScreenAutoViewboxSizeChangedHandler;
		}

		private void RefreshViewBoxMargins()
		{
			var child = (FrameworkElement)this.contentPresenter.Content;
			if (this.currentChild != child)
			{
				// child changed
				if (this.currentChild != null)
				{
					this.currentChild.SizeChanged -= this.OnCurrentChildOnSizeChanged;
				}

				this.currentChild = child;
				this.currentChild.SizeChanged += this.OnCurrentChildOnSizeChanged;
			}

			var margin = child.Margin;
			var targetWidth = child.ActualWidth + margin.Left + margin.Right;
			var targetHeight = child.ActualHeight + margin.Top + margin.Bottom;

			var scale = Math.Min(
				UISizeHelper.ScreenWidth / UISizeHelper.DefaultUIWidth,
				UISizeHelper.ScreenHeight / UISizeHelper.DefaultUIHeight);

			scale *= this.ScaleMult;

			var frameActualWidth = this.ActualWidth;
			var frameActualHeight = this.ActualHeight;

			var childActualWidth = targetWidth * scale;
			var childActualHeight = targetHeight * scale;

			float left, top;
			switch (child.HorizontalAlignment)
			{
				// center
				default:
					left = (frameActualWidth - childActualWidth) / 2f;
					break;

				case HorizontalAlignment.Left:
					left = 0;
					break;

				case HorizontalAlignment.Right:
					left = frameActualWidth - childActualWidth;
					break;
			}

			switch (child.VerticalAlignment)
			{
				// center
				default:
					top = (frameActualHeight - childActualHeight) / 2f;
					break;

				case VerticalAlignment.Top:
					top = 0;
					break;

				case VerticalAlignment.Bottom:
					top = frameActualHeight - childActualHeight;
					break;
			}

			// set offset
			Canvas.SetLeft(this.contentPresenter, left);
			Canvas.SetTop(this.contentPresenter, top);

			// set size
			this.lastMeasuredWidth = childActualWidth;
			this.lastMeasuredHeight = childActualHeight;

			// set scale
			this.contentPresenterScaleTransform.ScaleX = scale;
			this.contentPresenterScaleTransform.ScaleY = scale;
		}

		private void ScreenAutoViewboxSizeChangedHandler(object o, SizeChangedEventArgs e)
		{
			this.RefreshViewBoxMargins();
		}

		#endregion
	}
}