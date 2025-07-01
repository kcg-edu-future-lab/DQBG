namespace DQBG
{
	public static class RandomHelper
	{
		static readonly Random random = new();

		public static double NextDouble(double max) => random.NextDouble() * max;
		public static double NextDouble(double min, double max) => min + random.NextDouble() * (max - min);
		public static int NextInt(double value, double minRate, double maxRate) => (int)(value * NextDouble(minRate, maxRate));

		public static IEnumerable<T> Shuffle<T>(this T[] a)
		{
			var b = (T[])a.Clone();
			for (var i = a.Length - 1; i >= 0; --i)
			{
				var j = random.Next(i + 1);
				yield return b[j];
				b[j] = b[i];
			}
		}
	}
}
