using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Http.Resilience;
using NLog;
using Polly;
using System.Xml;

namespace Acm.CurrencyResolver
{
    public class CurrencyResolver : ICurrencyResolver
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly Dictionary<CurrencyCode, Dictionary<DateOnly, decimal>> Cache = new();

        private static readonly HttpClient HttpClient;

        private static readonly CultureInfo ParseCulture;

        static CurrencyResolver()
        {
            var retryPipeline = new ResiliencePipelineBuilder<HttpResponseMessage>()
                .AddRetry(new HttpRetryStrategyOptions
                {
                    BackoffType = DelayBackoffType.Exponential,
                    MaxRetryAttempts = 3
                })
                .Build();

            var socketHandler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(5)
            };
            var resilienceHandler = new ResilienceHandler(retryPipeline)
            {
                InnerHandler = socketHandler,
            };

            HttpClient = new HttpClient(resilienceHandler);

            ParseCulture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            ParseCulture.NumberFormat.NumberDecimalSeparator = ",";
        }

        public decimal Resolve(CurrencyCode code, DateOnly date)
        {
            if (TryGetFromCache(code, date, out var value))
            {
                return value;
            }

            var result = ResolveFromApi(code, date);
            UpdateCache(code, date, result);
            return result;
        }

        private bool TryGetFromCache(CurrencyCode code, DateOnly date, out decimal value)
        {
            if (Cache.TryGetValue(code, out var currencyCache))
            {
                return currencyCache.TryGetValue(date, out value);
            }

            value = 0;
            return false;
        }

        private static void UpdateCache(CurrencyCode code, DateOnly date, decimal value)
        {
            if (!Cache.TryGetValue(code, out var currencyCache))
            {
                currencyCache = new Dictionary<DateOnly, decimal>();
                Cache.Add(code, currencyCache);
            }
            currencyCache[date] = value;
        }

        private static decimal ResolveFromApi(CurrencyCode code, DateOnly date)
        {
            // See API description at https://cbr.ru/development/SXML/
            var uri = $"https://cbr.ru/scripts/XML_daily_eng.asp?date_req={date:dd/MM/yyyy}";
            Logger.Debug("Requesting currency rates for {0:dd/MM/yyyy} from {1}", date, uri);
            using var response = HttpClient.GetAsync(uri).Result;
            response.EnsureSuccessStatusCode();
            using var stream = response.Content.ReadAsStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            var text = reader.ReadToEnd();
            Logger.Debug("Response content: {0}", text);

            var xml = new XmlDocument();
            xml.LoadXml(text);
            var root = xml.DocumentElement;
            if (root == null)
            {
                throw new InvalidDataException("The currencies API returned invalid response");
            }

            var currencyNode = root.SelectNodes("Valute")?.Cast<XmlNode>()
                .FirstOrDefault(node => node.SelectSingleNode("CharCode")?.InnerText == code.ToString());
            if (currencyNode == null)
            {
                throw new InvalidDataException($"A node for the currency {code} is not found");
            }

            var s = currencyNode.SelectSingleNode("Value")?.InnerText;
            Logger.Debug("Resolved currency {0} rate for date {1}: {2}", code, date, s);
            return s == null
                ? throw new InvalidDataException($"The currency {code} node does not have a currency Value")
                : decimal.Parse(s, NumberStyles.AllowDecimalPoint, ParseCulture);
        }
    }
}
