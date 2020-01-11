using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Documents;
using Cinder.Extensions;
using Cinder.Stats;
using Foundatio.Caching;
using Foundatio.Jobs;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.Parity;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using StackExchange.Redis;

namespace Cinder.Indexing.AddressIndexer.Host.Infrastructure.Jobs
{
    public class AddressRefresherJob : JobBase, IDisposable
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ICacheClient _statsCache;
        private readonly IWeb3Parity _web3;

        public AddressRefresherJob(ILoggerFactory loggerFactory, IConnectionMultiplexer muxer,
            IAddressRepository addressRepository, IWeb3Parity web3) : base(loggerFactory)
        {
            _addressRepository = addressRepository;
            _web3 = web3;
            _statsCache =
                new StatsCache(new RedisCacheClientOptions {ConnectionMultiplexer = muxer, LoggerFactory = loggerFactory});
        }

        public void Dispose()
        {
            _statsCache?.Dispose();
        }

        protected override async Task<JobResult> RunInternalAsync(JobContext context)
        {
            try
            {
                NetInfo netInfo = await _statsCache.GetAsync<NetInfo>(NetInfo.DefaultCacheKey, null).AnyContext();
                if (netInfo == null)
                {
                    return JobResult.FailedWithMessage("NetInfo is null");
                }

                IEnumerable<CinderAddress> addresses = await _addressRepository.GetStaleAddresses(limit: 2500).AnyContext();
                IEnumerable<CinderAddress> enumerable = addresses as CinderAddress[] ?? addresses.ToArray();
                _logger.LogDebug("Found {Count} addresses to update", enumerable.Count());

                List<CinderAddress> updated = new List<CinderAddress>();
                foreach (CinderAddress address in enumerable)
                {
                    _logger.LogDebug("Updating stats for {Hash}", address.Hash);
                    HexBigInteger balance;
                    HexBigInteger balance7;
                    HexBigInteger balance30;

                    ulong blockNumber = netInfo.BestBlock;
                    long blockNumber7DaysAgo = GetPastBlock(blockNumber, netInfo.AverageBlockTime, 7);
                    long blockNumber30DaysAgo = GetPastBlock(blockNumber, netInfo.AverageBlockTime, 30);

                    try
                    {
                        List<Task<HexBigInteger>> test = new List<Task<HexBigInteger>>
                        {
                            _web3.Eth.GetBalance.SendRequestAsync(address.Hash, new BlockParameter(blockNumber)),
                            _web3.Eth.GetBalance.SendRequestAsync(address.Hash,
                                new BlockParameter((ulong) blockNumber7DaysAgo)),
                            _web3.Eth.GetBalance.SendRequestAsync(address.Hash,
                                new BlockParameter((ulong) blockNumber30DaysAgo))
                        };

                        HexBigInteger[] result = await Task.WhenAll(test).AnyContext();
                        balance = result[0];
                        balance7 = result[1];
                        balance30 = result[2];
                    }
                    catch (Exception e)
                    {
                        _logger.LogDebug(e, "Exception when trying to get balance for {Hash}", address.Hash);

                        continue;
                    }

                    address.Balance = UnitConversion.Convert.FromWei(balance);
                    address.Timestamp = (ulong) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    address.ForceRefresh = false;

                    address.BalanceHistory.Clear();
                    address.BalanceHistory.Add("7", UnitConversion.Convert.FromWei(balance7));
                    address.BalanceHistory.Add("30", UnitConversion.Convert.FromWei(balance30));

                    updated.Add(address);
                }

                if (!updated.Any())
                {
                    return JobResult.Success;
                }

                await _addressRepository.BulkUpsertAddresses(updated).AnyContext();
            }
            catch (Exception e)
            {
                return JobResult.FromException(e);
            }

            return JobResult.Success;
        }

        private static long GetPastBlock(ulong currentBlockNumber, decimal averageBlockTime, int days)
        {
            if (averageBlockTime == 0)
            {
                averageBlockTime = 15;
            }

            long pastBlock = (long) Math.Floor(currentBlockNumber - days * 1440 * (60 / averageBlockTime));

            if (pastBlock < 0)
            {
                pastBlock = (long) currentBlockNumber;
            }

            return pastBlock;
        }
    }
}
