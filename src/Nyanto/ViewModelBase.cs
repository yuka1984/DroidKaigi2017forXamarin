#region

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reflection;
using System.Runtime.CompilerServices;
using Android.App;
using Reactive.Bindings;

#endregion

namespace Nyanto
{
	public abstract class ViewModelBase : IDisposable, INotifyPropertyChanged, IObserver<bool>, IObservable<bool>
	{
		public readonly CompositeDisposable CompositeDisposable = new CompositeDisposable();

		protected ReadOnlyReactiveProperty<bool> IsActiveObservable;
		private readonly ReactiveProperty<bool> _isActive = new ReactiveProperty<bool>(TaskPoolScheduler.Default, false);

		public bool Initialized { get; internal set; }

		internal void Init()
		{
			Initialized = true;
		}

		protected ViewModelBase()
		{
			IsActiveObservable = _isActive.ToReadOnlyReactiveProperty(eventScheduler: TaskPoolScheduler.Default);
		}

		public void Dispose()
		{
			CompositeDisposable.Dispose();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value)) return false;

			storage = value;
			OnPropertyChanged(propertyName);

			return true;
		}

		protected bool SetProperty<TType, TProperty>(TType targetClass, Expression<Func<TType, TProperty>> selector,
			TProperty value, [CallerMemberName] string propertyName = null)
		{
			var oldValue = AccessorCache<TType>.LookupGet(selector).Invoke(targetClass);

			if (Equals(oldValue, value)) return false;

			AccessorCache<TType>.LookupSet(selector).Invoke(targetClass, value);
			OnPropertyChanged(propertyName);
			return true;
		}

		protected void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		void IObserver<bool>.OnNext(bool value)
		{
			_isActive.Value = value;
		}

		void IObserver<bool>.OnError(Exception error)
		{
			
		}

		void IObserver<bool>.OnCompleted()
		{
			
		}

		public IDisposable Subscribe(IObserver<bool> observer)
		{
			return IsActiveObservable.Subscribe(observer);
		}
	}


	internal static class AccessorCache<TType>
	{
		private static readonly Dictionary<string, Delegate> getCache = new Dictionary<string, Delegate>();
		private static readonly Dictionary<string, Delegate> setCache = new Dictionary<string, Delegate>();

		public static Func<TType, TProperty> LookupGet<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
		{
			var propertyName = GetPropertyName(propertySelector);
			Delegate accessor;

			lock (getCache)
			{
				if (!getCache.TryGetValue(propertyName, out accessor))
				{
					accessor = propertySelector.Compile();
					getCache.Add(propertyName, accessor);
				}
			}

			return (Func<TType, TProperty>) accessor;
		}

		private static string GetPropertyName<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
		{
			var memberExpression = propertySelector.Body as MemberExpression;
			if (memberExpression == null)
			{
				var unaryExpression = propertySelector.Body as UnaryExpression;
				if (unaryExpression == null) throw new ArgumentException(nameof(propertySelector));
				memberExpression = unaryExpression.Operand as MemberExpression;
				if (memberExpression == null) throw new ArgumentException(nameof(propertySelector));
			}

			return memberExpression.Member.Name;
		}

		public static Action<TType, TProperty> LookupSet<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
		{
			var propertyName = GetPropertyName(propertySelector);
			Delegate accessor;

			lock (setCache)
			{
				if (!setCache.TryGetValue(propertyName, out accessor))
				{
					accessor = CreateSetAccessor(propertySelector);
					setCache.Add(propertyName, accessor);
				}
			}

			return (Action<TType, TProperty>) accessor;
		}

		private static Delegate CreateSetAccessor<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
		{
			var propertyInfo = (PropertyInfo) ((MemberExpression) propertySelector.Body).Member;
			var selfParameter = Expression.Parameter(typeof(TType), "self");
			var valueParameter = Expression.Parameter(typeof(TProperty), "value");
			var body = Expression.Assign(Expression.Property(selfParameter, propertyInfo), valueParameter);
			var lambda = Expression.Lambda<Action<TType, TProperty>>(body, selfParameter, valueParameter);
			return lambda.Compile();
		}
	}
}