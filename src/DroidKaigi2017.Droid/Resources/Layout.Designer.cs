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

namespace DroidKaigi2017.Droid
{
	public class activity_main_holder : IDisposable
	{
		public Android.Support.V7.Widget.Toolbar toolbar { get; }
		public ImageView logo { get; }
		public TextView title { get; }
		public FrameLayout content_view { get; }
		public Android.Support.Design.Widget.BottomNavigationView bottom_nav { get; }

		public activity_main_holder(View view)
		{
			toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(DroidKaigi2017.Droid.Resource.Id.toolbar);
			logo = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.logo);
			title = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.title);
			content_view = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.content_view);
			bottom_nav = view.FindViewById<Android.Support.Design.Widget.BottomNavigationView>(DroidKaigi2017.Droid.Resource.Id.bottom_nav);
		}
		public void Dispose()
		{
			toolbar.Dispose();
			logo.Dispose();
			title.Dispose();
			content_view.Dispose();
			bottom_nav.Dispose();
		}
	}

	public class activity_session_detail_holder : IDisposable
	{
		public FrameLayout content_view { get; }

		public activity_session_detail_holder(View view)
		{
			content_view = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.content_view);
		}
		public void Dispose()
		{
			content_view.Dispose();
		}
	}

	public class fragment_sessions_holder : IDisposable
	{
		public HorizontalScrollView root { get; }
		public LinearLayout header_row { get; }
		public View border { get; }
		public TwoWayView.Layout.TouchlessTwoWayView recycler_view { get; }
		public TextView txt_date { get; }
		public FrameLayout loading { get; }

		public fragment_sessions_holder(View view)
		{
			root = view.FindViewById<HorizontalScrollView>(DroidKaigi2017.Droid.Resource.Id.root);
			header_row = view.FindViewById<LinearLayout>(DroidKaigi2017.Droid.Resource.Id.header_row);
			border = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.border);
			recycler_view = view.FindViewById<TwoWayView.Layout.TouchlessTwoWayView>(DroidKaigi2017.Droid.Resource.Id.recycler_view);
			txt_date = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_date);
			loading = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.loading);
		}
		public void Dispose()
		{
			root.Dispose();
			header_row.Dispose();
			border.Dispose();
			recycler_view.Dispose();
			txt_date.Dispose();
			loading.Dispose();
		}
	}

	public class fragment_session_detail_holder : IDisposable
	{
		public Android.Support.Design.Widget.AppBarLayout app_bar { get; }
		public Android.Support.Design.Widget.CollapsingToolbarLayout collapsing_toolbar { get; }
		public ImageView img_cover { get; }
		public View fade_cover { get; }
		public Android.Support.V7.Widget.Toolbar toolbar { get; }
		public Android.Support.V4.Widget.NestedScrollView nested_scroll { get; }
		public Com.Google.Android.Flexbox.FlexboxLayout tag_container { get; }
		public TextView txt_place { get; }
		public TextView txt_language { get; }
		public TextView txt_topic { get; }
		public ImageView img_speaker { get; }
		public TextView txt_speaker_name { get; }
		public ImageView icon_slide { get; }
		public ImageView icon_movie { get; }
		public TextView txt_description { get; }
		public Button txt_feedback { get; }
		public Android.Support.Design.Widget.FloatingActionButton fab { get; }

		public fragment_session_detail_holder(View view)
		{
			app_bar = view.FindViewById<Android.Support.Design.Widget.AppBarLayout>(DroidKaigi2017.Droid.Resource.Id.app_bar);
			collapsing_toolbar = view.FindViewById<Android.Support.Design.Widget.CollapsingToolbarLayout>(DroidKaigi2017.Droid.Resource.Id.collapsing_toolbar);
			img_cover = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.img_cover);
			fade_cover = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.fade_cover);
			toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(DroidKaigi2017.Droid.Resource.Id.toolbar);
			nested_scroll = view.FindViewById<Android.Support.V4.Widget.NestedScrollView>(DroidKaigi2017.Droid.Resource.Id.nested_scroll);
			tag_container = view.FindViewById<Com.Google.Android.Flexbox.FlexboxLayout>(DroidKaigi2017.Droid.Resource.Id.tag_container);
			txt_place = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_place);
			txt_language = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_language);
			txt_topic = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_topic);
			img_speaker = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.img_speaker);
			txt_speaker_name = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_speaker_name);
			icon_slide = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.icon_slide);
			icon_movie = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.icon_movie);
			txt_description = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_description);
			txt_feedback = view.FindViewById<Button>(DroidKaigi2017.Droid.Resource.Id.txt_feedback);
			fab = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(DroidKaigi2017.Droid.Resource.Id.fab);
		}
		public void Dispose()
		{
			app_bar.Dispose();
			collapsing_toolbar.Dispose();
			img_cover.Dispose();
			fade_cover.Dispose();
			toolbar.Dispose();
			nested_scroll.Dispose();
			tag_container.Dispose();
			txt_place.Dispose();
			txt_language.Dispose();
			txt_topic.Dispose();
			img_speaker.Dispose();
			txt_speaker_name.Dispose();
			icon_slide.Dispose();
			icon_movie.Dispose();
			txt_description.Dispose();
			txt_feedback.Dispose();
			fab.Dispose();
		}
	}

	public class fragment_settings_holder : IDisposable
	{
		public LinearLayout root_view { get; }
		public RelativeLayout language_settings_container { get; }
		public TextView txt_title { get; }
		public TextView setting_description { get; }
		public TextView txt_language { get; }
		public View heads_up_border_top { get; }
		public DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView local_time_switch_row { get; }
		public View heads_up_border_bottom { get; }
		public DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView notification_switch_row { get; }
		public DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView heads_up_switch_row { get; }
		public View heads_up_border { get; }
		public TextView developer_menu_title { get; }
		public TextView developer_menu_tips { get; }
		public DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView debug_overlay_view_switch_row { get; }
		public Switch setting_switch { get; }

		public fragment_settings_holder(View view)
		{
			root_view = view.FindViewById<LinearLayout>(DroidKaigi2017.Droid.Resource.Id.root_view);
			language_settings_container = view.FindViewById<RelativeLayout>(DroidKaigi2017.Droid.Resource.Id.language_settings_container);
			txt_title = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_title);
			setting_description = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.setting_description);
			txt_language = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_language);
			heads_up_border_top = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.heads_up_border_top);
			local_time_switch_row = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView>(DroidKaigi2017.Droid.Resource.Id.local_time_switch_row);
			heads_up_border_bottom = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.heads_up_border_bottom);
			notification_switch_row = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView>(DroidKaigi2017.Droid.Resource.Id.notification_switch_row);
			heads_up_switch_row = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView>(DroidKaigi2017.Droid.Resource.Id.heads_up_switch_row);
			heads_up_border = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.heads_up_border);
			developer_menu_title = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.developer_menu_title);
			developer_menu_tips = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.developer_menu_tips);
			debug_overlay_view_switch_row = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.SettingSwitchRowView>(DroidKaigi2017.Droid.Resource.Id.debug_overlay_view_switch_row);
			setting_switch = view.FindViewById<Switch>(DroidKaigi2017.Droid.Resource.Id.setting_switch);
		}
		public void Dispose()
		{
			root_view.Dispose();
			language_settings_container.Dispose();
			txt_title.Dispose();
			setting_description.Dispose();
			txt_language.Dispose();
			heads_up_border_top.Dispose();
			local_time_switch_row.Dispose();
			heads_up_border_bottom.Dispose();
			notification_switch_row.Dispose();
			heads_up_switch_row.Dispose();
			heads_up_border.Dispose();
			developer_menu_title.Dispose();
			developer_menu_tips.Dispose();
			debug_overlay_view_switch_row.Dispose();
			setting_switch.Dispose();
		}
	}

	public class view_sessions_header_cell_holder : IDisposable
	{
		public TextView txt_room_name { get; }

		public view_sessions_header_cell_holder(View view)
		{
			txt_room_name = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_room_name);
		}
		public void Dispose()
		{
			txt_room_name.Dispose();
		}
	}

	public class view_session_cell_holder : IDisposable
	{
		public Android.Support.Constraints.ConstraintLayout root { get; }
		public View categoryBorder { get; }
		public TextView txt_time { get; }
		public TextView txt_minutes { get; }
		public ImageView img_check { get; }
		public TextView txt_title { get; }
		public TextView txt_language { get; }
		public TextView txt_speaker_name { get; }

		public view_session_cell_holder(View view)
		{
			root = view.FindViewById<Android.Support.Constraints.ConstraintLayout>(DroidKaigi2017.Droid.Resource.Id.root);
			categoryBorder = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.categoryBorder);
			txt_time = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_time);
			txt_minutes = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_minutes);
			img_check = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.img_check);
			txt_title = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_title);
			txt_language = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_language);
			txt_speaker_name = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_speaker_name);
		}
		public void Dispose()
		{
			root.Dispose();
			categoryBorder.Dispose();
			txt_time.Dispose();
			txt_minutes.Dispose();
			img_check.Dispose();
			txt_title.Dispose();
			txt_language.Dispose();
			txt_speaker_name.Dispose();
		}
	}

	public class view_setting_switch_row_holder : IDisposable
	{
		public TextView setting_title { get; }
		public TextView setting_description { get; }
		public Android.Support.V7.Widget.SwitchCompat setting_switch { get; }

		public view_setting_switch_row_holder(View view)
		{
			setting_title = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.setting_title);
			setting_description = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.setting_description);
			setting_switch = view.FindViewById<Android.Support.V7.Widget.SwitchCompat>(DroidKaigi2017.Droid.Resource.Id.setting_switch);
		}
		public void Dispose()
		{
			setting_title.Dispose();
			setting_description.Dispose();
			setting_switch.Dispose();
		}
	}

}
