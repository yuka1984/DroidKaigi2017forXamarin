using System.Reactive.Linq;
using Android.Views;
using DroidKaigi2017.Droid.ViewModels;
using Nyanto;
using Nyanto.Binding;
using Reactive.Bindings.Extensions;

namespace DroidKaigi2017.Droid.Views.Fragments
{
	public class SessionFeedbackFragment : FragmentBase<SessionFeedbackViewModel>
	{
		public override int ViewResourceId => Resource.Layout.fragment_session_feedback;
		protected override void Bind(View view)
		{
			var holder = new fragment_session_feedback_holder(view);

			holder.loading.OneWayBind(x => x.Visibility, ViewModel.BusyNotifier.Select(x => x.ToViewStates()))
				.AddTo(CompositeDisposable);

			// relevancy
			holder.relevancy.CurrentRankingChanged += (sender, args) =>
			{
				ViewModel.Relevancy.Value = holder.relevancy.GetCurrentRanking();
			};
			holder.relevancy.OneWayBind(x => x.SetCurrentRanking, ViewModel.Relevancy).AddTo(CompositeDisposable);


			
			// AsExpected
			holder.asexpected.CurrentRankingChanged += (sender, args) =>
			{
				ViewModel.AsExpected.Value = holder.asexpected.GetCurrentRanking();
			};
			holder.asexpected.OneWayBind(x => x.SetCurrentRanking, ViewModel.AsExpected).AddTo(CompositeDisposable);

			// Knowledgeable
			holder.knowledgeable.CurrentRankingChanged += (sender, args) =>
			{
				ViewModel.Knowledgeable.Value = holder.knowledgeable.GetCurrentRanking();
			};
			holder.knowledgeable.OneWayBind(x => x.SetCurrentRanking, ViewModel.Knowledgeable).AddTo(CompositeDisposable);

			// Difficulty
			holder.difficulty.CurrentRankingChanged += (sender, args) =>
			{
				ViewModel.Difficulty.Value = holder.difficulty.GetCurrentRanking();
			};
			holder.difficulty.OneWayBind(x => x.SetCurrentRanking, ViewModel.Difficulty).AddTo(CompositeDisposable);

			// Comment
			holder.comment.FocusChange += (sender, args) =>
			{
				if (!args.HasFocus)
				{
					ViewModel.Comment.Value = holder.comment.Text;
				}
			};
			holder.comment.OneWayBind(x => x.Text, ViewModel.Comment).AddTo(CompositeDisposable);

			// Submit
			holder.submit_feedback.Click += (sender, args) =>
			{
				ViewModel.SubmitFeedbackCommand.CheckExecute(this);
			};

			holder.submit_feedback.Enabled = ViewModel.SubmitFeedbackCommand.CanExecute();
			holder.submit_feedback.OneWayBind(x => x.Enabled,
					ViewModel.SubmitFeedbackCommand
						.CanExecuteChangedAsObservable()
						.Select(x => ViewModel.SubmitFeedbackCommand.CanExecute()))
				.AddTo(CompositeDisposable);


		}
	}
}