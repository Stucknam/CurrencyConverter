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
    /// <summary>
    /// Сервис позволяющий конвертировать рубли в валюту, валюту в рубли и валюту в валюту через рубли
    /// </summary>
    public class ConverterService:IConverterService
    {
        /// <summary>
        /// Конвертация из рублей в валюту
        /// </summary>
        /// <param name="amount">Сумма в рублях</param>
        /// <param name="rate">Коэффициент конвертации</param>
        /// <returns>Возвращает сумму в конвертированной валюте</returns>
        public decimal FormRubles(decimal amount, decimal rate)
        {
            return amount / rate;
        }
        /// <summary>
        /// Конвертация из валюты в рубли
        /// </summary>
        /// <param name="amount">Сумма в валюте конвертации</param>
        /// <param name="rate">Коэффициент конвертации</param>
        /// <returns>Возвращает сконвертированную сумму в рублях></returns>
        public decimal ToRubles(decimal amount, decimal rate)
        {
            return amount * rate;
        }
        /// <summary>
        /// Конвертация из валюты в валюту через рубли
        /// </summary>
        /// <param name="amount">Сумма в конвертируемой валюте</param>
        /// <param name="fromRate">Коэффициент конвертируемой валюты к рублю</param>
        /// <param name="toRate">Коэфициент целевой валюты к рублю</param>
        /// <returns></returns>
        public decimal Convert(decimal amount, decimal fromRate, decimal toRate)
        {
            decimal rubles = ToRubles(amount, fromRate);
            return FormRubles(rubles, toRate);
        }
    }
}
