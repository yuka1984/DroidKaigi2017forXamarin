#region

using System.Collections.Generic;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Interface.Repository
{
	public interface ISessionRepository
	{
		ReadOnlyReactiveProperty<List<SessionModel>> SessionsObservable { get; }

		Task LoadAsync();
	}
}