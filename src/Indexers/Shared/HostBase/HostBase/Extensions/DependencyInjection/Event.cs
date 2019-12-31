using Cinder.Indexers.HostBase;
using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Event
    {
        public static void AddEvents(this IServiceCollection services)
        {
            services.AddSingleton<IMessageBus>(sp =>
            {
                IOptions<SettingsBase> options = sp.GetService<IOptions<SettingsBase>>();

                return new RabbitMQMessageBus(new RabbitMQMessageBusOptions
                {
                    ConnectionString = options.Value.Bus.ConnectionString,
                    IsSubscriptionQueueExclusive = false,
                    SubscriptionQueueAutoDelete = false,
                    SubscriptionQueueName = options.Value.Bus.QueueName
                });
            });
        }
    }
}
