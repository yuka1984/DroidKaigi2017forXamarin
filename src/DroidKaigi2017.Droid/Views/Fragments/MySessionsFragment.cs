using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.CustomViews;
using FFImageLoading;
using FFImageLoading.Work;
using Nyanto;
using Nyanto.Binding;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class MySessionsFragment : FragmentBase<MySessionsViewModel>
	{
		public IDateUtil DateUtil { get; set; }
		public override int ViewResourceId => Resource.Layout.fragment_my_sessions;
		protected override void Bind(View view)
		{
			var fragmentAccessor = new fragment_my_sessions_holder(view);

			var adapter = ViewModel.MySessions.ToAdapter(
				(group, i) =>
				{
					var v = LayoutInflater.FromContext(group.Context).Inflate(Resource.Layout.view_my_session, group, false);
					return new CompositDiposableViewHolder(v);
				}, (holder, model) =>
				{
					var disposableHolder = holder as CompositDiposableViewHolder;
					var accessor = new view_my_session_holder(holder.ItemView);
					accessor.root.ClickAsObservable()
						.Subscribe(x => model.GoDetailCommand.CheckExecute(null))
						.AddTo(disposableHolder.CompositDisposable);
					accessor.txt_session_title.Text = model.SessionTitle;
					accessor.txt_room.Text = model.RoomName;
					accessor.txt_room.Visibility = (!string.IsNullOrEmpty(model.RoomName)).ToViewStates();

					accessor.txt_session_time_range.Text = holder.ItemView.Context.GetString(Resource.String.session_time_range,
						DateUtil.GetLongFormatDate(model.StartTime), DateUtil.GetHourMinute(model.EndTime),
						Utils.DateUtil.GetMinutes(model.StartTime, model.EndTime));

					if (string.IsNullOrEmpty(model.SpeakerImageUrl))
					{
						accessor.img_speaker.SetImageDrawable(
							ContextCompat.GetDrawable(Context, Resource.Drawable.ic_speaker_placeholder));
					}
					else
					{
						var size = (int) Math.Round(accessor.img_speaker.Resources.GetDimension(Resource.Dimension.icon_36dp));
						ImageService.Instance.LoadUrl(model.SpeakerImageUrl, TimeSpan.FromDays(1))
							.Retry(3, 200)
							.DownSample(size, size)
							.LoadingPlaceholder(nameof(Resource.Drawable.ic_speaker_placeholder), ImageSource.CompiledResource)
							.ErrorPlaceholder(nameof(Resource.Drawable.ic_speaker_placeholder), ImageSource.CompiledResource)
							.Into(accessor.img_speaker);
					}

				});
			var layoutManager = new LinearLayoutManager(Context);

			fragmentAccessor.recycler_view.SetAdapter(adapter);
			fragmentAccessor.recycler_view.HasFixedSize = true;
			fragmentAccessor.recycler_view.AddItemDecoration(new Android.Support.V7.Widget.DividerItemDecoration(Context, layoutManager.Orientation));
			fragmentAccessor.recycler_view.SetLayoutManager(layoutManager);

			fragmentAccessor.empty_label.OneWayBind(x => x.Visibility,
				ViewModel.MySessions.ObserveProperty(y => y.Count).Select(y => (y == 0).ToViewStates()));

			fragmentAccessor.recycler_view.OneWayBind(x => x.Visibility,
				ViewModel.MySessions.ObserveProperty(y => y.Count).Select(y => (y > 0).ToViewStates()));


		}
	}

	public class SimpleViewHolder : RecyclerView.ViewHolder
	{

		public SimpleViewHolder(View view) : base(view)
		{
			
		}
	}


}