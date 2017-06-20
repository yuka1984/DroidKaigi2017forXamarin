#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Interface.Speaker
{
	public interface ISpeakerService
	{
		ReadOnlyReactiveProperty<List<SpeakerModel>> SpealersObservable { get; }

		Task LoadAsync();
	}
}