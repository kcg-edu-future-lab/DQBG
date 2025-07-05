using System.Windows;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace DQBG
{
	public class ControlSoundAction : TriggerAction<DependencyObject>
	{
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(Uri), typeof(ControlSoundAction), new PropertyMetadata(OnSourceChanged));

		public static readonly DependencyProperty ControlTypeProperty =
			DependencyProperty.Register("ControlType", typeof(SoundControlType), typeof(ControlSoundAction), new PropertyMetadata(SoundControlType.Play));

		public static readonly DependencyProperty VolumeProperty =
			DependencyProperty.Register("Volume", typeof(double), typeof(ControlSoundAction), new PropertyMetadata(0.5));

		public Uri Source
		{
			get { return (Uri)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		public SoundControlType ControlType
		{
			get { return (SoundControlType)GetValue(ControlTypeProperty); }
			set { SetValue(ControlTypeProperty, value); }
		}

		public double Volume
		{
			get { return (double)GetValue(VolumeProperty); }
			set { SetValue(VolumeProperty, value); }
		}

		static readonly Dictionary<string, MediaPlayer> PlayerMap = [];

		static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			var uri = (Uri)e.NewValue;
			if (PlayerMap.ContainsKey(uri.OriginalString)) return;

			var player = new MediaPlayer();
			player.MediaEnded += (_, _) => player.Stop();
			player.Volume = 0;
			player.Open(uri);
			PlayerMap[uri.OriginalString] = player;
		}

		protected override void Invoke(object parameter)
		{
			var player = PlayerMap[Source.OriginalString];
			switch (ControlType)
			{
				case SoundControlType.Play:
					player.Volume = Volume;
					player.Play();
					break;
				case SoundControlType.Stop:
					player.Stop();
					break;
				case SoundControlType.Pause:
					player.Pause();
					break;
				// not tested
				case SoundControlType.TogglePlayPause:
					if (player.CanPause) player.Pause();
					else player.Play();
					break;
				default:
					break;
			}
		}
	}

	public enum SoundControlType
	{
		Play,
		Stop,
		Pause,
		// not tested
		TogglePlayPause,
	}
}
