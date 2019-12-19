using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Exceptions;
using Cinder.Data.Repositories;
using Cinder.Documents;
using Cinder.Extensions;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexTypes;
using Nethereum.Parity;
using Nethereum.Util;

namespace Cinder.Indexer.Host.Infrastructure
{
    public class AddressRefresherRunner : IAddressRefresherRunner
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger<AddressRefresherRunner> _logger;
        private readonly IWeb3Parity _web3;

        public AddressRefresherRunner(ILogger<AddressRefresherRunner> logger, IAddressRepository addressRepository,
            IWeb3Parity web3)
        {
            _logger = logger;
            _addressRepository = addressRepository;
            _web3 = web3;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<CinderAddress> addresses = await _addressRepository
                    .GetStaleAddresses(limit: 2500, cancellationToken: cancellationToken)
                    .AnyContext();
                IEnumerable<CinderAddress> enumerable = addresses as CinderAddress[] ?? addresses.ToArray();
                _logger.LogDebug("Found {Count} addresses to update", enumerable.Count());

                List<CinderAddress> updated = new List<CinderAddress>();
                foreach (CinderAddress address in enumerable)
                {
                    _logger.LogDebug("Updating balance for {Hash}", address.Hash);
                    HexBigInteger balance;

                    try
                    {
                        balance = await _web3.Eth.GetBalance.SendRequestAsync(address.Hash).AnyContext();
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    address.Balance = UnitConversion.Convert.FromWei(balance);
                    address.Timestamp = (ulong) DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                    address.ForceRefresh = false;
                    updated.Add(address);
                }

                if (!updated.Any())
                {
                    return;
                }

                await _addressRepository.BulkUpsertAddresses(updated, cancellationToken).AnyContext();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Class} -> {Method} -> Unexpected error", nameof(AddressRefresherRunner), nameof(Run));
                throw new LoggedException(e);
            }
        }
    }
}
