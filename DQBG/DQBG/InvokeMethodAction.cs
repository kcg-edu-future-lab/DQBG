using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace DQBG
{
	public class InvokeMethodAction : TriggerAction<DependencyObject>
	{
		public static readonly DependencyProperty TargetObjectProperty =
			DependencyProperty.Register("TargetObject", typeof(object), typeof(InvokeMethodAction));

		public static readonly DependencyProperty MethodNameProperty =
			DependencyProperty.Register("MethodName", typeof(string), typeof(InvokeMethodAction), new PropertyMetadata(""));

		public static readonly DependencyProperty IsAsyncProperty =
			DependencyProperty.Register("IsAsync", typeof(bool), typeof(InvokeMethodAction), new PropertyMetadata(false));

		public object TargetObject
		{
			get { return (object)GetValue(TargetObjectProperty); }
			set { SetValue(TargetObjectProperty, value); }
		}

		public string MethodName
		{
			get { return (string)GetValue(MethodNameProperty); }
			set { SetValue(MethodNameProperty, value); }
		}

		public bool IsAsync
		{
			get { return (bool)GetValue(IsAsyncProperty); }
			set { SetValue(IsAsyncProperty, value); }
		}

		protected override void Invoke(object parameter)
		{
			var method = TargetObject?.GetType().GetMethod(MethodName);
			if (method == null) return;
			//if (method.ContainsGenericParameters) return;

			// Copy objects for an async thread.
			var target = TargetObject;
			var associated = AssociatedObject;

			if (IsAsync)
				Task.Run(InvokeMethod);
			else
				InvokeMethod();

			void InvokeMethod()
			{
				var pars = method.GetParameters();
				var r = pars.Length switch
				{
					0 => method.Invoke(target, []),
					1 => method.Invoke(target, [parameter]),
					2 => method.Invoke(target, [associated, parameter]),
					_ => null,
				};
			}
		}
	}
}
