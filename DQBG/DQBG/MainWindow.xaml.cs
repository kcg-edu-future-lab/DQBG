using System.Reactive.Linq;
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
using Reactive.Bindings.Extensions;

namespace DQBG
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Sounds
		const double DefaultVolume = 0.5;
		const string BgmMP3Name = "Sounds/BGM.mp3";
		const string OverMP3Name = "Sounds/Over.mp3";
		const string AttackMP3Name = "Sounds/Attack.mp3";
		const string SpellMP3Name = "Sounds/Spell.mp3";
		const string ExhaleMP3Name = "Sounds/Exhale.mp3";
		const string HitMP3Name = "Sounds/Hit.mp3";
		//const string MissMP3Name = "Sounds/Miss.mp3";

		MediaElement currentBGM;
		MediaElement currentSE;

		void PlayBgm(bool silent = false)
		{
			if (currentBGM != null) currentBGM.Source = null;
			BgmMedia.Volume = silent ? 0 : DefaultVolume;
			BgmMedia.Source = new Uri(BgmMP3Name, UriKind.Relative);
			currentBGM = BgmMedia;
		}
		void PlayOver(bool silent = false)
		{
			if (currentBGM != null) currentBGM.Source = null;
			OverMedia.Volume = silent ? 0 : DefaultVolume;
			OverMedia.Source = new Uri(OverMP3Name, UriKind.Relative);
			currentBGM = OverMedia;
		}
		void PlayAttack(bool silent = false)
		{
			if (currentSE != null) currentSE.Source = null;
			AttackMedia.Volume = silent ? 0 : DefaultVolume;
			AttackMedia.Source = new Uri(AttackMP3Name, UriKind.Relative);
			currentSE = AttackMedia;
		}
		void PlaySpell(bool silent = false)
		{
			if (currentSE != null) currentSE.Source = null;
			SpellMedia.Volume = silent ? 0 : DefaultVolume;
			SpellMedia.Source = new Uri(SpellMP3Name, UriKind.Relative);
			currentSE = SpellMedia;
		}
		void PlayExhale(bool silent = false)
		{
			if (currentSE != null) currentSE.Source = null;
			ExhaleMedia.Volume = silent ? 0 : DefaultVolume;
			ExhaleMedia.Source = new Uri(ExhaleMP3Name, UriKind.Relative);
			currentSE = ExhaleMedia;
		}
		void PlayHit(bool silent = false)
		{
			//if (currentSE != null) currentSE.Source = null;
			HitMedia.Volume = silent ? 0 : DefaultVolume;
			HitMedia.Source = new Uri(HitMP3Name, UriKind.Relative);
			currentSE = HitMedia;
		}
		#endregion

		readonly AppModel model;

		public MainWindow()
		{
			InitializeComponent();
		}

		void Window_Loaded(object sender, RoutedEventArgs e)
		{
			PlayBgm(true);
			PlayOver(true);
			PlayAttack(true);
			PlaySpell(true);
			PlayExhale(true);
			PlayHit(true);
		}

		void CharacterImage_Loaded(object sender, RoutedEventArgs e)
		{
			var f = (FrameworkElement)sender;
			var c = (Character)f.DataContext;

			var attackSub = c.AttackEvent
				.ObserveOnUIDispatcher()
				.Subscribe(_ =>
				{
					PlayAttack();
					//BeginAnimation("AttackSB", f);
				});
			var hitSub = c.HitEvent
				.ObserveOnUIDispatcher()
				.Subscribe(_ =>
				{
					PlayHit();
					//BeginAnimation("HitSB", f);
				});

			f.Unloaded += (_, _) =>
			{
				attackSub.Dispose();
				hitSub.Dispose();
			};
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
