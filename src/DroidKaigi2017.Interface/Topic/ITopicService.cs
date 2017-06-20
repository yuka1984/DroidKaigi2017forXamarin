#region

using System.Collections.Generic;
using System.Threading.Tasks;
using Reactive.Bindings;

#endregion

namespace DroidKaigi2017.Interface.Topic
{
	public class Topic
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public string Other { get; set; }
	}

	public interface ITopicService
	{
		ReadOnlyReactiveProperty<List<Topic>> TopicsObservable { get; }

		Task LoadAsync();
	}
}