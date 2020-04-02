using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Notifications
{
	public class DefaultEventPublisher : IEventPublisher
	{
		private readonly IServiceProvider _services;
		
		public DefaultEventPublisher(IServiceProvider services)
		{
			_services = services;
		}

		public virtual Task Publish<T>(T @event, [CallerMemberName]string eventName = default, CancellationToken cancellationToken = default) where T : class, IEvent
		{
			var handlerTasks = GetEventHandlers<T>().Select(handler => handler.Handle(@event, eventName, cancellationToken));
            return Task.Factory.StartNew(() => Task.WhenAll(handlerTasks), cancellationToken, TaskCreationOptions.None, TaskScheduler.Default).Unwrap();
		}

		protected virtual IEnumerable<IEventHandler<T>> GetEventHandlers<T>() where T : class, IEvent
		{
			var handlers = _services.GetService(typeof(IEnumerable<IEventHandler<T>>));

			if (handlers != null)
			{
				return (IEnumerable<IEventHandler<T>>)handlers;
			}

			var handler = _services.GetService(typeof(IEventHandler<T>));

			if (handler != null)
			{
				return (IEnumerable<IEventHandler<T>>)new[] { handler };
			}
			
			return Enumerable.Empty<IEventHandler<T>>();
		}
	}
}
