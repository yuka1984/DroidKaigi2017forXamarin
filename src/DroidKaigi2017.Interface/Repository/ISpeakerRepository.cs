#region

using System.Collections.Generic;
using System.Threading.Tasks;
using DroidKaigi2017.Interface.Models;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Interface.Repository
{
	public interface ISpeakerRepository
	{
		ReadOnlyReactiveProperty<List<SpeakerModel>> SpealersObservable { get; }

		Task LoadAsync();
	}
}