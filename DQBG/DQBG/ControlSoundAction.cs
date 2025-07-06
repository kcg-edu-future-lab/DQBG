using System.Windows;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;

namespace DQBG
{
	public class ControlSoundAction : TriggerAction<DependencyObject>
	{
		public static readonly DependencyProperty SourceProperty =
			DependencyProperty.Register("Source", typeof(Uri), typeof(ControlSoundAction), new PropertyMetadata(OnSourceChanged));

		public static readonly DependencyProperty SourceKeyProperty =
			DependencyProperty.Register("SourceKey", typeof(string), typeof(ControlSoundAction), new PropertyMetadata(OnSourceChanged));

		public static readonly DependencyProperty ControlTypeProperty =
			DependencyProperty.Register("ControlType", typeof(SoundControlType), typeof(ControlSoundAction), new PropertyMetadata(SoundControlType.Play));

		public static readonly DependencyProperty VolumeProperty =
			DependencyProperty.Register("Volume", typeof(double), typeof(ControlSoundAction), new PropertyMetadata(0.5));

		public Uri Source
		{
			get { return (Uri)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}

		// 同一の音源を複数のインスタンスで利用する場合に設定します。
		public string SourceKey
		{
			get { return (string)GetValue(SourceKeyProperty); }
			set { SetValue(SourceKeyProperty, value); }
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
		string MapKey => string.IsNullOrWhiteSpace(SourceKey) ? Source.OriginalString : SourceKey;

		static void OnSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
		{
			if (sender is ControlSoundAction action) action.OnSourceChanged(e);
		}

		void OnSourceChanged(DependencyPropertyChangedEventArgs e)
		{
			if (Source == null) return;
			var mapKey = MapKey;
			if (PlayerMap.ContainsKey(mapKey)) return;

			var player = new MediaPlayer();
			// この方法では雑音が出ます。
			//player.MediaEnded += (_, _) => player.Stop();
			player.Volume = 0;
			player.Open(Source);
			PlayerMap[mapKey] = player;
		}

		protected override void Invoke(object parameter)
		{
			var player = PlayerMap[MapKey];
			switch (ControlType)
			{
				case SoundControlType.Play:
					player.Stop();
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
