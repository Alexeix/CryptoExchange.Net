////using Bybit.Net.Clients;
//using BingX.Net.Clients;
//using BingX.Net.Interfaces.Clients;
////using Bybit.Net.Interfaces.Clients;
//using ConsoleClient.Models;
//using CryptoExchange.Net.Objects;
//using CryptoExchange.Net.Objects.Sockets;
//using CryptoExchange.Net.Sockets;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;


//namespace ConsoleClient.Exchanges
//{
//    internal class BingXExchange : IExchange
//    {
//        private IBingXSocketClient _socketClient = new BingXSocketClient();
//        private CancellationToken CancellationToken = default;
//        public BingXExchange(CancellationToken cancellationToken)
//        {
//            this.CancellationToken = cancellationToken;
//        }
//        public async Task<WebCallResult> CancelOrder(string symbol, string idStr)
//        {
//            long.TryParse(idStr, out var id);
//            using var client = new BingXRestClient();
//            var result = await client.PerpetualFuturesApi.Trading.CancelOrderAsync(symbol, id);
//            return result.AsDataless();
//        }

//        public async Task<Dictionary<string, decimal>> GetBalances()
//        {
//            using var client = new BingXRestClient();
//            var result = await client.PerpetualFuturesApi.Account.GetBalancesAsync();
//            return result.Data.ToDictionary(d => d.Asset, d => d.Balance );
//        }

//        public async Task<IEnumerable<OpenOrder>> GetOpenOrders()
//        {
//            using var client = new BingXRestClient();
//            var order = await client.PerpetualFuturesApi.Trading.GetOrdersAsync(ct: CancellationToken);
//            return order.Data.Select(o => new OpenOrder
//            {
//                Symbol = o.Symbol,
//                OrderSide = o.Side.ToString(),
//                OrderStatus = o.Status.ToString(),
//                OrderTime = o.CreateTime,
//                OrderType = o.Type.ToString(),
//                Price = o.Price ?? 0,
//                Quantity = o.Quantity ?? 0,
//                QuantityFilled = o.QuantityFilled ?? 0
//            });
//        }

//        public async Task<decimal> GetPrice(string symbol)
//        {
//            using var client = new BingXRestClient();
//            var result = await client.PerpetualFuturesApi.ExchangeData.GetRecentTradesAsync(symbol);
//            return result.Data.First().Value;
//        }

//        public async Task<WebCallResult<string>> PlaceOrder(string symbol, string side, string type, decimal quantity, decimal? price)
//        {
//            using var client = new BingXRestClient();
//            var result = await client.PerpetualFuturesApi.Trading.PlaceOrderAsync(
//                symbol,
//                side.ToLower() == "buy" ? BingX.Net.Enums.OrderSide.Buy : BingX.Net.Enums.OrderSide.Sell,
//                type == "market" ? BingX.Net.Enums.FuturesOrderType.Market : BingX.Net.Enums.FuturesOrderType.Limit,
//                BingX.Net.Enums.PositionSide.Long
//                quantity,
//                price: price);
//            return result.As(result.Data?.OrderId.ToString());
//        }

//        public async Task<UpdateSubscription> SubscribePrice(string symbol, Action<decimal> handler)
//        {
//            var sub = await _socketClient.V5SpotApi.SubscribeToTickerUpdatesAsync(symbol, data => handler(data.Data.LastPrice));
//            return sub.Data;
//        }
//    }
//}
