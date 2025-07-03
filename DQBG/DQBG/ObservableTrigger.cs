using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.Xaml.Behaviors;
using Reactive.Bindings.Extensions;

namespace DQBG
{
	public class ObservableTrigger : TriggerBase<FrameworkElement>
	{
		public static readonly DependencyProperty ObservableProperty =
			DependencyProperty.Register("Observable", typeof(object), typeof(ObservableTrigger), new PropertyMetadata(null));

		public object Observable
		{
			get { return (object)GetValue(ObservableProperty); }
			set { SetValue(ObservableProperty, value); }
		}

		static readonly MethodInfo SubscribeMethod =
			typeof(ObservableTrigger).GetMethod(nameof(Subscribe), BindingFlags.NonPublic | BindingFlags.Instance);

		void Subscribe<T>(IObservable<T> observable)
		{
			var sub = observable
				.ObserveOnUIDispatcher()
				.Subscribe(o => InvokeActions(o));
			AssociatedObject.Unloaded += (_, _) => sub.Dispose();
		}

		protected override void OnAttached()
		{
			base.OnAttached();

			// Asserts Observable is IObservable<T>.
			if (Observable == null) return;
			var type = Observable.GetType().GetTypeInfo();
			var io_t = type.ImplementedInterfaces.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IObservable<>));
			if (io_t == null) return;

			SubscribeMethod
				.MakeGenericMethod(io_t.GenericTypeArguments[0])
				.Invoke(this, [Observable]);
		}
	}
}
