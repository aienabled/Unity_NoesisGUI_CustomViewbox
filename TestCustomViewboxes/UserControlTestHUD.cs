namespace WpfApplication1
{
	#region

	using Noesis;

	using UserControl = System.Windows.Controls.UserControl;

	#endregion

	[UserControlSource(XamlControlPath)]
	public class UserControlTestHUD : UserControl
	{
		#region Constants

		public const string XamlControlPath = "TestWindow/UserControlTestHUD.xaml";

		#endregion

		#region Constructors and Destructors

		public UserControlTestHUD()
		{
			this.InitializeComponent();
			this.DataContext = new TestViewModelUIScale();
		}

		#endregion
	}
}