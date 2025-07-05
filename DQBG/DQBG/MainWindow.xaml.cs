using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DQBG
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		#region Common Methods
		public void BeginAnimation(string storyboardKey) => BeginAnimation(storyboardKey, this);
		public void BeginAnimation(string storyboardKey, FrameworkElement f)
		{
			var sb = (Storyboard)Resources[storyboardKey];
			sb.Begin(f);
		}

		public static Storyboard CloneStoryboard(Storyboard storyboard, string targetName)
		{
			var sb = storyboard.Clone();
			foreach (var tl in sb.Children)
				Storyboard.SetTargetName(tl, targetName);
			return sb;
		}
		#endregion
	}
}
