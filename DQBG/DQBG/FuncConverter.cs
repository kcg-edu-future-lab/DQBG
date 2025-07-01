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
     * (2) Add the FuncConverter object to <Window.Resources> in the XAML file.
     * <local:FuncConverter x:Key="Scale3Converter" ToFunc="{x:Static local:MainWindow.Scale3}"/>
     * 
     * (3) Set the FuncConverter object to the Binding.Converter property.
     * <TextBlock Text="{Binding Width, Converter={StaticResource Scale3Converter}}"/>
     */

	/// <summary>
	/// Represents the value converter that uses the defined functions.
	/// </summary>
	[ValueConversion(typeof(object), typeof(object))]
	public class FuncConverter : DependencyObject, IValueConverter
	{
		public static readonly DependencyProperty ToFuncProperty =
			DependencyProperty.Register("ToFunc", typeof(MulticastDelegate), typeof(FuncConverter), new PropertyMetadata(null));

		public static readonly DependencyProperty FromFuncProperty =
			DependencyProperty.Register("FromFunc", typeof(MulticastDelegate), typeof(FuncConverter), new PropertyMetadata(null));

		/// <summary>
		/// Gets or sets the function to use in the <see cref="Convert"/> method.
		/// </summary>
		public MulticastDelegate ToFunc
		{
			get { return (MulticastDelegate)GetValue(ToFuncProperty); }
			set { SetValue(ToFuncProperty, value); }
		}

		/// <summary>
		/// Gets or sets the function to use in the <see cref="ConvertBack"/> method.
		/// </summary>
		public MulticastDelegate FromFunc
		{
			get { return (MulticastDelegate)GetValue(FromFuncProperty); }
			set { SetValue(FromFuncProperty, value); }
		}

		/// <summary>
		/// Converts a value using the function represented by the <see cref="ToFunc"/> property.
		/// </summary>
		/// <param name="value">The value produced by the binding source.</param>
		/// <param name="targetType">The type of the binding target property.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value.</returns>
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DoFunc(ToFunc, value, parameter);
		}

		/// <summary>
		/// Converts a value using the function represented by the <see cref="FromFunc"/> property.
		/// </summary>
		/// <param name="value">The value produced by the binding target.</param>
		/// <param name="targetType">The type to convert to.</param>
		/// <param name="parameter">The converter parameter to use.</param>
		/// <param name="culture">The culture to use in the converter.</param>
		/// <returns>A converted value.</returns>
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return DoFunc(FromFunc, value, parameter);
		}

		static object DoFunc(MulticastDelegate func, object value, object parameter)
		{
			if (func == null) return value;

			if (func.Method.ContainsGenericParameters) return Binding.DoNothing;

			var parameterInfoes = func.Method.GetParameters();
			return parameterInfoes.Length switch
			{
				0 => func.DynamicInvoke(),
				1 => func.DynamicInvoke(value),
				2 => func.DynamicInvoke(value, parameter),
				_ => Binding.DoNothing,
			};
		}
	}
}
