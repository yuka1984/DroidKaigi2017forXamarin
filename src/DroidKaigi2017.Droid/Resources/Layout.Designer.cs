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

		public activity_main_holder(Activity view)
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

	public class activity_my_sessions_holder : IDisposable
	{
		public Android.Support.V7.Widget.Toolbar toolbar { get; }
		public FrameLayout content_view { get; }

		public activity_my_sessions_holder(Activity view)
		{
			toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(DroidKaigi2017.Droid.Resource.Id.toolbar);
			content_view = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.content_view);
		}
		public void Dispose()
		{
			toolbar.Dispose();
			content_view.Dispose();
		}
	}

	public class activity_search_holder : IDisposable
	{
		public RelativeLayout root { get; }
		public Android.Support.V7.Widget.Toolbar toolbar { get; }
		public FrameLayout content_view { get; }

		public activity_search_holder(Activity view)
		{
			root = view.FindViewById<RelativeLayout>(DroidKaigi2017.Droid.Resource.Id.root);
			toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(DroidKaigi2017.Droid.Resource.Id.toolbar);
			content_view = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.content_view);
		}
		public void Dispose()
		{
			root.Dispose();
			toolbar.Dispose();
			content_view.Dispose();
		}
	}

	public class activity_session_detail_holder : IDisposable
	{
		public FrameLayout content_view { get; }

		public activity_session_detail_holder(Activity view)
		{
			content_view = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.content_view);
		}
		public void Dispose()
		{
			content_view.Dispose();
		}
	}

	public class activity_session_feedback_holder : IDisposable
	{
		public Android.Support.V7.Widget.Toolbar toolbar { get; }
		public FrameLayout content_view { get; }

		public activity_session_feedback_holder(Activity view)
		{
			toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(DroidKaigi2017.Droid.Resource.Id.toolbar);
			content_view = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.content_view);
		}
		public void Dispose()
		{
			toolbar.Dispose();
			content_view.Dispose();
		}
	}

	public class activity_splash_holder : IDisposable
	{
		

		public activity_splash_holder(Activity view)
		{
			
		}
		public void Dispose()
		{
			
		}
	}

	public class fragment_my_sessions_holder : IDisposable
	{
		public Android.Support.V7.Widget.RecyclerView recycler_view { get; }
		public TextView empty_label { get; }

		public fragment_my_sessions_holder(View view)
		{
			recycler_view = view.FindViewById<Android.Support.V7.Widget.RecyclerView>(DroidKaigi2017.Droid.Resource.Id.recycler_view);
			empty_label = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.empty_label);
		}
		public void Dispose()
		{
			recycler_view.Dispose();
			empty_label.Dispose();
		}
	}

	public class fragment_search_holder : IDisposable
	{
		public RelativeLayout cover { get; }
		public Android.Support.V7.Widget.RecyclerView recycler_view { get; }

		public fragment_search_holder(View view)
		{
			cover = view.FindViewById<RelativeLayout>(DroidKaigi2017.Droid.Resource.Id.cover);
			recycler_view = view.FindViewById<Android.Support.V7.Widget.RecyclerView>(DroidKaigi2017.Droid.Resource.Id.recycler_view);
		}
		public void Dispose()
		{
			cover.Dispose();
			recycler_view.Dispose();
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
		public DroidKaigi2017.Droid.Views.CustomViews.OverScrollLayout root { get; }
		public Android.Support.Design.Widget.AppBarLayout app_bar { get; }
		public Android.Support.Design.Widget.CollapsingToolbarLayout collapsing_toolbar { get; }
		public ImageView img_cover { get; }
		public View fade_cover { get; }
		public Android.Support.V7.Widget.Toolbar toolbar { get; }
		public Android.Support.V4.Widget.NestedScrollView nested_scroll { get; }
		public TextView txt_timerange { get; }
		public Com.Google.Android.Flexbox.FlexboxLayout tag_container { get; }
		public TextView txt_place { get; }
		public TextView txt_language { get; }
		public TextView txt_topic { get; }
		public RelativeLayout speaker { get; }
		public FFImageLoading.Views.ImageViewAsync img_speaker { get; }
		public TextView txt_speaker_name { get; }
		public ImageView icon_slide { get; }
		public ImageView icon_movie { get; }
		public TextView txt_description { get; }
		public Button txt_feedback { get; }
		public Android.Support.Design.Widget.FloatingActionButton fab { get; }

		public fragment_session_detail_holder(View view)
		{
			root = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.OverScrollLayout>(DroidKaigi2017.Droid.Resource.Id.root);
			app_bar = view.FindViewById<Android.Support.Design.Widget.AppBarLayout>(DroidKaigi2017.Droid.Resource.Id.app_bar);
			collapsing_toolbar = view.FindViewById<Android.Support.Design.Widget.CollapsingToolbarLayout>(DroidKaigi2017.Droid.Resource.Id.collapsing_toolbar);
			img_cover = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.img_cover);
			fade_cover = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.fade_cover);
			toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(DroidKaigi2017.Droid.Resource.Id.toolbar);
			nested_scroll = view.FindViewById<Android.Support.V4.Widget.NestedScrollView>(DroidKaigi2017.Droid.Resource.Id.nested_scroll);
			txt_timerange = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_timerange);
			tag_container = view.FindViewById<Com.Google.Android.Flexbox.FlexboxLayout>(DroidKaigi2017.Droid.Resource.Id.tag_container);
			txt_place = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_place);
			txt_language = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_language);
			txt_topic = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_topic);
			speaker = view.FindViewById<RelativeLayout>(DroidKaigi2017.Droid.Resource.Id.speaker);
			img_speaker = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(DroidKaigi2017.Droid.Resource.Id.img_speaker);
			txt_speaker_name = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_speaker_name);
			icon_slide = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.icon_slide);
			icon_movie = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.icon_movie);
			txt_description = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_description);
			txt_feedback = view.FindViewById<Button>(DroidKaigi2017.Droid.Resource.Id.txt_feedback);
			fab = view.FindViewById<Android.Support.Design.Widget.FloatingActionButton>(DroidKaigi2017.Droid.Resource.Id.fab);
		}
		public void Dispose()
		{
			root.Dispose();
			app_bar.Dispose();
			collapsing_toolbar.Dispose();
			img_cover.Dispose();
			fade_cover.Dispose();
			toolbar.Dispose();
			nested_scroll.Dispose();
			txt_timerange.Dispose();
			tag_container.Dispose();
			txt_place.Dispose();
			txt_language.Dispose();
			txt_topic.Dispose();
			speaker.Dispose();
			img_speaker.Dispose();
			txt_speaker_name.Dispose();
			icon_slide.Dispose();
			icon_movie.Dispose();
			txt_description.Dispose();
			txt_feedback.Dispose();
			fab.Dispose();
		}
	}

	public class fragment_session_feedback_holder : IDisposable
	{
		public DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView relevancy { get; }
		public DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView asexpected  { get; }
		public DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView difficulty  { get; }
		public DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView knowledgeable  { get; }
		public EditText comment { get; }
		public Button submit_feedback { get; }
		public FrameLayout loading { get; }

		public fragment_session_feedback_holder(View view)
		{
			relevancy = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView>(DroidKaigi2017.Droid.Resource.Id.relevancy);
			asexpected  = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView>(DroidKaigi2017.Droid.Resource.Id.asexpected );
			difficulty  = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView>(DroidKaigi2017.Droid.Resource.Id.difficulty );
			knowledgeable  = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.FeedbackRankingView>(DroidKaigi2017.Droid.Resource.Id.knowledgeable );
			comment = view.FindViewById<EditText>(DroidKaigi2017.Droid.Resource.Id.comment);
			submit_feedback = view.FindViewById<Button>(DroidKaigi2017.Droid.Resource.Id.submit_feedback);
			loading = view.FindViewById<FrameLayout>(DroidKaigi2017.Droid.Resource.Id.loading);
		}
		public void Dispose()
		{
			relevancy.Dispose();
			asexpected .Dispose();
			difficulty .Dispose();
			knowledgeable .Dispose();
			comment.Dispose();
			submit_feedback.Dispose();
			loading.Dispose();
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

	public class fragment_splash_holder : IDisposable
	{
		public DroidKaigi2017.Droid.Views.CustomViews.ParticlesAnimationView particle_animation_view { get; }

		public fragment_splash_holder(View view)
		{
			particle_animation_view = view.FindViewById<DroidKaigi2017.Droid.Views.CustomViews.ParticlesAnimationView>(DroidKaigi2017.Droid.Resource.Id.particle_animation_view);
		}
		public void Dispose()
		{
			particle_animation_view.Dispose();
		}
	}

	public class view_feedback_ranking_holder : IDisposable
	{
		public LinearLayout ranking_container { get; }
		public TextView txt_label_start { get; }
		public TextView txt_label_end { get; }

		public view_feedback_ranking_holder(View view)
		{
			ranking_container = view.FindViewById<LinearLayout>(DroidKaigi2017.Droid.Resource.Id.ranking_container);
			txt_label_start = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_label_start);
			txt_label_end = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_label_end);
		}
		public void Dispose()
		{
			ranking_container.Dispose();
			txt_label_start.Dispose();
			txt_label_end.Dispose();
		}
	}

	public class view_feedback_ranking_item_holder : IDisposable
	{
		public TextView txt_ranking { get; }

		public view_feedback_ranking_item_holder(View view)
		{
			txt_ranking = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_ranking);
		}
		public void Dispose()
		{
			txt_ranking.Dispose();
		}
	}

	public class view_my_session_holder : IDisposable
	{
		public Android.Support.Constraints.ConstraintLayout root { get; }
		public TextView txt_session_title { get; }
		public TextView txt_room { get; }
		public TextView txt_session_time_range { get; }
		public FFImageLoading.Views.ImageViewAsync img_speaker { get; }

		public view_my_session_holder(View view)
		{
			root = view.FindViewById<Android.Support.Constraints.ConstraintLayout>(DroidKaigi2017.Droid.Resource.Id.root);
			txt_session_title = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_session_title);
			txt_room = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_room);
			txt_session_time_range = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_session_time_range);
			img_speaker = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(DroidKaigi2017.Droid.Resource.Id.img_speaker);
		}
		public void Dispose()
		{
			root.Dispose();
			txt_session_title.Dispose();
			txt_room.Dispose();
			txt_session_time_range.Dispose();
			img_speaker.Dispose();
		}
	}

	public class view_search_holder : IDisposable
	{

		public view_search_holder(View view)
		{
		}
		public void Dispose()
		{
		}
	}

	public class view_search_compat_holder : IDisposable
	{
		public LinearLayout search_bar { get; }
		public TextView search_badge { get; }
		public ImageView search_button { get; }
		public LinearLayout search_edit_frame { get; }
		public ImageView search_mag_icon { get; }
		public LinearLayout search_plate { get; }
		public View search_src_text { get; }
		public ImageView search_close_btn { get; }
		public LinearLayout submit_area { get; }
		public ImageView search_go_btn { get; }
		public ImageView search_voice_btn { get; }

		public view_search_compat_holder(View view)
		{
			search_bar = view.FindViewById<LinearLayout>(DroidKaigi2017.Droid.Resource.Id.search_bar);
			search_badge = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.search_badge);
			search_button = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.search_button);
			search_edit_frame = view.FindViewById<LinearLayout>(DroidKaigi2017.Droid.Resource.Id.search_edit_frame);
			search_mag_icon = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.search_mag_icon);
			search_plate = view.FindViewById<LinearLayout>(DroidKaigi2017.Droid.Resource.Id.search_plate);
			search_src_text = view.FindViewById<View>(DroidKaigi2017.Droid.Resource.Id.search_src_text);
			search_close_btn = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.search_close_btn);
			submit_area = view.FindViewById<LinearLayout>(DroidKaigi2017.Droid.Resource.Id.submit_area);
			search_go_btn = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.search_go_btn);
			search_voice_btn = view.FindViewById<ImageView>(DroidKaigi2017.Droid.Resource.Id.search_voice_btn);
		}
		public void Dispose()
		{
			search_bar.Dispose();
			search_badge.Dispose();
			search_button.Dispose();
			search_edit_frame.Dispose();
			search_mag_icon.Dispose();
			search_plate.Dispose();
			search_src_text.Dispose();
			search_close_btn.Dispose();
			submit_area.Dispose();
			search_go_btn.Dispose();
			search_voice_btn.Dispose();
		}
	}

	public class view_search_result_holder : IDisposable
	{
		public Android.Support.Constraints.ConstraintLayout root { get; }
		public TextView txt_type { get; }
		public FFImageLoading.Views.ImageViewAsync img_speaker { get; }
		public TextView txt_search_result { get; }

		public view_search_result_holder(View view)
		{
			root = view.FindViewById<Android.Support.Constraints.ConstraintLayout>(DroidKaigi2017.Droid.Resource.Id.root);
			txt_type = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_type);
			img_speaker = view.FindViewById<FFImageLoading.Views.ImageViewAsync>(DroidKaigi2017.Droid.Resource.Id.img_speaker);
			txt_search_result = view.FindViewById<TextView>(DroidKaigi2017.Droid.Resource.Id.txt_search_result);
		}
		public void Dispose()
		{
			root.Dispose();
			txt_type.Dispose();
			img_speaker.Dispose();
			txt_search_result.Dispose();
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
