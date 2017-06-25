#region

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;

#endregion

namespace DroidKaigi2017.Interface.Repository
{
	public interface IMySessionRepository
	{
		ReadOnlyObservableCollection<MySessionModel> MySessions { get; }

		Task LoadAsync();

		Task Add(int sessionId);

		Task Remove(int sessionId);
	}
}