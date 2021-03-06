﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.Content.Res;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Droid.ViewModels;
using DroidKaigi2017.Droid.Views.CustomViews;
using Nyanto;
using Reactive.Bindings.Extensions;
using TwoWayView.Core;
using TwoWayView.Layout;
using DividerItemDecoration = TwoWayView.Layout.DividerItemDecoration;
using Object = Java.Lang.Object;

#endregion

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SessionsFragment : FragmentBase<SessionsViewModel>
	{
		public new static readonly string Tag = typeof(SessionsFragment).Name;
		private SessionsAdapter adapter;
		private View border;
		private LinearLayout headerRow;
		private View loading;

		private TouchlessTwoWayView recycleView;
		private HorizontalScrollView root;
		private TextView txtDate;

		public ViewUtil ViewUtil { get; set; }
		public override int ViewResourceId => Resource.Layout.fragment_sessions;

		public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
		{
			inflater.Inflate(Resource.Menu.menu_sessions, menu);
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Resource.Id.item_search:
					ViewModel.GoSearchCommand.CheckExecute(this);
					break;
				case Resource.Id.item_my_sessions:
					ViewModel.GoMySessionsCommand.CheckExecute(this);
					break;
			}
			return base.OnOptionsItemSelected(item);
		}

		public override void OnResume()
		{
			base.OnResume();
		}

		protected override void Bind(View view)
		{
			HasOptionsMenu = true;
			recycleView = view.FindViewById<TouchlessTwoWayView>(Resource.Id.recycler_view);
			root = view.FindViewById<HorizontalScrollView>(Resource.Id.root);
			headerRow = view.FindViewById<LinearLayout>(Resource.Id.header_row);
			border = view.FindViewById(Resource.Id.border);
			txtDate = view.FindViewById<TextView>(Resource.Id.txt_date);
			loading = view.FindViewById(Resource.Id.loading);

			ViewModel.SessionsObservable
				.DistinctUntilChanged()
				.ObserveOnUIDispatcher()
				.Subscribe(RenderSessions)
				.AddTo(CompositeDisposable);
			ViewModel.BusyNotifier
				.ObserveOnUIDispatcher()
				.Subscribe(x => { loading.Visibility = x ? ViewStates.Visible : ViewStates.Gone; })
				.AddTo(CompositeDisposable);
			InitView();
			ViewModel.LoadCommand.CheckExecute(this);
		}

		private void RenderSessions(List<SessionViewModel> sessionViewModels)
		{
			if (sessionViewModels.Count <= 0)
				return;
			;

			if (recycleView.GetLayoutManager() == null)
			{
				var lm = new SpannableGridLayoutManager(Orientation.Vertical, ViewModel.SessionRooms.Length,
					ViewModel.StartTimes.Count);
				recycleView.SetLayoutManager(lm);
			}

			if (headerRow.ChildCount == 0)
				foreach (var room in ViewModel.SessionRooms)
				{
					var view = LayoutInflater.From(Context).Inflate(Resource.Layout.view_sessions_header_cell, null);
					var txtRoomName = view.FindViewById<TextView>(Resource.Id.txt_room_name);
					txtRoomName.Text = room.Name;
					var param = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.MatchParent, 1f);
					txtRoomName.LayoutParameters = param;
					headerRow.AddView(view);
				}

			adapter.Reset(sessionViewModels);

			if (string.IsNullOrEmpty(txtDate.Text))
			{
				txtDate.Text = sessionViewModels.FirstOrDefault()?.FormattedDate;
				txtDate.Visibility = ViewStates.Visible;
			}
		}

		private void InitView()
		{
			((SimpleItemAnimator) recycleView.GetItemAnimator()).SupportsChangeAnimations = false;
			recycleView.HasFixedSize = true;
			var sessionsTableWidth = GetScreenWidth();
			var minWidth = (int) Resources.GetDimension(Resource.Dimension.session_table_min_width);
			if (sessionsTableWidth < minWidth)
				sessionsTableWidth = minWidth;

			recycleView.SetMinimumWidth(sessionsTableWidth);

			var divider = ResourcesCompat.GetDrawable(Resources, Resource.Drawable.divider, null);
			recycleView.AddItemDecoration(new DividerItemDecoration(divider));

			adapter = new SessionsAdapter(Context);
			recycleView.SetAdapter(adapter);

			var clickCanceller = new ClickGestureCanceller(Context, recycleView);

			root.Touch += (sender, args) =>
			{
				clickCanceller.SendCancelIfScrolling(args.Event);
				var e = MotionEvent.Obtain(args.Event);
				e.SetLocation(e.GetX() + root.ScrollX, e.GetY() - headerRow.Height);
				recycleView.ForceToDispatchTouchEvent(e);
				args.Handled = false;
			};

			ViewUtil.AddOneTimeOnGlobalLayoutListener(headerRow, new AnonymousOnGlobalLayoutListener
			{
				OnGlobalLayoutAction = () =>
				{
					if (headerRow.Height > 0)
					{
						recycleView.LayoutParameters.Height = root.Height - border.Height
						                                      - headerRow.Height;
						recycleView.RequestLayout();
						return true;
					}
					return false;
				}
			});

			recycleView.ClearOnScrollListeners();
			recycleView.AddOnScrollListener(new AnonymousOnScrollListener
			{
				OnScrolledAction = (recyclerView, dx, dy, baseAction) =>
				{
					var viewModel = adapter.Get(recycleView.getFirstVisiblePosition());

					if (!string.IsNullOrEmpty(viewModel.FormattedDate))
						txtDate.Text = viewModel.FormattedDate;
				}
			});
		}

		private int GetScreenWidth()
		{
			var display = Activity.WindowManager.DefaultDisplay;
			var size = new Point();
			display.GetSize(size);
			return size.X;
		}

		public class SessionsAdapter : ArrayRecyclerAdapter<SessionViewModel>
		{
			public SessionsAdapter(Context context) : base(context)
			{
			}

			public SessionsAdapter(Context context, List<SessionViewModel> list) : base(context, list)
			{
			}

			public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
			{
				var compositHoldeer = holder as CompositDiposableViewHolder;
				compositHoldeer.CompositDispose();
				var vm = Get(position);
				var param = holder.ItemView.LayoutParameters;
				if (param is SpannableGridLayoutManager.LayoutParams)
				{
					var spanParam = param as SpannableGridLayoutManager.LayoutParams;
					spanParam.ColSpan = vm.ColSpan;
					spanParam.RowSpan = vm.RowSpan;
				}

				var viewAccesor = new view_session_cell_holder(holder.ItemView);


				viewAccesor.root.Clickable = vm.IsSelectable;
				viewAccesor.root.Enabled = vm.IsSelectable;
				viewAccesor.root.SetBackgroundResource(vm.BackgroundResourceId);

				viewAccesor.categoryBorder.Visibility = vm.IsNormalSession.ToViewStates();
				viewAccesor.categoryBorder.SetBackgroundResource(vm.TopicColorResourceId);
				var clicklistner = new AnonymousOnClickListner
				{
					OnClickAction = view => { vm.GoDetailCommand.Execute(); },
					OnLongClickAction = view =>
					{
						vm.CheckCommand?.Execute(!vm.IsCheckVisible.Value);
						return true;
					}
				};
				viewAccesor.root.SetOnClickListener(clicklistner);
				viewAccesor.root.SetOnLongClickListener(clicklistner);

				viewAccesor.img_check.Visibility = vm.IsCheckVisible.Value.ToViewStates();
				var dispose = vm.IsCheckVisible
					.Skip(1)
					.DistinctUntilChanged()
					.ObserveOnUIDispatcher()
					.Subscribe(x =>
					{
						if (position == holder.AdapterPosition)
							NotifyItemChanged(position);
					});
				compositHoldeer.CompositDisposable.Add(dispose);

				viewAccesor.txt_time.Text = vm.ShortStartTime;
				viewAccesor.txt_minutes.Text = vm.Minutes.HasValue
					? Context.GetString(Resource.String.session_minutes, vm.Minutes)
					: "";
				viewAccesor.txt_title.Text = vm.Title;
				viewAccesor.txt_title.SetMaxLines(vm.TitleMaxLines);
				viewAccesor.txt_language.Text = vm.LanguageId;
				viewAccesor.txt_language.Visibility = vm.IsLanguageVisible.ToViewStates();
				viewAccesor.txt_speaker_name.Text = vm.SpeakerName;
				viewAccesor.txt_speaker_name.SetMaxLines(vm.SpeakerNameMaxLines);
				viewAccesor.txt_speaker_name.Visibility = vm.IsNormalSession.ToViewStates();
			}

			public override void OnViewDetachedFromWindow(Object holder)
			{
				base.OnViewDetachedFromWindow(holder);
				var viewHolder = holder as CompositDiposableViewHolder;
				viewHolder.CompositDispose();
			}

			public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
			{
				var view = LayoutInflater.From(Context).Inflate(Resource.Layout.view_session_cell, parent, false);
				return new CompositDiposableViewHolder(view);
			}
		}

		private class ClickGestureCanceller
		{
			private readonly GestureDetector gestureDetector;
			private bool ignoreMotionEventOnScroll;

			public ClickGestureCanceller(Context context, TouchlessTwoWayView targetView)
			{
				gestureDetector = new GestureDetector(context, new AnonymousOnGestureListener
				{
					OnDownAction = e =>
					{
						ignoreMotionEventOnScroll = true;
						return false;
					},
					OnScrollAction = (e1, e2, a1, a2) =>
					{
						if (ignoreMotionEventOnScroll)
						{
							var now = SystemClock.UptimeMillis();
							var cancelEvent = MotionEvent.Obtain(now, now, MotionEventActions.Cancel, 0.0f, 0.0f, 0);
							targetView.ForceToDispatchTouchEvent(cancelEvent);
							ignoreMotionEventOnScroll = false;
						}

						return false;
					}
				});
			}

			public void SendCancelIfScrolling(MotionEvent e)
			{
				gestureDetector.OnTouchEvent(e);
			}
		}
	}

	public class AnonymousOnScrollListener : RecyclerView.OnScrollListener
	{
		public Action<RecyclerView, int, Action<RecyclerView, int>> OnScrollChangedAction { get; set; }

		public Action<RecyclerView, int, int, Action<RecyclerView, int, int>> OnScrolledAction { get; set; }

		public override void OnScrollStateChanged(RecyclerView recyclerView, int newState)
		{
			if (OnScrollChangedAction != null)
				OnScrollChangedAction(recyclerView, newState, base.OnScrollStateChanged);
			else
				base.OnScrollStateChanged(recyclerView, newState);
		}

		public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
		{
			if (OnScrolledAction != null)
				OnScrolledAction(recyclerView, dx, dy, base.OnScrolled);
			else
				base.OnScrolled(recyclerView, dx, dy);
		}
	}
}