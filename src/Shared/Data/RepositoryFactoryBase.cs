using System;
using System.Text;
using Cinder.Data.Repositories;
using Cinder.Documents;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;

namespace Cinder.Data
{
    public abstract class RepositoryFactoryBase : IRepositoryFactory
    {
        protected readonly IMongoClient Client;
        protected readonly string DatabaseName;

        protected RepositoryFactoryBase(string connectionString, string dbTag)
        {
            DatabaseName = "cinder" + dbTag;
            MongoUrl url = new MongoUrl(connectionString);
            Client = new MongoClient(new MongoClientSettings
            {
                Server = new MongoServerAddress(url.Server.Host, url.Server.Port),
                ReadEncoding = new UTF8Encoding(false, false)
            });


            CreateMaps();
        }

        public TRepository CreateRepository<TRepository>() where TRepository : IRepository
        {
            IRepository repository;
            switch (typeof(TRepository))
            {
                case var t when t == typeof(AddressRepository):
                    repository = new AddressRepository(Client, DatabaseName);
                    break;
                case var t when t == typeof(AddressTransactionRepository):
                    repository = new AddressTransactionRepository(Client, DatabaseName);
                    break;
                case var t when t == typeof(BlockProgressRepository):
                    repository = new BlockProgressRepository(Client, DatabaseName);
                    break;
                case var t when t == typeof(BlockRepository):
                    repository = new BlockRepository(Client, DatabaseName);
                    break;
                case var t when t == typeof(ContractRepository):
                    repository = new ContractRepository(Client, DatabaseName);
                    break;
                case var t when t == typeof(TransactionLogRepository):
                    repository = new TransactionLogRepository(Client, DatabaseName);
                    break;
                case var t when t == typeof(TransactionRepository):
                    repository = new TransactionRepository(Client, DatabaseName);
                    break;
                case var t when t == typeof(AddressMetaRepository):
                    repository = new AddressMetaRepository(Client, DatabaseName);
                    break;
                default:
                    throw new NotImplementedException($"Repository not implemented for type {typeof(TRepository).Name}");
            }

            return (TRepository) repository;
        }

        protected void CreateMaps()
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
