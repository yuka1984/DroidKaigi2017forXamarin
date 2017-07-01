using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace DroidKaigi2017.Interface.Services
{
	public interface IMySessionService
	{
		ReadOnlyObservableCollection<int> MySessions { get; }
		Task AddAsync(int sessionId);
		Task RemoveAsync(int sessionid);
		Task LoadAsync();
	}
}