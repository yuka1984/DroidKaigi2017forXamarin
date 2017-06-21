using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.ViewModels;
using Nyanto;
using Reactive.Bindings.Extensions;

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SessionDetailFragment : FragmentBase<SessionDetailViewModel>
	{
		public override int ViewResourceId => Resource.Layout.fragment_session_detail;
		protected override void Bind(View view)
		{
			var accessor = new fragment_session_detail_holder(view);

			ViewModel.Title.Subscribe(x => accessor.collapsing_toolbar.Title = x).AddTo(CompositeDisposable);

		}
	}
}