#region

using System;
using Android.App;
using Android.Runtime;
using Autofac;
using Nyanto.Core;

#endregion

namespace Nyanto
{
	public abstract class ApplicationBase : Application
	{
		private readonly ActivityLifecycleManager _lifecycleManager = new ActivityLifecycleManager();

		public ApplicationBase(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
		{
		}

		public IContainer ApplicationComponentContext { get; private set; }

		public ActivityLifeCycycle GetActivityState(Activity activity)
		{
			return _lifecycleManager.GetState(activity);
		}

		public IObservable<bool> GetActivityAvtiveStateObsrevable(Activity activity)
		{
			return _lifecycleManager
				.GetActivityAvtiveStateObsrevable(activity);
		}

		protected abstract void ContainerSetting(ContainerBuilder builder);

		public override void OnCreate()
		{
			base.OnCreate();

			RegisterActivityLifecycleCallbacks(_lifecycleManager);

			var containerBuilder = new ContainerBuilder();

			ContainerSetting(containerBuilder);

			ApplicationComponentContext = containerBuilder.Build();
		}
	}
}