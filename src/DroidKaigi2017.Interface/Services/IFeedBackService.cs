using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;

namespace DroidKaigi2017.Interface.Services
{
	public interface IFeedBackService
	{
		ReadOnlyObservableCollection<SessionFeedbackModel> Feedbacks { get; }
		IObservable<bool> IsLoading { get; }

		Task LoadAsync();
		Task SaveAsync(SessionFeedbackModel feedback);
		Task SubmitAsync(SessionFeedbackModel feedback);
	}
}