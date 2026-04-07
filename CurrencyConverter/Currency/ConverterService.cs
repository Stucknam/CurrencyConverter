using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Currency
{
    public interface IConverterService
    {
        decimal FormRubles(decimal amount, decimal rate);
        decimal ToRubles(decimal amount, decimal rate);
        decimal Convert(decimal amount, decimal fromRate, decimal toRate);
    }
    public class ConverterService:IConverterService
    {
        public decimal FormRubles(decimal amount, decimal rate)
        {
            return amount / rate;
        }

        public decimal ToRubles(decimal amount, decimal rate)
        {
            return amount * rate;
        }

        public decimal Convert(decimal amount, decimal fromRate, decimal toRate)
        {
            decimal rubles = ToRubles(amount, fromRate);
            return FormRubles(rubles, toRate);
        }
    }
}
