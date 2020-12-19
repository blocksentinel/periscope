using System;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Periscope.Data.Repositories;
using Periscope.Documents;

namespace Periscope.Data
{
    public abstract class RepositoryFactoryBase : IRepositoryFactory
    {
        protected readonly IMongoClient Client;
        protected readonly string DatabaseName;

        protected RepositoryFactoryBase(string connectionString, string dbTag)
        {
            DatabaseName = "cinder" + dbTag;
            MongoUrl url = new(connectionString);
            Client = new MongoClient(new MongoClientSettings
            {
                Server = new MongoServerAddress(url.Server.Host, url.Server.Port),
                ReadEncoding = new UTF8Encoding(false, false)
            });

            CreateMaps();
        }

        public TRepository CreateRepository<TRepository>() where TRepository : IRepository
        {
            IRepository repository = typeof(TRepository) switch
            {
                var t when t == typeof(AddressRepository) => new AddressRepository(Client, DatabaseName),
                var t when t == typeof(AddressTransactionRepository) => new AddressTransactionRepository(Client, DatabaseName),
                var t when t == typeof(BlockProgressRepository) => new BlockProgressRepository(Client, DatabaseName),
                var t when t == typeof(BlockRepository) => new BlockRepository(Client, DatabaseName),
                var t when t == typeof(ContractRepository) => new ContractRepository(Client, DatabaseName),
                var t when t == typeof(TransactionLogRepository) => new TransactionLogRepository(Client, DatabaseName),
                var t when t == typeof(TransactionRepository) => new TransactionRepository(Client, DatabaseName),
                var t when t == typeof(AddressMetaRepository) => new AddressMetaRepository(Client, DatabaseName),
                _ => throw new NotImplementedException($"Repository not implemented for type {typeof(TRepository).Name}")
            };

            return (TRepository) repository;
        }

        protected static void CreateMaps()
        {
            BsonClassMap.RegisterClassMap<TableRow>(map =>
            {
                map.AutoMap();
                map.UnmapMember(m => m.RowIndex);
                map.UnmapMember(m => m.RowCreated);
                map.UnmapMember(m => m.RowUpdated);
                map.SetIsRootClass(true);
            });
            BsonClassMap.RegisterClassMap<CinderAddress>(map =>
            {
                map.AutoMap();
                map.MapProperty(prop => prop.Balance).SetSerializer(new DecimalSerializer(BsonType.Decimal128));
            });
            BsonClassMap.RegisterClassMap<CinderAddressTransaction>();
            BsonClassMap.RegisterClassMap<CinderBlock>();
            BsonClassMap.RegisterClassMap<CinderContract>();
            BsonClassMap.RegisterClassMap<CinderTransaction>();
            BsonClassMap.RegisterClassMap<CinderTransactionLog>();

            BsonClassMap.RegisterClassMap<BlockProgress>(map =>
            {
                map.AutoMap();
                map.UnmapMember(m => m.LastBlockProcessed);
                map.SetIsRootClass(true);
            });
            BsonClassMap.RegisterClassMap<CinderBlockProgress>();
            BsonClassMap.RegisterClassMap<CinderAddressMeta>();
        }
    }
}
