#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.App;
using Android.OS;
using Android.Runtime;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Object = Java.Lang.Object;

#endregion

namespace Nyanto.Core
{
	public static class ReadOnlySwitchReactivePropertyExtentions
	{
		public static ReadOnlySwitchReactiveProperty<T> ToReadOnlySwitchReactiveProperty<T>(
			this IObservable<T> observable,
			IObservable<bool> switchSource = null,
			T initialValue = default(T),
			bool initialActive = false,
			IScheduler eventScheduler = null)
		{
			return new ReadOnlySwitchReactiveProperty<T>(observable, switchSource, initialValue, initialActive, eventScheduler);
		}
	}

	public interface ISwitchReactiveProperty
	{
		bool IsActive { get; set; }
	}

	public class ReadOnlySwitchReactiveProperty<T> : IReadOnlyReactiveProperty<T>, ISwitchReactiveProperty, IDisposable
	{
		private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
		private readonly IScheduler _eventScheduler;
		private readonly IObservable<T> _onScheduleObservable;

		private readonly Subject<T> _subject = new Subject<T>();

		private bool _isActive;

		public ReadOnlySwitchReactiveProperty(
			IObservable<T> source,
			IObservable<bool> switchSource,
			T initialValue = default(T),
			bool initialActive = false,
			IScheduler eventScheduler = null)
		{
			_isActive = initialActive;
			Value = initialValue;
			_eventScheduler = eventScheduler ?? ReactivePropertyScheduler.Default;
			_onScheduleObservable = _subject.ObserveOn(_eventScheduler);

			source
				.Do(x => { Value = x; })
				.Where(x => IsActive)
				.Subscribe(
					x => { _subject.OnNext(x); },
					e => { _subject.OnError(e); },
					() => { _subject.OnCompleted(); })
				.AddTo(_compositeDisposable)
				;
			switchSource.Subscribe(x => IsActive = x).AddTo(_compositeDisposable);
			_subject.AddTo(_compositeDisposable);
		}

		public bool IsActive
		{
			get => _isActive;
			set
			{
				if (_compositeDisposable.IsDisposed == false && _isActive == false && value)
					_subject.OnNext(Value);
				_isActive = value;
			}
		}

		object IReadOnlyReactiveProperty.Value => Value;

		public T Value { get; private set; }

		public IDisposable Subscribe(IObserver<T> observer)
		{
			var disposable = _onScheduleObservable.Subscribe(observer);
			if (IsActive)
				Observable.Range(0, 1)
					.ObserveOn(_eventScheduler)
					.Subscribe(x => observer.OnNext(Value));
			return disposable;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public new void Dispose()
		{
			if (!_compositeDisposable.IsDisposed)
				_compositeDisposable.Dispose();
		}
	}
}