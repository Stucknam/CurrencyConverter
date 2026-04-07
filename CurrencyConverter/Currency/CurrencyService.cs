using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurrencyConverter.Models.ValCursData;

namespace CurrencyConverter.Currency
{
    public interface ICurrencyService
    {
        Task<ValCursData> GetRates(DateOnly date);
        Task<ValuteRow?> GetCurrency(DateOnly date, string charCode);

        Task<List<string>> GetCharCodes(DateOnly date);
        IEnumerable<ValuteRow> SortByName(ValCursData data);
        IEnumerable<ValuteRow> SortByValue(ValCursData data);
    }
    public class CurrencyService:ICurrencyService
    {
        private readonly IDataLoader _dataLoader;
        private readonly ICurrencyCache _cache;
        public CurrencyService(IDataLoader dataLoader, ICurrencyCache cache )
        {
            _dataLoader = dataLoader;
            _cache = cache;
        }

        public async Task<ValCursData> GetRates(DateOnly date)
        {
            if (_cache.TryGet(date, out ValCursData data))
            {
                return data;
            }
            else
            {
                data = await _dataLoader.LoadCurrencyRates(date);
                _cache.Set(date, data);
                return data;
            }
        }

        public async Task<ValuteRow?> GetCurrency(DateOnly date, string charCode)
        {
            var data = await GetRates(date);
            return data.Valute.FirstOrDefault(v => v.CharCode.Equals(charCode, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<string>> GetCharCodes(DateOnly date)
        {
            var data = await GetRates(date);
            return data.Valute.Select(v => v.CharCode).ToList();
        }

        public IEnumerable<ValuteRow> SortByName(ValCursData data)
        {
            return data.Valute.OrderBy(v => v.Name);
        }

        public IEnumerable<ValuteRow> SortByValue(ValCursData data)
        {
            return data.Valute.OrderBy(v => v.Value);
        }

    }
}
