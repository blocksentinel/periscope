using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Event
    {
        public static void AddEvents(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBus>(new InMemoryMessageBus());
        }
    }
}
