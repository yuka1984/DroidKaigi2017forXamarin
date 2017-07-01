using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DroidKaigi2017.Droid.Utils;
using DroidKaigi2017.Interface.Models;
using DroidKaigi2017.Interface.Repository;
using DroidKaigi2017.Interface.Services;
using DroidKaigi2017.Service;
using DroidKaigi2017.Services;
using Nyanto;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Reactive.Bindings.Notifiers;

namespace DroidKaigi2017.Droid.ViewModels
{
	public class SessionFeedbackViewModel : ViewModelBase
	{
		private readonly ISessionService _sessionService;
		private readonly IFeedBackService _feedbackService;
		private readonly INavigator _navigator;
		private IDisposable _busyDisposable;
		private SessionModel _sessionModel;
		private readonly ReactiveProperty<SessionFeedbackModel> _feedbackModel = new ReactiveProperty<SessionFeedbackModel>();

		public SessionFeedbackViewModel(ISessionService sessionService, IFeedBackService feedbackService, INavigator navigator)
		{
			_sessionService = sessionService;
			_feedbackService = feedbackService;
			_navigator = navigator;

			BusyNotifier = new BusyNotifier();
			LoadCommand = new ReactiveCommand<int>();
			

			Relevancy = new ReactiveProperty<int>();
			AsExpected = new ReactiveProperty<int>();
			Difficulty = new ReactiveProperty<int>();
			Knowledgeable = new ReactiveProperty<int>();
			Comment = new ReactiveProperty<string>();
			SubmitFeedbackCommand = new[]
				{
					_feedbackModel.Select(x => x?.IsSubmitted != true),
					Relevancy.Select(x => x > 0),
					AsExpected.Select(x => x > 0),
					Difficulty.Select(x => x > 0),
					Knowledgeable.Select(x => x > 0),
				}.CombineLatestValuesAreAllTrue()
				.ToReactiveCommand();

			SubmitFeedbackCommand.Subscribe(async x =>
			{
				if (await
					_navigator.DisplayAlert(
						Resource.String.session_feedback_confirm_title,
						Resource.String.session_feedback_confirm_message
						, Android.Resource.String.Ok
						, Android.Resource.String.Cancel))
				{
					using (_busyDisposable = BusyNotifier.ProcessStart())
					{
						await _feedbackService.SubmitAsync(new SessionFeedbackModel
						{
							SessionId = _sessionModel.Id,
							SessionTitle = _sessionModel.Title,
							Relevancy = Relevancy.Value,
							AsExpected = AsExpected.Value,
							Knowledgeable = Knowledgeable.Value,
							Difficulty = Difficulty.Value,
							Comment = Comment.Value,
							IsSubmitted = true,
						});
					}
					_navigator.Finish();
				}
			});	

			LoadCommand.Subscribe(async x =>
			{
				await _feedbackService.LoadAsync();
				if (_sessionService.Sessions.Value.Any(y => y.SessionModel.Id == x))
				{
					_sessionModel = _sessionService.Sessions.Value.First(y => y.SessionModel.Id == x).SessionModel;
					if (_feedbackService.Feedbacks.Any(y => y.SessionId == x))
					{
						var feedback = feedbackService.Feedbacks.First(y => y.SessionId == x);
						Relevancy.Value = feedback.Relevancy;
						AsExpected.Value = feedback.AsExpected;
						Difficulty.Value = feedback.Difficulty;
						Knowledgeable.Value = feedback.Knowledgeable;
						Comment.Value = feedback.Comment;
						_feedbackModel.Value = feedback;
					}
					else
					{
						Relevancy.Value = 0;
						AsExpected.Value = 0;
						Difficulty.Value = 0;
						Knowledgeable.Value = 0;
						Comment.Value = "";
						_feedbackModel.Value = null;
					}
					_busyDisposable.Dispose();
				}				
			});
			_busyDisposable = BusyNotifier.ProcessStart();

			IsActiveObservable.Pairwise().Subscribe(x =>
			{
				if (x.OldItem == true && x.NewItem == false)
				{
					_feedbackService.SaveAsync(new SessionFeedbackModel
					{
						SessionId = _sessionModel.Id,
						SessionTitle = _sessionModel.Title,
						Relevancy = Relevancy.Value,
						AsExpected = AsExpected.Value,
						Knowledgeable = Knowledgeable.Value,
						Difficulty = Difficulty.Value,
						Comment = Comment.Value,
						IsSubmitted = false,
					});
				}				
			}).AddTo(CompositeDisposable);
		}

		

		public ReactiveCommand<int> LoadCommand { get; }

		public ReactiveProperty<int> Relevancy { get;  }

		public ReactiveProperty<int> AsExpected { get;  }

		public ReactiveProperty<int> Difficulty { get; }

		public ReactiveProperty<int> Knowledgeable { get; }

		public ReactiveProperty<string> Comment { get; }

		public ReactiveCommand SubmitFeedbackCommand { get; }

		public BusyNotifier BusyNotifier { get; }
	}
}