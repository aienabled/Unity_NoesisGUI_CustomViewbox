namespace WpfApplication1.Controls
{
	#region

	using System;

	using Noesis;

	#endregion

	public class GameWindow : ContentControl
	{
		#region Static Fields

		public static DependencyProperty CloseByEscapeKeyProperty = DependencyProperty.Register(
			"CloseByEscapeKey",
			typeof(bool),
			typeof(GameWindow),
			new PropertyMetadata(false));

		public static DependencyProperty FocusOnControlProperty = DependencyProperty.Register(
			"FocusOnControl",
			typeof(FrameworkElement),
			typeof(GameWindow),
			new PropertyMetadata(default(FrameworkElement)));

		public static DependencyProperty IsModalProperty = DependencyProperty.Register(
			"IsModal",
			typeof(bool),
			typeof(GameWindow),
			new PropertyMetadata(true));

		public static DependencyProperty WindowHeightProperty = DependencyProperty.Register(
			"WindowHeight",
			typeof(float),
			typeof(GameWindow),
			new PropertyMetadata(default(float)));

		public static DependencyProperty WindowWidthProperty = DependencyProperty.Register(
			"WindowWidth",
			typeof(float),
			typeof(GameWindow),
			new PropertyMetadata(default(float)));

		#endregion

		#region Fields

		public uint DisplayDelayMs;

		private ColumnDefinition columnStretch1;

		private ColumnDefinition columnStretch2;

		private string currrentWindowName;

		private bool isSetFocus = true;

		private Grid layoutRoot;

		private FrameworkElement parentInstance;

		private Storyboard sbHide;

		private Storyboard sbShow;

		private string specialWindowName;

		#endregion

		#region Constructors and Destructors

		static GameWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GameWindow), new FrameworkPropertyMetadata(typeof(GameWindow)));
		}

		public GameWindow()
		{
		}

		#endregion

		#region Public Events

		public event Action<GameWindow> Closed;

		public event Action<GameWindow> Opened;

		public event Action SetFocus;

		#endregion

		#region Enums

		public enum WindowState
		{
			Undefined,

			Opened,

			Closed
		}

		#endregion

		#region Public Properties

		public bool CloseByEscapeKey
		{
			get
			{
				return (bool)this.GetValue(CloseByEscapeKeyProperty);
			}
			set
			{
				this.SetValue(CloseByEscapeKeyProperty, value);
			}
		}

		public int CurrentZIndex { get; set; }

		public FrameworkElement FocusOnControl
		{
			get
			{
				return (FrameworkElement)this.GetValue(FocusOnControlProperty);
			}
			set
			{
				this.SetValue(FocusOnControlProperty, value);
			}
		}

		public bool IsCached { get; set; }

		public bool IsClosing { get; private set; }

		public bool IsClosingOrClosed
		{
			get
			{
				return !this.IsOpenedOrOpening;
			}
		}

		public bool IsModal
		{
			get
			{
				return (bool)this.GetValue(IsModalProperty);
			}

			set
			{
				this.SetValue(IsModalProperty, value);
			}
		}

		public bool IsOpenedOrOpening { get; private set; }

		public bool IsOpening { get; private set; }

		public FrameworkElement ParentInstance
		{
			get
			{
				return this.parentInstance;
			}
		}

		public WindowState SelectedWindowState { get; private set; }

		public float WindowHeight
		{
			get
			{
				return (float)this.GetValue(WindowHeightProperty);
			}

			set
			{
				this.SetValue(WindowHeightProperty, value);
			}
		}

		public string WindowName
		{
			get
			{
				return this.currrentWindowName;
			}
			private set
			{
				this.currrentWindowName = value;
			}
		}

		public float WindowWidth
		{
			get
			{
				return (float)this.GetValue(WindowWidthProperty);
			}

			set
			{
				this.SetValue(WindowWidthProperty, value);
			}
		}

		public int ZIndexOffset { get; set; }

		#endregion

		#region Public Methods and Operators

		public void Close()
		{
			//InputBlocker.Block();

			this.SelectedWindowState = WindowState.Closed;
			if (this.IsClosingOrClosed)
			{
				return;
			}

			this.IsClosing = true;

			if (this.IsOpening)
			{
				this.sbShow.Stop(this.layoutRoot);
				this.IsOpening = false;
			}

			this.IsOpenedOrOpening = false;

			this.sbHide.Begin(this.layoutRoot);
		}

		public void OnPostInit()
		{
			this.IsOpenedOrOpening = false;

			var templateRoot = VisualTreeHelper.GetChild(this, 0) as FrameworkElement;
			this.layoutRoot = (Grid)templateRoot.FindName("LayoutRoot");
			this.sbHide = (Storyboard)templateRoot.FindResource("SbHide");
			this.sbShow = (Storyboard)templateRoot.FindResource("SbShow");

			// this used for measurement hack
			this.columnStretch1 = (ColumnDefinition)templateRoot.FindName("ColumnStretch1");
			this.columnStretch2 = (ColumnDefinition)templateRoot.FindName("ColumnStretch2");

			if (float.IsNaN(this.WindowHeight)
			    || float.IsNaN(this.WindowWidth)
			    || this.WindowHeight < 1
			    || this.WindowWidth < 1)
			{
				this.CalcAndSetWindowSize();
			}

			// binding don't work! so set values from here
			var viewbox = (ScreenViewbox)templateRoot.FindName("Viewbox");
			viewbox.TargetWidth = this.WindowWidth;
			viewbox.TargetHeight = this.WindowHeight;

			this.Loaded += this.OnLoaded;
		}

		public void Open()
		{
			this.SelectedWindowState = WindowState.Opened;
			if (this.IsOpening
			    || this.IsOpenedOrOpening)
			{
				return;
			}

			this.IsOpening = true;
			if (this.IsClosing)
			{
				this.IsClosing = false;
				this.sbHide.Stop(this.layoutRoot);
			}

			this.IsOpenedOrOpening = true;

			this.IsEnabled = true;

			if (this.parentInstance != null)
			{
				this.parentInstance.Visibility = Visibility.Visible;
			}

			this.sbShow.Begin(this.layoutRoot);
		}

		public void SetSpecialWindowName(string name)
		{
			this.specialWindowName = name;
			this.WindowName = name;
		}

		#endregion

		#region Methods

		private void CalcAndSetWindowSize()
		{
			var setWidth = true;
			var setHeight = true;
			var availableHeight = UISizeHelper.DefaultUIHeight;
			var availableWidth = UISizeHelper.DefaultUIWidth;
			if (!float.IsNaN(this.WindowHeight)
			    && this.WindowHeight > 0)
			{
				availableHeight = this.WindowHeight;
				setHeight = false;
			}

			if (!float.IsNaN(this.WindowWidth)
			    && this.WindowWidth > 0)
			{
				availableWidth = this.WindowWidth;
				setWidth = false;
			}

			var gridLengthAuto = new GridLength(1, GridUnitType.Auto);
			this.columnStretch1.Width = gridLengthAuto;
			this.columnStretch2.Width = gridLengthAuto;

			this.layoutRoot.Measure(new Size(availableWidth, availableHeight));

			var gridLengthStar = new GridLength(1, GridUnitType.Star);
			this.columnStretch1.Width = gridLengthStar;
			this.columnStretch2.Width = gridLengthStar;

			var desiredSize = this.layoutRoot.DesiredSize;
			if (setWidth)
			{
				// add margins also
				this.WindowWidth = desiredSize.Width + 24;
			}

			if (setHeight)
			{
				// add margins also
				this.WindowHeight = desiredSize.Height + 26;
			}
		}

		void OnLoaded(object sender, RoutedEventArgs e)
		{
			this.sbShow.Completed += this.SbShowCompletedHandler;
			this.sbHide.Completed += this.SbHideCompletedHandler;

			if (this.parentInstance == null)
			{
				this.parentInstance = LogicalTreeHelper.GetParent(this);

				if (this.parentInstance == null)
				{
					// this means window defined in ControlTemplate
					var visual = VisualTreeHelper.GetParent(this);
					this.parentInstance = visual != null ? visual as FrameworkElement : null;
					if (this.parentInstance == null)
					{
						return;
					}
				}

				this.parentInstance.Visibility = this.IsOpenedOrOpening ? Visibility.Visible : Visibility.Hidden;
			}

			if (this.specialWindowName != null)
			{
				this.currrentWindowName = this.specialWindowName;
			}
			else if (this.parentInstance == null)
			{
				this.currrentWindowName = @"without ParentInstance";
			}
			else
			{
				var name = this.parentInstance.Name;
				this.currrentWindowName = string.IsNullOrEmpty(name) ? @"no name window" : name;
			}

			this.Open();
		}

		private void SbHideCompletedHandler(object sender, TimelineEventArgs timelineEventArgs)
		{
			if (timelineEventArgs.Target != this.layoutRoot)
			{
				return;
			}

			this.IsClosing = false;
			// do something
		}

		private void SbShowCompletedHandler(object sender, TimelineEventArgs timelineEventArgs)
		{
			if (timelineEventArgs.Target != this.layoutRoot)
			{
				return;
			}

			this.IsOpening = false;

			if (this.IsClosing)
			{
				//Global.Logger.Write("Window " + this.WindowName + " will be not opened as it closing", LogSeverity.Important);
				return;
			}

			this.IsOpenedOrOpening = true;

			var handler = this.Opened;
			if (handler != null)
			{
				handler(this);
			}
		}

		#endregion
	}
}