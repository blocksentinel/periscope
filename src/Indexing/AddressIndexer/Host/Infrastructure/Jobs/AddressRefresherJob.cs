using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Jobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Util;
using Nethereum.Web3;
using Periscope.Core.Extensions;
using Periscope.Core.SharedKernel;
using Periscope.Data.Repositories;
using Periscope.Documents;
using Periscope.Indexing.AddressIndexer.Host.Infrastructure.Settings;
using Periscope.Stats;

namespace Periscope.Indexing.AddressIndexer.Host.Infrastructure.Jobs
{
    public class AddressRefresherJob : JobBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly AddressRefresherSettings _settings;
        private readonly ScopedHybridCacheClient _statsCache;
        private readonly IWeb3 _web3;

        public AddressRefresherJob(ILoggerFactory loggerFactory, IOptions<AddressRefresherSettings> settings,
            IHybridCacheClient cacheClient, IAddressRepository addressRepository, IWeb3 web3) : base(loggerFactory)
        {
            _settings = settings.Value;
            _addressRepository = addressRepository;
            _statsCache = new ScopedHybridCacheClient(cacheClient, CacheScopes.Stats);
            _web3 = web3;
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

                IEnumerable<CinderAddress> addresses =
                    await _addressRepository.GetStaleAddresses(_settings.Age, _settings.Limit).AnyContext();
                IEnumerable<CinderAddress> enumerable = addresses as CinderAddress[] ?? addresses.ToArray();
                _logger.LogDebug("Found {Count} addresses to update", enumerable.Count());

                List<CinderAddress> updated = new List<CinderAddress>();
                foreach (CinderAddress address in enumerable)
                {
                    _logger.LogDebug("Updating stats for {Hash}", address.Hash);
                    HexBigInteger balance;

                    try
                    {
                        balance = await _web3.Eth.GetBalance.SendRequestAsync(address.Hash, new BlockParameter()).AnyContext();
                    }
                    catch (Exception e)
                    {
                        _logger.LogDebug(e, "Could not get balance for {Hash}", address.Hash);

                        continue;
                    }

                    address.Balance = UnitConversion.Convert.FromWei(balance);
                    address.Timestamp = (ulong) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    address.ForceRefresh = false;

                    if (_settings.BalanceHistory.Enabled && _settings.BalanceHistory.Days.Any())
                    {
                        IEnumerable<Task<HexBigInteger>> tasks = _settings.BalanceHistory.Days.Select(d =>
                        {
                            ulong block = GetPastBlock(netInfo.BestBlock, netInfo.AverageBlockTime, d);

                            return _web3.Eth.GetBalance.SendRequestAsync(address.Hash, new BlockParameter(block));
                        });

                        try
                        {
                            HexBigInteger[] results = await Task.WhenAll(tasks).AnyContext();

                            address.BalanceHistory.Clear();

                            for (int i = 0; i < _settings.BalanceHistory.Days.Length; i++)
                            {
                                address.BalanceHistory.Add(_settings.BalanceHistory.Days[i].ToString(),
                                    UnitConversion.Convert.FromWei(results[i]));
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.LogDebug(e, "Could not get historical balance for {Hash}", address.Hash);
                        }
                    }

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

        private static ulong GetPastBlock(ulong currentBlockNumber, decimal averageBlockTime, int days)
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

            return (ulong) pastBlock;
        }
    }
}
