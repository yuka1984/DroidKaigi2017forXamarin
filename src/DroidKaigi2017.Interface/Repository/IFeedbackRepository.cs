using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;

namespace DroidKaigi2017.Interface.Repository
{
    public interface IFeedbackRepository
    {
		ReadOnlyObservableCollection<SessionFeedbackModel> Feedbacks { get; }
		Task SubmitAsync(SessionFeedbackModel feedback);

	    Task SaveAsync(SessionFeedbackModel feedback);

	    Task LoadAsync();
    }
}
