#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Interface.Session
{
	public interface ISessionService
	{
		ReadOnlyReactiveProperty<List<SessionModel>> SessionsObservable { get; }

		Task LoadAsync();
	}
}