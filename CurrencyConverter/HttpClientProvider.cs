using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter
{
    public interface IHttpClientProvider
    {
        HttpClient Client { get; }
    }
    public class HttpClientProvider: IHttpClientProvider
    {
        public HttpClient Client { get; }
        public HttpClientProvider(HttpClient client) {
            Client = client;
        }
    }
}
