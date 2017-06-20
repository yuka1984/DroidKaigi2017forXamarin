#region

using System;
using System.Reactive.Disposables;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Autofac;
using Nyanto.Core;
using Reactive.Bindings.Extensions;

#endregion

namespace Nyanto
{
	public abstract class FragmentBase<T> : FragmentBase where T : ViewModelBase
	{
		public T ViewModel { get; set; }

		public override void OnAttach(Context context)
		{
			base.OnAttach(context);


			if (ViewModel != null)
			{
				var isActive = false;
				if (Activity.Application is ApplicationBase)
				{
					var appbase = (Activity.Application as ApplicationBase);
					var state = appbase.GetActivityState(Activity);

					isActive = state == ActivityLifeCycycle.Resumed || state == ActivityLifeCycycle.Started ||
					           state == ActivityLifeCycycle.Paused;
					(ViewModel as IObserver<bool>).OnNext(isActive);
					var dispose = appbase.GetActivityAvtiveStateObsrevable(Activity).Subscribe(ViewModel).AddTo(CompositeDisposable);
					ViewModel.Init();
				}
			}
		}
	}

	public abstract class FragmentBase : Fragment
	{
		private readonly ILifetimeScope _componentContext = null;
		protected CompositeDisposable CompositeDisposable;

		public abstract int ViewResourceId { get; }

		public ApplicationBase NyantoApplication
		{
			get
			{
				var activity = Activity as AppCompatActivityBase;
				if (activity != null)
					return activity.NyantoApplication;

				throw new ArgumentException("The activity of this fragment is not an instance of AppCompatActivityBase");
			}
		}

		protected ILifetimeScope GetParentComponent()
		{
			if (_componentContext != null)
				return _componentContext;

			var activity = Activity as AppCompatActivityBase;
			if (activity != null)
				return activity.GetComponentContext();

			throw new ArgumentException("The activity of this fragment is not an instance of BaseActivity");
		}

		protected abstract void Bind(View view);

		public override void OnAttach(Context context)
		{
			base.OnAttach(context);
			GetParentComponent().InjectProperties(this);
			CompositeDisposable = new CompositeDisposable();
		}

		public override void OnDetach()
		{
			base.OnDetach();
			CompositeDisposable.Dispose();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view = inflater.Inflate(ViewResourceId, container, false);
			Bind(view);
			return view;
		}
	}
}