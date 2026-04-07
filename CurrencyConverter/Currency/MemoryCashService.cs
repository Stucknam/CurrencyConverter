using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Currency
{
    public interface ICurrencyCache
    {
        bool TryGet(DateOnly date, out ValCursData data);
        void Set(DateOnly date, ValCursData data);

    }
    /// <summary>
    /// Сервис кэширования данных о курсах валют
    /// </summary>
    public class MemoryCashService:ICurrencyCache
    {
            private readonly Dictionary<DateOnly, ValCursData> _cache = new();

            public bool TryGet(DateOnly date, out ValCursData data)
            {
                return _cache.TryGetValue(date, out data);
            }

            public void Set(DateOnly date, ValCursData data)
            {
                _cache[date] = data;
            }

    }
}
