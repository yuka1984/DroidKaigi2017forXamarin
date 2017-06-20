#region

using System.Collections.ObjectModel;
using System.Threading.Tasks;

#endregion

namespace DroidKaigi2017.Interface.MySession
{
	public interface IMySessionService
	{
		ReadOnlyObservableCollection<MySessionModel> MySessions { get; }

		Task LoadAsync();

		Task Add(int sessionId);

		Task Remove(int sessionId);
	}
}