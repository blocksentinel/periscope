using Cinder.Core.SharedKernel;
using Foundatio.Messaging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                IOptions<Settings> options = sp.GetService<IOptions<Settings>>();
                ILoggerFactory loggerFactory = sp.GetService<ILoggerFactory>();

                return new RabbitMQMessageBus(new RabbitMQMessageBusOptions
                {
                    ConnectionString = options.Value.Bus.ConnectionString,
                    IsSubscriptionQueueExclusive = false,
                    SubscriptionQueueAutoDelete = false,
                    SubscriptionQueueName = options.Value.Bus.QueueName,
                    AcknowledgementStrategy = AcknowledgementStrategy.Automatic,
                    LoggerFactory = loggerFactory
                });
            });
        }
    }
}
