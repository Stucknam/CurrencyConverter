using CurrencyConverter.Models;
using CurrencyConverter.UI;
using System;
using System.Net.Http.Headers;
using System.Text;
using static CurrencyConverter.Models.ValCursData;

namespace CurrencyConverter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppServices services = new AppServices();

            MenuController menu = new MenuController(services);
            menu.Run();
        }
    }
}
