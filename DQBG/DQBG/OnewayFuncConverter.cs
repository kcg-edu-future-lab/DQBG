using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DQBG
{
	/* 
     * Usage Example
     * 
     * (1) Define the function on the code.
     * public static readonly Func<double, double> Scale3 = x => 3 * x;
     * 
     * (2) Add the OnewayFuncConverter object to <Window.Resources> in the XAML file.
     * <local:OnewayFuncConverter x:Key="OnewayFuncConverter"/>
     * 
     * (3) Set the OnewayFuncConverter object to the Binding.Converter property, and the function to the Binding.ConverterParameter property.
     * <TextBlock Text="{Binding Width, Converter={StaticResource OnewayFuncConverter}, ConverterParameter={x:Static local:MainWindow.Scale3}}"/>
     */

	/// <summary>
	/// Represents the value converter that uses the defined function.
	/// </summary>
	[ValueConversion(typeof(object), typeof(object))]
	public class OnewayFuncConverter : DependencyObject, IValueConverter
	{
		/// <summary>
		/// Converts a value using the function represented by the <paramref name="parameter"/>.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The function to convert a value.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (parameter == null) return value;

			if (parameter is not MulticastDelegate func) return Binding.DoNothing;
			if (func.Method.ContainsGenericParameters) return Binding.DoNothing;

			var parameterInfoes = func.Method.GetParameters();
			return parameterInfoes.Length switch
			{
				0 => func.DynamicInvoke(),
				1 => func.DynamicInvoke(value),
				_ => Binding.DoNothing,
			};
		}

		/// <summary>
		/// Does nothing.
		/// </summary>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Binding.DoNothing;
		}
	}
}
