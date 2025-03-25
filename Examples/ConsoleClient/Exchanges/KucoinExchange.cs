//using Bybit.Net.Clients;
using Kucoin.Net.Clients;
using Kucoin.Net.Interfaces.Clients;
//using Bybit.Net.Interfaces.Clients;
using ConsoleClient.Models;
using CryptoExchange.Net.Objects;
using CryptoExchange.Net.Objects.Sockets;
using CryptoExchange.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace ConsoleClient.Exchanges
{
    internal class KucoinExchange : IExchange
    {
        private IKucoinSocketClient _socketClient = new KucoinSocketClient();
        private CancellationToken CancellationToken = default;
        public KucoinExchange(CancellationToken cancellationToken)
        {
            this.CancellationToken = cancellationToken;
        }
        public async Task<WebCallResult> CancelOrder(string symbol, string idStr)
        {
            using var client = new KucoinRestClient();
            var result = await client.FuturesApi.Trading.CancelOrderAsync(idStr, CancellationToken);
            return result.AsDataless();
        }

        public async Task<Dictionary<string, decimal>> GetBalances()
        {
            using var client = new KucoinRestClient();
            var result = await client.FuturesApi.Account.GetAccountOverviewAsync(ct:CancellationToken);
            return new Dictionary<string, decimal>() { { "balance", result.Data.AvailableBalance } };
        }


        public async Task<IEnumerable<OpenOrder>> GetOpenOrders()
        {
            using var client = new KucoinRestClient();
            var order = await client.FuturesApi.Trading.GetOrdersAsync(ct: CancellationToken);
            return order.Data.Items.Select(o => new OpenOrder
            {
                Symbol = o.Symbol,
                OrderSide = o.Side.ToString(),
                OrderStatus = o.Status.ToString(),
                OrderTime = o.CreateTime,
                OrderType = o.Type.ToString(),
                Price = o.Price ?? 0,
                Quantity = o.Quantity ?? 0,
                QuantityFilled = o.QuantityFilled
            });
        }

        public async Task<decimal> GetPrice(string symbol)
        {
            using var client = new KucoinRestClient();
            //var r1 = await client.FuturesApi.ExchangeData.GetSymbolsAsync(CancellationToken);
            //var x = r1.Data.Where(x => x.Symbol.Contains("ETH")).ToList();
            //;
            var result = await client.FuturesApi.ExchangeData.GetTickerAsync(symbol, CancellationToken);
            return result.Data.Price;
        }

        public async Task<WebCallResult<string>> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
        {
            using var client = new KucoinRestClient();
            var result = await client.FuturesApi.Trading.PlaceOrderAsync(
                symbol,
                side.ToLower() == "buy" ? Kucoin.Net.Enums.OrderSide.Buy : Kucoin.Net.Enums.OrderSide.Sell,
                type == "market" ?  Kucoin.Net.Enums.NewOrderType.Market : Kucoin.Net.Enums.NewOrderType.Limit,
                quantity,
                price: price);
            return result.As(result.Data?.ClientOrderId.ToString());
        }

        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
        {
            var sub = await _socketClient.FuturesApi.SubscribeToKlineUpdatesAsync(symbol, Kucoin.Net.Enums.KlineInterval.OneMinute, data => handler(data.Data.ClosePrice));
            return sub.Data;
        }
    }
}
