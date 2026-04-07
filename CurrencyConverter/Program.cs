using CurrencyConverter.Models;
using CurrencyConverter.UI;
using System;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;

namespace CurrencyConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppServices services = new AppServices();

            // Устанавливаем культуру по умолчанию для корректного парсинга чисел.
            // В Linux (Docker) по умолчанию используется en-US, где разделитель — точка.
            // Курсы ЦБ РФ используют запятую, поэтому задаём ru-RU вручную.
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("ru-RU");

            MenuController menu = new MenuController(services);

            menu.Run();
        }
    }
}
