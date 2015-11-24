namespace WpfApplication1
{
	#region

	using Noesis;

	using UserControl = System.Windows.Controls.UserControl;

	#endregion

	[UserControlSource(XamlControlPath)]
	public class UserControlTestWindow : UserControl
	{
		#region Constants

		public const string XamlControlPath = "TestWindow/UserControlTestWindow.xaml";

		#endregion

		#region Constructors and Destructors

		public UserControlTestWindow()
		{
			this.InitializeComponent();
			this.DataContext = new TestViewModelUIScale();
		}

		#endregion
	}
}