using System.Windows;

namespace DQBG
{
	public static class ViewFuncs
	{
		public static readonly Func<bool, Visibility> ToVisibleOrHidden = b => b ? Visibility.Visible : Visibility.Hidden;
		public static readonly Func<int, string> ToHPString = hp => AppModel.ToFullString(hp).PadLeft(3, '　');
		public static readonly Func<CharacterStatus, string> ToStatusColor = s => s == CharacterStatus.Dead ? "#CC3333" : s == CharacterStatus.Caution ? "#BB8800" : "#E0E0E0";
	}
}
