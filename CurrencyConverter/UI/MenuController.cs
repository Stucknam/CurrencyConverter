using CurrencyConverter.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CurrencyConverter.Models.ValCursData;

namespace CurrencyConverter.UI
{
    public class MenuController
    {
        private readonly AppServices appServices;
        public MenuController(AppServices appServices)
        {
            this.appServices = appServices;
        }

        public void ShowMainMenu()
        {
            Console.ResetColor();
            Console.Write("Что вы хотите сделать?\n" +
                    "\t1 - Курсы валют\n" +
                    "\t2 - Конвертировать рубли в валюту\n" +
                    "\t3 - Конвертировать валюту в рубли\n" +
                    "\t4 - Конвертировать валюту в валюту\n" +
                    "\tq - Выход\n" +
                    ">> ");
        }

        int ReadInt()
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out int result))
                return result;
            else
                throw new FormatException("Некорректный ввод. Ожидалось целое число.");

        }


        public void Run()
        {
            var _msg = appServices.MessageService;

            Console.Title = "Конвертер валют";

            _msg.PrintMessage("Добро пожаловать в конвертер валют!", MessageType.Accent);
            _msg.PrintMessage("Здесь вы можете узнать актуальные курсы валют и конвертировать деньги между различными валютами.", MessageType.Info);

            Console.WriteLine("============================================");

            string? input;

            do
            {

                ShowMainMenu();

                input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        // Получение и отображение курсов валют
                        GetRates().Wait();
                        Console.Clear();
                        break;
                    case "2":
                        ConvertRublesToCurrency().Wait();
                        Console.Clear();
                        break;
                    case "3":
                        ConvertCurrencyToRubles().Wait();
                        Console.Clear();
                        break;
                    case "4":
                        ConvertCurrencyToCurrency().Wait();
                        Console.Clear();
                        break;
                    case "q":
                    case "stop":
                        _msg.PrintMessage("Спасибо за использование конвертера валют! До свидания!", MessageType.Accent);
                        return;
                    default:

                        _msg.PrintMessage("Некорректный ввод.Пожалуйста, выберите одну из предложенных опций.", MessageType.Error);
                        Thread.Sleep(1500);
                        Console.Clear();
                        break;

                }
            } while (input?.ToLower() != "q" && input != "stop");
        }

        public void PaginateCurrencyRates(List<ValuteRow> rates, DateOnly date, int pageSize = 10)
        {
            var _msg = appServices.MessageService;
            var _pageService = appServices.pageService;

            int currentPage = 1;

            while (true)
            {
                Console.Clear();

                var page = _pageService.Paginate(rates, currentPage, pageSize);

                _msg.PrintTitle($"Курсы валют на {date:dd.MM.yyyy} Страница: {page.Page}/{page.TotalPages}");

                foreach (var item in page.Items)
                {
                    var type = (item.CharCode == "USD" || item.CharCode == "EUR")
                        ? MessageType.Accent
                        : MessageType.Default;

                    _msg.PrintMessage(item.ToString(), type);
                }

                Console.WriteLine("\nНавигация: [N]ext, [P]revious, [Q]uit");
                var input = Console.ReadKey(true).Key;
                if (input == ConsoleKey.N && page.HasNext)
                    currentPage++;
                else if (input == ConsoleKey.P && page.HasPrevious)
                    currentPage--;
                else if (input == ConsoleKey.Q)
                    break;
            }
        }

        public async Task GetRates()
        {
            var _msg = appServices.MessageService;
            var _currensyService = appServices.currencyService;

            while (true)
            {
                try
                {
                    Console.Clear();
                    _msg.PrintTitle("Курсы валют");

                    Console.Write(
                        "Введите дату (dd.mm.yyyy)\n" +
                        "Enter — сегодня\n" +
                        "[Q]uit — выход\n>> "
                    );

                    string? input = Console.ReadLine()?.Trim().ToLower();

                    if (input == "q" || input == "quit")
                        return;

                    DateOnly date;

                    if (string.IsNullOrEmpty(input))
                    {
                        date = DateOnly.FromDateTime(DateTime.UtcNow);
                    }
                    else if (!DateOnly.TryParse(input, out date))
                    {
                        throw new FormatException("Некорректный формат даты. Ожидалось dd.mm.yyyy.");
                    }

                    _msg.PrintMessage("Загрузка курсов валют...", MessageType.Info);
                    var valCursData = await _currensyService.GetRates(date);


                    _msg.PrintMessage("Курсы валют успешно загружены!", MessageType.Success);
                    // Вывод курсов валют с постраничной 
                    PaginateCurrencyRates(valCursData.Valute.ToList(), date);
                    return;

                }
                catch (HttpRequestException ex)
                {
                    _msg.PrintMessage(ex.Message, MessageType.Error);
                    Thread.Sleep(1500);
                }
                catch (Exception ex)
                {
                    _msg.PrintMessage(ex.Message, MessageType.Error);
                    Thread.Sleep(1500);
                }
            }

        }

        private string? AskCurrencyCode(List<ValuteRow> availableCurrencies)
        {
            var _msg = appServices.MessageService;

            while (true)
            {
                Console.Write("Введите код валюты (USD, EUR), [V] — список, [Q] — назад\n>> ");
                string? input = Console.ReadLine()?.Trim().ToUpper();

                if (string.IsNullOrEmpty(input))
                {
                    _msg.PrintMessage("Код валюты не может быть пустым.", MessageType.Error);
                    continue;
                }

                if (input == "Q")
                    return null;

                if (input == "V")
                {
                    Console.Clear();
                    Console.WriteLine("Список доступных валют:\n");

                    foreach (var v in availableCurrencies)
                        Console.WriteLine($"{v.CharCode} - {v.Name}");

                    Console.WriteLine();
                    continue;
                }

                return input;
            }
        }
        public async Task ConvertRublesToCurrency()
        {
            var _msg = appServices.MessageService;
            var _currency = appServices.currencyService;
            var _converter = appServices.Converter;

            Console.Clear();
            _msg.PrintTitle("Из рублей в валюту");

            // Загружаем курсов валют
            var rates = await _currency.GetRates(DateOnly.FromDateTime(DateTime.Today));

            // Выбор валюты
            string? code = AskCurrencyCode(rates.Valute.ToList());
            if (code is null)
                return;

            var valute = rates.Valute.FirstOrDefault(v => v.CharCode == code);
            if (valute is null)
            {
                _msg.PrintMessage("Валюта не найдена.", MessageType.Error);
                Thread.Sleep(1500);
                return;
            }

            // Конвертация
            while (true)
            {
                Console.Clear();
                _msg.PrintTitle($"Конвертация RUB -> {code}");

                Console.Write("Введите сумму в рублях (или Q для выхода): ");
                var input = Console.ReadLine()?.Trim().ToUpper();

                if (input == "Q")
                    return;

                if (!decimal.TryParse(input, out var rubles))
                {
                    _msg.PrintMessage("Неверная сумма.", MessageType.Error);
                    Thread.Sleep(1000);
                    continue;
                }

                // Конвертация
                if (!decimal.TryParse(valute.VunitRate, out var unitRate))
                {
                    _msg.PrintMessage("Ошибка курса валюты.", MessageType.Error);
                    Thread.Sleep(1000);
                    return;
                }

                decimal result = _converter.FormRubles(rubles, unitRate);

                _msg.PrintMessage($"{rubles} RUB = {result:F2} {code}", MessageType.Accent);

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }
        }
        public async Task ConvertCurrencyToRubles()
        {
            var _msg = appServices.MessageService;
            var _currency = appServices.currencyService;
            var _converter = appServices.Converter;

            Console.Clear();
            _msg.PrintTitle("Из валюты в рубли");

            // Загрузка кусов валют
            var rates = await _currency.GetRates(DateOnly.FromDateTime(DateTime.Today));

            // Выбор валюты
            string? code = AskCurrencyCode(rates.Valute.ToList());
            if (code is null)
                return;

            var valute = rates.Valute.FirstOrDefault(v => v.CharCode == code);
            if (valute is null)
            {
                _msg.PrintMessage("Валюта не найдена.", MessageType.Error);
                Thread.Sleep(1500);
                return;
            }

            // Конвертация
            while (true)
            {
                Console.Clear();
                _msg.PrintTitle($"Конвертация {code} -> RUB");

                Console.Write($"Введите сумму в {code} (или Q для выхода): ");
                var input = Console.ReadLine()?.Trim().ToUpper();

                if (input == "Q")
                    return;

                if (!decimal.TryParse(input, out var amount))
                {
                    _msg.PrintMessage("Неверная сумма.", MessageType.Error);
                    Thread.Sleep(1000);
                    continue;
                }

                // Конвертация
                if (!decimal.TryParse(valute.VunitRate, out var unitRate))
                {
                    _msg.PrintMessage("Ошибка курса валюты.", MessageType.Error);
                    Thread.Sleep(1000);
                    return;
                }

                decimal result = _converter.ToRubles(amount, unitRate);

                _msg.PrintMessage($"{amount} {code} = {result:F2} RUB", MessageType.Accent);

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }
        }

        public async Task ConvertCurrencyToCurrency()
        {
            var _msg = appServices.MessageService;
            var _currency = appServices.currencyService;
            var _converter = appServices.Converter;

            Console.Clear();
            _msg.PrintTitle("Из валюты в валюту");


            // Загрузка кусов валют
            var rates = await _currency.GetRates(DateOnly.FromDateTime(DateTime.Today));

            // Выбор валюты из которой конвертируем
            _msg.PrintMessage("Выберите валюту, из которой хотите конвертировать:", MessageType.Info);
            string? codeFrom = AskCurrencyCode(rates.Valute.ToList());
            if (codeFrom is null)
                return;

            var valuteFrom = rates.Valute.FirstOrDefault(v => v.CharCode == codeFrom);
            if (valuteFrom is null)
            {
                _msg.PrintMessage("Валюта не найдена.", MessageType.Error);
                Thread.Sleep(1500);
                return;
            }
            // Выбор валюты в которую конвертируем
            _msg.PrintMessage("Выберите валюту, в которую хотите конвертировать:", MessageType.Info);
            string? codeTo = AskCurrencyCode(rates.Valute.ToList());
            if (codeTo is null)
                return;

            var valuteTo = rates.Valute.FirstOrDefault(v => v.CharCode == codeTo);
            if (valuteTo is null)
            {
                _msg.PrintMessage("Валюта не найдена.", MessageType.Error);
                Thread.Sleep(1500);
                return;
            }


            // Конвертация
            while (true)
            {
                Console.Clear();
                _msg.PrintTitle($"Конвертация {codeFrom} -> {codeTo}");
                _msg.PrintMessage("Конвертация валюты происходит через рубли, данные могут быть не точными.", MessageType.Info);

                Console.Write($"Введите сумму в {codeFrom} (или Q для выхода): ");
                var input = Console.ReadLine()?.Trim().ToUpper();

                if (input == "Q")
                    return;

                if (!decimal.TryParse(input, out var amount))
                {
                    _msg.PrintMessage("Неверная сумма.", MessageType.Error);
                    Thread.Sleep(1000);
                    continue;
                }

                // Конвертация
                if (!decimal.TryParse(valuteFrom.VunitRate, out var fromUnitRate))
                {
                    _msg.PrintMessage("Ошибка курса валюты.", MessageType.Error);
                    Thread.Sleep(1000);
                    return;
                }

                if (!decimal.TryParse(valuteTo.VunitRate, out var toUnitRate))
                {
                    _msg.PrintMessage("Ошибка курса валюты.", MessageType.Error);
                    Thread.Sleep(1000);
                    return;
                }

                decimal result = _converter.Convert(amount, fromUnitRate, toUnitRate);

                _msg.PrintMessage($"{amount} {codeFrom} = {result:F2} {codeTo}", MessageType.Accent);

                Console.WriteLine("Нажмите любую клавишу для продолжения...");
                Console.ReadKey(true);
            }
        }

    }
}



