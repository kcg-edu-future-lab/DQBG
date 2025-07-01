using System.IO;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using Reactive.Bindings;

namespace DQBG
{
	public class AppModel
	{
		const string CsvFileName = "Characters.csv";
		static readonly Encoding UTF8N = new UTF8Encoding();

		const string FullDigits = "０１２３４５６７８９";
		internal static string ToFullString(int i) => string.Join("", i.ToString().Select(c => FullDigits[c - '0']));

		static readonly Character[] DefaultCharacters =
		[
			new Character("メンフクロウ", "menfukuro.png", 42, 42, 42, true, 14, "バギを　となえた！"),
			new Character("ぬりかべ", "nurikabe.png", 52, 40, 72, true, 18, "ヒャダルコを　となえた！"),
			new Character("サーバル", "serval.png", 47, 52, 43, false, 16, "ひのたまを　はいた！"),
			new Character("スライム", "slime.png", 56, 30, 35, true, 12, "ギラを　となえた！"),
			new Character("ワイバーン", "wyvern.png", 50, 55, 64, false, 19, "ほのおを　はいた！"),
			new Character("ゆきおんな", "yukionna.png", 48, 48, 48, false, 15, "ふぶきを　はいた！"),
		];

		static AppModel()
		{
			Array.ForEach(DefaultCharacters, c => c.Initialize());
		}

		static Character[] LoadCharacters()
		{
			var lines = File.ReadAllLines(CsvFileName, UTF8N);
			var csvColumns = Array.ConvertAll(lines[0].Split(','), s => s.Trim());

			var ctor = typeof(Character).GetConstructors()[0];
			var pars = ctor.GetParameters();

			return lines.Skip(1)
				.Select(l =>
				{
					var d = csvColumns.Zip(l.Split(',').Select(s => s.Trim())).ToDictionary(p => p.First, p => p.Second);
					var args = Array.ConvertAll(pars, p => Convert.ChangeType(d[p.Name], p.ParameterType));
					var c = (Character)ctor.Invoke(args);
					return c.Initialize();
				})
				.ToArray();
		}

		public bool IsInDesignMode { get; set; }

		public ReactiveProperty<Character[]> Characters { get; } = new();
		public ReactiveProperty<bool> IsReady { get; } = new(true);
		public ReactiveProperty<string> Message { get; } = new("");

		public Subject<bool> StartedEvent { get; } = new();
		public Subject<bool> OverEvent { get; } = new();
		public Subject<bool> SpellEvent { get; } = new();
		public Subject<bool> ExhaleEvent { get; } = new();

		public AppModel()
		{
			// For design mode.
			Task.Run(() =>
			{
				Thread.Sleep(3000);
				if (!IsInDesignMode) return;
				while (true)
				{
					Start();
					Thread.Sleep(3000);
				}
			});
		}

		Character[] GetCharacters()
		{
			if (IsInDesignMode) return DefaultCharacters;
			try
			{
				return LoadCharacters();
			}
			catch (Exception)
			{
				return DefaultCharacters;
			}
		}

		public void Start()
		{
			Characters.Value = null;
			IsReady.Value = false;
			Message.Value = "";
			Thread.Sleep(500);

			StartedEvent.OnNext(false);
			var chars = GetCharacters().Shuffle().Take(4).ToArray();
			foreach (var c in chars)
			{
				c.HP.Value = RandomHelper.NextInt(c.MaxHP, 0.93, 1);
			}
			Characters.Value = chars;
			Thread.Sleep(800);

			foreach (var c in chars)
			{
				Message.Value += $"{c.Name}が　あらわれた。\n";
				Thread.Sleep(500);
			}

			Message.Value = "";
			Thread.Sleep(500);

			while (true)
			{
				foreach (var c in chars.Shuffle().Where(c => c.IsAlive.Value))
				{
					var rate = RandomHelper.NextDouble(1);
					if (rate < 0.5)
						Attack(c);
					else
						Skill(c);

					if (chars.Count(c => c.IsAlive.Value) == 1)
					{
						OverEvent.OnNext(false);
						Message.Value = $"{c.Name}が　かちのこった！\n";
						Thread.Sleep(1000);

						IsReady.Value = true;
						return;
					}
				}
				Thread.Sleep(300);
			}
		}

		void Attack(Character c)
		{
			var target = Characters.Value.Shuffle().First(t => t != c && t.IsAlive.Value);

			c.AttackEvent.OnNext(false);
			Message.Value = $"{c.Name}の　こうげき！\n";
			Thread.Sleep(500);

			target.HitEvent.OnNext(false);
			var damageBase = c.Attack / 2.0 - target.Defence / 4.0;
			var damage = RandomHelper.NextInt(damageBase, 0.85, 1.15);
			if (damage < 1) damage = 1;
			Message.Value += $"{target.Name}に　{ToFullString(damage)}のダメージ！\n";
			Thread.Sleep(1000);

			target.HP.Value = Math.Max(0, target.HP.Value - damage);
			if (!target.IsAlive.Value)
			{
				Message.Value += $"{target.Name}を　やっつけた。\n";
				Thread.Sleep(700);
			}
			Message.Value = "";
			Thread.Sleep(300);
		}

		void Skill(Character c)
		{
			var messageBase = $"{c.Name}は　{c.SkillMessage}\n";

			(c.IsMagician ? SpellEvent : ExhaleEvent).OnNext(false);
			Message.Value = messageBase;
			Thread.Sleep(500);

			foreach (var target in Characters.Value.Where(t => t != c && t.IsAlive.Value))
			{
				Message.Value = messageBase;
				Thread.Sleep(200);

				target.HitEvent.OnNext(false);
				var damage = RandomHelper.NextInt(c.SkillDamage, 0.85, 1.15);
				if (damage < 1) damage = 1;
				Message.Value += $"{target.Name}に　{ToFullString(damage)}のダメージ！\n";
				Thread.Sleep(1000);

				target.HP.Value = Math.Max(0, target.HP.Value - damage);
				if (!target.IsAlive.Value)
				{
					Message.Value += $"{target.Name}を　やっつけた。\n";
					Thread.Sleep(700);
				}
			}
			Message.Value = "";
			Thread.Sleep(300);
		}
	}

	public record Character(string Name, string Icon, int MaxHP, int Attack, int Defence, bool IsMagician, int SkillDamage, string SkillMessage)
	{
		public string IconPath => @".\Images\" + Icon;
		public string IconFullPath => Path.GetFullPath(IconPath);
		public int CautionHP => MaxHP / 4;

		public ReactiveProperty<int> HP { get; } = new(0);
		public ReadOnlyReactiveProperty<bool> IsAlive { get; private set; }
		public ReadOnlyReactiveProperty<CharacterStatus> Status { get; private set; }

		public Subject<bool> AttackEvent { get; } = new();
		public Subject<bool> HitEvent { get; } = new();

		public Character Initialize()
		{
			IsAlive = HP
				.Select(hp => hp > 0)
				.ToReadOnlyReactiveProperty();
			Status = HP
				.Select(hp => hp <= 0 ? CharacterStatus.Dead : hp <= CautionHP ? CharacterStatus.Caution : CharacterStatus.Fine)
				.ToReadOnlyReactiveProperty();
			return this;
		}
	}

	public enum CharacterStatus
	{
		Fine, Caution, Dead
	}
}
