#region

using System.Reactive.Disposables;
using Android.OS;
using Android.Support.V7.App;
using Autofac;

#endregion

namespace Nyanto
{
	public abstract class AppCompatActivityBase : AppCompatActivity
	{
		private ILifetimeScope _lifetimeScope;
		protected CompositeDisposable CompositeDisposable;
		public ApplicationBase NyantoApplication => Application as ApplicationBase;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			CompositeDisposable = new CompositeDisposable();
			GetComponentContext().InjectProperties(this);
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			CompositeDisposable.Dispose();
		}

		public ILifetimeScope GetComponentContext()
		{
			if (_lifetimeScope == null)
			{
				_lifetimeScope = LifeTimeProviders.Of(this, ConfigurationAction).Get(typeof(ILifetimeScope));
			}
				
			return _lifetimeScope;
		}

		protected abstract void ConfigurationAction(ContainerBuilder containerBuilder);
	}
}