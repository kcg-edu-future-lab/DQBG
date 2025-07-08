using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DQBG
{
	public static class UIHelper
	{
		public static void BeginAnimation(this FrameworkElement f, string storyboardKey)
		{
			var sb = (Storyboard)f.Resources[storyboardKey];
			// sb.Begin() でも可能。
			sb.Begin(f);
		}

		// defined で定義されている Storyboard を、target 上で実行します。
		public static void BeginAnimation(this FrameworkElement target, FrameworkElement defined, string storyboardKey)
		{
			var sb = (Storyboard)defined.Resources[storyboardKey];
			sb.Begin(target);
		}

		public static Storyboard CloneStoryboard(Storyboard storyboard, string targetName)
		{
			var sb = storyboard.Clone();
			foreach (var tl in sb.Children)
				Storyboard.SetTargetName(tl, targetName);
			return sb;
		}

		public static void RunOnUIDispatcher(Action action) => RunOnUIDispatcher(Application.Current, action);
		public static void RunOnUIDispatcher(this DispatcherObject obj, Action action)
		{
			if (action != null) obj.Dispatcher.InvokeAsync(action);
		}
	}
}
