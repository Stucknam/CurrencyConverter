using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverter.Currency;
using CurrencyConverter.UI;

namespace CurrencyConverter
{
    public class AppServices
    {
        public IConverterService Converter { get; }
        public IDataLoader Loader { get; }
        public IHttpClientProvider Http { get; }
        public IMessageService MessageService { get; } = new ConsoleMessageService();
        public ICurrencyCache Cache { get; }
        public ICurrencyService currencyService { get; }
        public IPaginationService pageService { get; }


        public AppServices() {

            Http = new HttpClientProvider(new HttpClient());
            Converter = new ConverterService();
            Loader = new DataLoader(Http);
            Cache = new MemoryCashService();
            MessageService = new ConsoleMessageService();
            currencyService = new CurrencyService(Loader, Cache);
            pageService = new PaginateService();
        }
    }
}
