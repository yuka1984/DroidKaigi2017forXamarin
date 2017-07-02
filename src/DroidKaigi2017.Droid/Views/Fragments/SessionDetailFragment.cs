	#region

using System;
using System.Reactive.Linq;
using Android.App;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.CustomViews;
using FFImageLoading;
using FFImageLoading.Work;
using Nyanto;
using Nyanto.Binding;
using Reactive.Bindings.Extensions;
using TwoWayView.Core;
using ActionBar = Android.App.ActionBar;
using Uri = Android.Net.Uri;

#endregion

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SessionDetailFragment : FragmentBase<SessionDetailViewModel>
	{
		public IDateUtil DateUtil { get; set; }
		public override int ViewResourceId => Resource.Layout.fragment_session_detail;

		protected override void Bind(View view)
		{
			var accessor = new fragment_session_detail_holder(view);

			accessor.collapsing_toolbar.OneWayBind(x => x.Title, ViewModel.Title).AddTo(CompositeDisposable);
			Activity.OneWayBind(x => x.SetTheme, ViewModel.sessionThemeResId).AddTo(CompositeDisposable);

			ViewModel.Title.Zip(ViewModel.sessionVividColorResId, (title, vivid) => new {title, vivid})
				.ObserveOnUIDispatcher()
				.Subscribe(x =>
				{
					var taskDescription =
						new ActivityManager.TaskDescription(x.title, null,
							new Color(ContextCompat.GetColor(Activity, x.vivid)));
					Activity.SetTaskDescription(taskDescription);
				})
				.AddTo(CompositeDisposable)
				;

			accessor.fade_cover.OneWayBind(x => x.SetBackgroundResource, ViewModel.sessionPaleColorResId)
				.AddTo(CompositeDisposable);

			// TimeRange	
			var rangeText = ViewModel.StartTime.CombineLatest(ViewModel.EndTime, (s, e) => new { s, e })
				.Select(x => Context.GetString(Resource.String.session_time_range
					, DateUtil.GetLongFormatDate(x.s), DateUtil.GetHourMinute(x.e), Utils.DateUtil.GetMinutes(x.s, x.e)));
			accessor.txt_timerange.OneWayBind(x => x.Text, rangeText).AddTo(CompositeDisposable);

			// tag
			accessor.tag_container.OneWayBind(x => x.Visibility, ViewModel.tagContainerVisibility.Select(x => x.ToViewStates()))
				.AddTo(CompositeDisposable);

			// room
			accessor.txt_place.OneWayBind(x => x.Text, ViewModel.RoomName).AddTo(CompositeDisposable);
			accessor.txt_place.OneWayBind(x => x.Visibility,
					ViewModel.RoomName.Select(x => (!string.IsNullOrEmpty(x)).ToViewStates()))
				.AddTo(CompositeDisposable);

			// language
			accessor.txt_language.OneWayBind(x => x.Text, ViewModel.languageResId).AddTo(CompositeDisposable);

			// topic
			accessor.txt_topic.OneWayBind(x => x.Text, ViewModel.Topic).AddTo(CompositeDisposable);

			// speaker
			accessor.speaker.OneWayBind(x => x.Visibility, ViewModel.speakerVisibility.Select(x => x.ToViewStates()))
				.AddTo(CompositeDisposable);
			ViewModel.speakerImageUrl.ObserveOnUIDispatcher()
				.Subscribe(x =>
				{
					if (string.IsNullOrEmpty(x))
					{
						accessor.img_speaker.SetImageDrawable(ContextCompat.GetDrawable(Context, Resource.Drawable.ic_speaker_placeholder));	
					}
					else
					{
						var size = (int)Math.Round(Resources.GetDimension(Resource.Dimension.icon_48dp));
						ImageService.Instance.LoadUrl(x, TimeSpan.FromDays(1))
							.Retry(3, 200)
							.DownSample(size, size)
							.LoadingPlaceholder(nameof(Resource.Drawable.ic_speaker_placeholder), ImageSource.CompiledResource)
							.ErrorPlaceholder(nameof(Resource.Drawable.ic_speaker_placeholder), ImageSource.CompiledResource)
							.Into(accessor.img_speaker);
					}						
				})
				.AddTo(CompositeDisposable);
			accessor.txt_speaker_name.OneWayBind(x => x.Text, ViewModel.SpeakerName).AddTo(CompositeDisposable);			


			// movie
			accessor.icon_movie.OneWayBind(x => x.Visibility
					, ViewModel.IsMovieVisibility.Select(x => x.ToViewStates()))
				.AddTo(CompositeDisposable);
			accessor.icon_movie.Click += (sender, args) => { ViewModel.MovieCommand.CheckExecute(sender); };

			// slide
			accessor.icon_slide.OneWayBind(x => x.Visibility, ViewModel.IsSlideEnabled.Select(x => x.ToViewStates()))
				.AddTo(CompositeDisposable);
			accessor.icon_slide.Click += (sender, args) => { ViewModel.SlideCommand.CheckExecute(sender); };

			// description
			accessor.txt_description.OneWayBind(x => x.Text, ViewModel.Description).AddTo(CompositeDisposable);


			// feedback
			accessor.txt_feedback.OneWayBind(x => x.Visibility, ViewModel.feedbackButtonVisiblity.Select(x => x.ToViewStates()))
				.AddTo(CompositeDisposable);
			accessor.txt_feedback.Click += (s, a) => { ViewModel.FeedBackCommand.CheckExecute(s); };

			// fab
			ViewModel.sessionVividColorResId.ObserveOnUIDispatcher()
				.Select(x => Resources.GetColorStateList(x))
				.Subscribe(
					x =>
					{
						accessor.fab.BackgroundTintList = x;
					})
				.AddTo(CompositeDisposable);			

			accessor.fab.OneWayBind(x => x.SetImageResource,
					ViewModel.isMySession.Select(y => y
						? Resource.Drawable.avd_check_to_add_24dp
						: Resource.Drawable.avd_add_to_check_24dp))
				.AddTo(CompositeDisposable);

			accessor.fab.SetOnClickListener(new AnonymousOnClickListner
			{
				OnClickAction = view1 => { ViewModel.FavCommand.CheckExecute(view); }
			});

			// scroll
			accessor.root.SetOverScrollListener(new OverScrollLayout.AnonymousOnOverScrillListner(){ OnOverScrollAction = () =>
			{
				ViewModel.CloseCommand.CheckExecute(this);
			}} );

			accessor.nested_scroll.SetOnScrollChangeListener(new AnonymousOnScrollChangeListener
			{
				OnScrollChangeAction = (v, x, y, ox, oy) =>
				{
					if (y > oy)
					{
						accessor.fab.Hide();
					}
					if (y < oy)
					{
						accessor.fab.Show();
					}
				}
			});
			

			var activity = ((AppCompatActivity)Activity);
			activity.SetSupportActionBar(accessor.toolbar);
			var bar = activity.SupportActionBar;
			if (bar != null)
			{
				bar.SetDisplayHomeAsUpEnabled(true);
				bar.SetDisplayShowHomeEnabled(true);
				bar.SetDisplayShowTitleEnabled(false);
				bar.SetHomeButtonEnabled(true);
			}
		}

		
	}

	public class AnonymousOnScrollChangeListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
	{
		public Action<NestedScrollView, int, int, int, int> OnScrollChangeAction { get; set; }
		public void OnScrollChange(NestedScrollView v, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
		{
			OnScrollChangeAction?.Invoke(v, scrollX, scrollY, oldScrollX, oldScrollY);
		}
	}
}