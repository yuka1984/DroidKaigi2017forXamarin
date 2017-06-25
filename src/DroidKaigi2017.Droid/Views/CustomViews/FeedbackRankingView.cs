#region

using System;
using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;
using TwoWayView.Core;

#endregion

namespace DroidKaigi2017.Droid.Views.CustomViews
{
	public class FeedbackRankingView : FrameLayout
	{
		private static readonly int DEFAULT_MAX_SIZE = 5;

		private readonly view_feedback_ranking_holder holder;

		private readonly string labelEnd;

		private readonly string labelStart;

		private readonly int maxSize;

		private int currentRanking;

		private IOnCurrentRankingChangeListener listener;

		public event EventHandler CurrentRankingChanged;

		public FeedbackRankingView(Context context) : this(context, null)
		{
			;
		}

		public FeedbackRankingView(Context context, IAttributeSet attrs) : this(context, attrs, 0)
		{
			;
		}

		public FeedbackRankingView(Context context, IAttributeSet attrs, int defStyleAttr) : base(context, attrs,
			defStyleAttr)
		{
			var view = Inflate(context, Resource.Layout.view_feedback_ranking, this);
			if (IsInEditMode)
				return;
			holder = new view_feedback_ranking_holder(view);

			var a = context.ObtainStyledAttributes(attrs, Resource.Styleable.FeedbackRankingView);
			maxSize = a.GetInteger(Resource.Styleable.FeedbackRankingView_rankingMaxSize, DEFAULT_MAX_SIZE);
			labelStart = a.GetString(Resource.Styleable.FeedbackRankingView_rankingLabelStart);
			labelEnd = a.GetString(Resource.Styleable.FeedbackRankingView_rankingLabelEnd);
			a.Recycle();

			initView();
		}

		private void initView()
		{
			if (!string.IsNullOrEmpty(labelStart))
			{
				holder.txt_label_start.Visibility = ViewStates.Visible;
				holder.txt_label_start.Text = labelStart;
			}

			if (!string.IsNullOrEmpty(labelEnd))
			{
				holder.txt_label_end.Visibility = ViewStates.Visible;
				holder.txt_label_end.Text = labelEnd;
			}

			AddRankingViews();
		}

		private void AddRankingViews()
		{
			for (var i = 1; i <= maxSize; i++)
			{
				var number = i;
				var view = LayoutInflater.From(Context)
					.Inflate(Resource.Layout.view_feedback_ranking_item, holder.ranking_container, false);
				var txtRanking = view.FindViewById<TextView>(Resource.Id.txt_ranking);
				txtRanking.Text = number.ToString();
				txtRanking.SetOnClickListener(new AnonymousOnClickListner
				{
					OnClickAction = v =>
					{
						UnselectAll();
						v.Selected = true;
						currentRanking = number;
						if (listener != null)
							listener.onCurrentRankingChange(this, currentRanking);
						CurrentRankingChanged?.Invoke(this, EventArgs.Empty);
					}
				});

				var param = new LinearLayout.LayoutParams(0, ViewGroup.LayoutParams.WrapContent, 1f);
				holder.ranking_container.AddView(view, param);
			}
		}

		private void UnselectAll()
		{
			for (int i = 0, size = holder.ranking_container.ChildCount; i < size; i++)
				holder.ranking_container.GetChildAt(i).FindViewById(Resource.Id.txt_ranking).Selected = false;
		}

		public int GetCurrentRanking()
		{
			return currentRanking;
		}

		public void SetCurrentRanking(int currentRanking)
		{
			if (currentRanking <= 0)
			{
				UnselectAll();
			}
			else if (currentRanking <= holder.ranking_container.ChildCount)
			{
				UnselectAll();
				var view = holder.ranking_container.GetChildAt(currentRanking - 1);
				view.Selected = true;
				this.currentRanking = currentRanking;
			}
		}

		public void SetListener(IOnCurrentRankingChangeListener listener)
		{
			this.listener = listener;
		}


		public static int GetCurrentRanking(FeedbackRankingView view)
		{
			return view.GetCurrentRanking();
		}


		public static void SetCurrentRanking(FeedbackRankingView view, int currentRanking)
		{
			if (currentRanking != view.GetCurrentRanking())
				view.SetCurrentRanking(currentRanking);
		}

		public interface IOnCurrentRankingChangeListener
		{
			void onCurrentRankingChange(FeedbackRankingView view, int currentRanking);
		}
	}
}