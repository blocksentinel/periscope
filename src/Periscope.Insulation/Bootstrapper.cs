using System;
using System.Collections.Generic;
using System.Linq;
using Foundatio.Caching;
using Foundatio.Extensions.Hosting.Startup;
using Foundatio.Messaging;
using Foundatio.Queues;
using Foundatio.Serializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Periscope.Core.Clients.CoinGecko;
using Refit;
using StackExchange.Redis;

namespace Periscope.Insulation
{
    public class Bootstrapper
    {
        public static void RegisterServices(IServiceCollection services, AppOptions appOptions, bool runMaintenanceTasks)
        {
            //if (!string.IsNullOrEmpty(appOptions.ExceptionlessApiKey))
            //{
            //    ExceptionlessClient client = ExceptionlessClient.Default;
            //    client.Configuration.ApiKey = appOptions.ExceptionlessApiKey;

            //    client.Configuration.SetDefaultMinLogLevel(LogLevel.Warn);
            //    client.Configuration.UpdateSettingsWhenIdleInterval = TimeSpan.FromMinutes(2);
            //    client.Configuration.SetVersion(appOptions.Version);
            //    client.Configuration.UseInMemoryStorage();
            //    client.Configuration.UseReferenceIds();
            //    //client.Configuration.Settings.Changed += (sender, args) => Console.WriteLine("The key {0} was {1}", args.Item.Key, args.Action);

            //    services.AddSingleton(client);
            //}

            RegisterDatabase(services, appOptions.DatabaseOptions);
            RegisterCache(services, appOptions.CacheOptions);
            RegisterCqrs(services);
            RegisterApiClients(services);
            RegisterMessageBus(services, appOptions.MessageBusOptions);
            //RegisterMetric(services, appOptions.MetricOptions);
            RegisterQueue(services, appOptions.QueueOptions, runMaintenanceTasks);
            //RegisterStorage(services, appOptions.StorageOptions);

            IHealthChecksBuilder healthCheckBuilder = RegisterHealthChecks(services, appOptions);

            //if (appOptions.AppMode != AppMode.Development)
            //{
            //    if (!string.IsNullOrEmpty(appOptions.EmailOptions.SmtpHost))
            //    {
            //        services.ReplaceSingleton<IMailSender, MailKitMailSender>();
            //        healthCheckBuilder.Add(new HealthCheckRegistration("Mail",
            //            s => s.GetRequiredService<IMailSender>() as MailKitMailSender, null,
            //            new[] {"Mail", "MailMessage", "AllJobs"}));
            //    }
            //}
        }

        private static IHealthChecksBuilder RegisterHealthChecks(IServiceCollection services, AppOptions appOptions)
        {
            services.AddStartupActionToWaitForHealthChecks("Critical");

            return services.AddHealthChecks()
                .AddCheckForStartupActions("Critical")
                .AddAutoNamedCheck<ElasticSearchHealthCheck>("Critical")
                .AddAutoNamedCheck<ExternalCoinJob>("ExternalCoin", "AllJobs")
                .AddAutoNamedCheck<ExternalExchangeVolumeJob>("ExternalExchangeVolume", "AllJobs")
                .AddAutoNamedCheck<TradeHubExchangeVolumeJob>("TradeHubExchangeVolume", "AllJobs")
                .AddAutoNamedCheck<TradeHubMarketsJob>("TradeHubMarkets", "AllJobs")
                .AddAutoNamedCheck<TradeHubStatsJob>("TradeHubStats", "AllJobs")
                .AddAutoNamedCheck<ZilliqaCrawlerJob>("ZilliqaCrawler", "AllJobs");
        }

        private static void RegisterDatabase(IServiceCollection container, DatabaseOptions options)
        {
            container.AddDbContext<AppDbContext>(builder => builder.UseNpgsql(options.ConnectionString, o => o.UseNodaTime()));
        }

        private static void RegisterCache(IServiceCollection container, CacheOptions options)
        {
            if (!string.Equals(options.Provider, "redis"))
            {
                return;
            }

            container.ReplaceSingleton(s => GetRedisConnection(options.Data));

            if (!string.IsNullOrEmpty(options.Scope))
            {
                container.ReplaceSingleton<ICacheClient>(s => new ScopedCacheClient(CreateRedisCacheClient(s), options.Scope));
            }
            else
            {
                container.ReplaceSingleton<ICacheClient>(CreateRedisCacheClient);
            }

            container.ReplaceSingleton<IConnectionMapping, RedisConnectionMapping>();
        }

        private static void RegisterCqrs(IServiceCollection container)
        {
            container.AddMediatR(typeof(Core.Bootstrapper));
        }

        private static void RegisterApiClients(IServiceCollection container)
        {
            container.AddRefitClient<ICoinGeckoApi>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri("https://api.coingecko.com/api"));
        }

        private static void RegisterMessageBus(IServiceCollection container, MessageBusOptions options)
        {
            if (string.Equals(options.Provider, "redis"))
            {
                container.ReplaceSingleton(_ => GetRedisConnection(options.Data));

                container.ReplaceSingleton<IMessageBus>(s => new RedisMessageBus(new RedisMessageBusOptions
                {
                    Subscriber = s.GetRequiredService<ConnectionMultiplexer>().GetSubscriber(),
                    Topic = options.Topic,
                    Serializer = s.GetRequiredService<ISerializer>(),
                    LoggerFactory = s.GetRequiredService<ILoggerFactory>()
                }));
            }
            else if (string.Equals(options.Provider, "rabbitmq"))
            {
                container.ReplaceSingleton<IMessageBus>(s => new RabbitMQMessageBus(new RabbitMQMessageBusOptions
                {
                    ConnectionString = options.ConnectionString,
                    Topic = options.Topic,
                    Serializer = s.GetRequiredService<ISerializer>(),
                    LoggerFactory = s.GetRequiredService<ILoggerFactory>()
                }));
            }
        }

        private static ConnectionMultiplexer GetRedisConnection(IDictionary<string, string> options)
        {
            // TODO: Remove this extra config parse step when sentinel bug is fixed
            ConfigurationOptions config = ConfigurationOptions.Parse(options.GetString("server"));

            return ConnectionMultiplexer.Connect(config);
        }

        //private static void RegisterMetric(IServiceCollection container, MetricOptions options)
        //{
        //    if (string.Equals(options.Provider, "statsd"))
        //    {
        //        container.ReplaceSingleton<IMetricsClient>(s => new StatsDMetricsClient(new StatsDMetricsClientOptions
        //        {
        //            ServerName = options.Data.GetString("server", "127.0.0.1"),
        //            Port = options.Data.GetValueOrDefault("port", 8125),
        //            Prefix = "ex",
        //            LoggerFactory = s.GetRequiredService<ILoggerFactory>()
        //        }));
        //    }
        //    else
        //    {
        //        var metrics = BuildAppMetrics(options);
        //        if (metrics == null)
        //        {
        //            return;
        //        }

        //        container.ReplaceSingleton(metrics.Clock);
        //        container.ReplaceSingleton(metrics.Filter);
        //        container.ReplaceSingleton(metrics.DefaultOutputMetricsFormatter);
        //        container.ReplaceSingleton(metrics.OutputMetricsFormatters);
        //        container.ReplaceSingleton(metrics.DefaultOutputEnvFormatter);
        //        container.ReplaceSingleton(metrics.OutputEnvFormatters);
        //        container.TryAddSingleton<EnvironmentInfoProvider>();
        //        container.ReplaceSingleton<IMetrics>(metrics);
        //        container.ReplaceSingleton(metrics);
        //        container.ReplaceSingleton(metrics.Options);
        //        container.ReplaceSingleton(metrics.Reporters);
        //        container.ReplaceSingleton(metrics.ReportRunner);
        //        container.TryAddSingleton<AppMetricsMarkerService, AppMetricsMarkerService>();
        //        container.ReplaceSingleton<IMetricsClient, AppMetricsClient>();
        //    }
        //}

        //private static IMetricsRoot BuildAppMetrics(MetricOptions options)
        //{
        //    var metricsBuilder = AppMetrics.CreateDefaultBuilder();
        //    switch (options.Provider)
        //    {
        //        case "graphite":
        //            metricsBuilder.Report.ToGraphite(new MetricsReportingGraphiteOptions
        //            {
        //                Graphite = {BaseUri = new Uri(options.Data.GetString("server"))}
        //            });
        //            break;
        //        case "http":
        //            metricsBuilder.Report.OverHttp(new MetricsReportingHttpOptions
        //            {
        //                HttpSettings =
        //                {
        //                    RequestUri = new Uri(options.Data.GetString("server")),
        //                    UserName = options.Data.GetString("username"),
        //                    Password = options.Data.GetString("password")
        //                }
        //            });
        //            break;
        //        case "influxdb":
        //            metricsBuilder.Report.ToInfluxDb(new MetricsReportingInfluxDbOptions
        //            {
        //                InfluxDb =
        //                {
        //                    BaseUri = new Uri(options.Data.GetString("server")),
        //                    UserName = options.Data.GetString("username"),
        //                    Password = options.Data.GetString("password"),
        //                    Database = options.Data.GetString("database", "exceptionless")
        //                }
        //            });
        //            break;
        //        default:
        //            return null;
        //    }

        //    return metricsBuilder.Build();
        //}

        private static void RegisterQueue(IServiceCollection container, QueueOptions options, bool runMaintenanceTasks)
        {
            if (string.Equals(options.Provider, "redis"))
            {
                //container.ReplaceSingleton(s =>
                //    CreateRedisQueue<WorkItemData>(s, options, runMaintenanceTasks, workItemTimeout: TimeSpan.FromHours(1)));
            }
        }

        //private static void RegisterStorage(IServiceCollection container, StorageOptions options)
        //{
        //    if (string.Equals(options.Provider, "aliyun"))
        //    {
        //        container.ReplaceSingleton<IFileStorage>(s => new AliyunFileStorage(new AliyunFileStorageOptions
        //        {
        //            ConnectionString = options.ConnectionString,
        //            Serializer = s.GetRequiredService<ITextSerializer>(),
        //            LoggerFactory = s.GetRequiredService<ILoggerFactory>()
        //        }));
        //    }
        //    else if (string.Equals(options.Provider, "azurestorage"))
        //    {
        //        container.ReplaceSingleton<IFileStorage>(s => new AzureFileStorage(new AzureFileStorageOptions
        //        {
        //            ConnectionString = options.ConnectionString,
        //            ContainerName = $"{options.ScopePrefix}ex-events",
        //            Serializer = s.GetRequiredService<ITextSerializer>(),
        //            LoggerFactory = s.GetRequiredService<ILoggerFactory>()
        //        }));
        //    }
        //    else if (string.Equals(options.Provider, "folder"))
        //    {
        //        string path = options.Data.GetString("path", "|DataDirectory|\\storage");
        //        container.AddSingleton<IFileStorage>(s => new FolderFileStorage(new FolderFileStorageOptions
        //        {
        //            Folder = PathHelper.ExpandPath(path),
        //            Serializer = s.GetRequiredService<ITextSerializer>(),
        //            LoggerFactory = s.GetRequiredService<ILoggerFactory>()
        //        }));
        //    }
        //    else if (string.Equals(options.Provider, "minio"))
        //    {
        //        container.ReplaceSingleton<IFileStorage>(s => new MinioFileStorage(new MinioFileStorageOptions
        //        {
        //            ConnectionString = options.ConnectionString,
        //            Serializer = s.GetRequiredService<ITextSerializer>(),
        //            LoggerFactory = s.GetRequiredService<ILoggerFactory>()
        //        }));
        //    }
        //    else if (string.Equals(options.Provider, "s3"))
        //    {
        //        container.ReplaceSingleton<IFileStorage>(s => new S3FileStorage(new S3FileStorageOptions
        //        {
        //            ConnectionString = options.ConnectionString,
        //            Credentials = GetAWSCredentials(options.Data),
        //            Region = GetAWSRegionEndpoint(options.Data),
        //            Bucket = $"{options.ScopePrefix}{options.Data.GetString("bucket", "ex-events")}",
        //            Serializer = s.GetRequiredService<ITextSerializer>(),
        //            LoggerFactory = s.GetRequiredService<ILoggerFactory>()
        //        }));
        //    }
        //}

        //private static IQueue<T> CreateAzureStorageQueue<T>(IServiceProvider container, QueueOptions options, int retries = 2,
        //    TimeSpan? workItemTimeout = null) where T : class
        //{
        //    return new AzureStorageQueue<T>(new AzureStorageQueueOptions<T>
        //    {
        //        ConnectionString = options.ConnectionString,
        //        Name = GetQueueName<T>(options).ToLowerInvariant(),
        //        Retries = retries,
        //        Behaviors = container.GetServices<IQueueBehavior<T>>().ToList(),
        //        WorkItemTimeout = workItemTimeout.GetValueOrDefault(TimeSpan.FromMinutes(5.0)),
        //        Serializer = container.GetRequiredService<ISerializer>(),
        //        LoggerFactory = container.GetRequiredService<ILoggerFactory>()
        //    });
        //}

        private static IQueue<T> CreateRedisQueue<T>(IServiceProvider container, QueueOptions options, bool runMaintenanceTasks,
            int retries = 2, TimeSpan? workItemTimeout = null) where T : class
        {
            return new RedisQueue<T>(new RedisQueueOptions<T>
            {
                ConnectionMultiplexer = container.GetRequiredService<ConnectionMultiplexer>(),
                Name = GetQueueName<T>(options),
                Retries = retries,
                Behaviors = container.GetServices<IQueueBehavior<T>>().ToList(),
                WorkItemTimeout = workItemTimeout.GetValueOrDefault(TimeSpan.FromMinutes(5.0)),
                RunMaintenanceTasks = runMaintenanceTasks,
                Serializer = container.GetRequiredService<ISerializer>(),
                LoggerFactory = container.GetRequiredService<ILoggerFactory>()
            });
        }

        private static RedisCacheClient CreateRedisCacheClient(IServiceProvider container)
        {
            return new(new RedisCacheClientOptions
            {
                ConnectionMultiplexer = container.GetRequiredService<ConnectionMultiplexer>(),
                Serializer = container.GetRequiredService<ISerializer>(),
                LoggerFactory = container.GetRequiredService<ILoggerFactory>()
            });
        }

        //private static IQueue<T> CreateSQSQueue<T>(IServiceProvider container, QueueOptions options, int retries = 2,
        //    TimeSpan? workItemTimeout = null) where T : class
        //{
        //    return new SQSQueue<T>(new SQSQueueOptions<T>
        //    {
        //        Name = GetQueueName<T>(options),
        //        Credentials = GetAWSCredentials(options.Data),
        //        Region = GetAWSRegionEndpoint(options.Data),
        //        CanCreateQueue = false,
        //        Retries = retries,
        //        Behaviors = container.GetServices<IQueueBehavior<T>>().ToList(),
        //        WorkItemTimeout = workItemTimeout.GetValueOrDefault(TimeSpan.FromMinutes(5.0)),
        //        Serializer = container.GetRequiredService<ISerializer>(),
        //        LoggerFactory = container.GetRequiredService<ILoggerFactory>()
        //    });
        //}

        private static string GetQueueName<T>(QueueOptions options)
        {
            return string.Concat(options.ScopePrefix, typeof(T).Name);
        }

        //private static RegionEndpoint GetAWSRegionEndpoint(IDictionary<string, string> data)
        //{
        //    string region = data.GetString("region");
        //    return RegionEndpoint.GetBySystemName(string.IsNullOrEmpty(region) ? "us-east-1" : region);
        //}

        //private static AWSCredentials GetAWSCredentials(IDictionary<string, string> data)
        //{
        //    string accessKey = data.GetString("accesskey");
        //    string secretKey = data.GetString("secretkey");
        //    if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
        //    {
        //        return FallbackCredentialsFactory.GetCredentials();
        //    }

        //    return new BasicAWSCredentials(accessKey, secretKey);
        //}
    }
}
