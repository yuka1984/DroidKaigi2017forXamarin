#region

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Reactive.Bindings.Extensions;

#endregion

namespace Nyanto.Binding
{
	public static class BindingHelper
	{
		public static IDisposable OneWayBind<TType, TProperty>(this TType type, Expression<Func<TType, TProperty>> selector,
			IObservable<TProperty> observable)
		{
			return observable.ObserveOnUIDispatcher()
				.Subscribe(x => { AccessorCache<TType>.LookupSet(selector).Invoke(type, x); });
		}

		public static IDisposable OneWayBind<TType, TProperty>(this TType type,
			Expression<Func<TType, Action<TProperty>>> selector, IObservable<TProperty> observable)
		{
			return observable.ObserveOnUIDispatcher()
				.Subscribe(x => { AccessorCache<TType>.LookupMethod(selector).Invoke(type).Invoke(x); });
		}

		private static class AccessorCache<TType>
		{
			private static readonly Dictionary<string, Delegate> getCache = new Dictionary<string, Delegate>();
			private static readonly Dictionary<string, Delegate> setCache = new Dictionary<string, Delegate>();
			private static readonly Dictionary<string, Delegate> methodCache = new Dictionary<string, Delegate>();

			public static Func<TType, Action<TProperty>> LookupMethod<TProperty>(
				Expression<Func<TType, Action<TProperty>>> methodSelector)
			{
				var methodName = GetMethodName(methodSelector);
				Delegate accessor = null;
				lock (methodCache)
				{
					if (!methodCache.TryGetValue(methodName, out accessor))
					{
						accessor = methodSelector.Compile();
						methodCache.Add(methodName, accessor);
					}
				}

				return (Func<TType, Action<TProperty>>)accessor;
			}

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

				return (Func<TType, TProperty>)accessor;
			}

			private static string GetMethodName<TProperty>(Expression<Func<TType, Action<TProperty>>> methodSelector)
			{
				var methodCallExpression = methodSelector.Body as MethodCallExpression;
				if (methodCallExpression == null)
				{
					var unaryExpression = methodSelector.Body as UnaryExpression;
					if (unaryExpression == null) throw new ArgumentException(nameof(methodSelector));
					methodCallExpression = unaryExpression.Operand as MethodCallExpression;
					if (methodCallExpression == null) throw new ArgumentException(nameof(methodSelector));
				}

				var methodInfo = (methodCallExpression.Object as ConstantExpression)?.Value as MethodInfo;
				if (methodInfo == null)
					throw new ArgumentException(nameof(methodSelector));

				return methodInfo.Name;
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

				return (Action<TType, TProperty>)accessor;
			}

			private static Delegate CreateSetAccessor<TProperty>(Expression<Func<TType, TProperty>> propertySelector)
			{
				var propertyInfo = (PropertyInfo)((MemberExpression)propertySelector.Body).Member;
				var selfParameter = Expression.Parameter(typeof(TType), "self");
				var valueParameter = Expression.Parameter(typeof(TProperty), "value");
				var body = Expression.Assign(Expression.Property(selfParameter, propertyInfo), valueParameter);
				var lambda = Expression.Lambda<Action<TType, TProperty>>(body, selfParameter, valueParameter);
				return lambda.Compile();
			}
		}
	}
}