using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using System.Xml.Serialization;
using CurrencyConverter.Models;

namespace CurrencyConverter.Currency
{
    public interface IDataLoader
    {
        Task<ValCursData> LoadCurrencyRates(DateOnly date);
        Task<ValCursData> LoadCurrencyRates();
    }
    /// <summary>
    /// Сервис для загрузки данных с API ЦБ РФ
    /// </summary>
    public  class DataLoader: IDataLoader
    {
        /// <summary>
        /// Базовый URL для получения курсов валют из ЦБ РФ. Курс валют предоставляется в виде XML-документа, 
        /// который необходимо распарсить для получения актуальных данных о курсах валют 
        /// на определенную дату или на текущую дату.
        /// </summary>
        private const string BaseUrl = "https://www.cbr.ru/scripts/XML_daily.asp";
        private readonly IHttpClientProvider _http;

        public DataLoader(IHttpClientProvider httpClientProvider)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);// Регистрация кодировки windows-1251
            _http = httpClientProvider;
        }
        /// <summary>
        /// Загрузка курсов валют с API ЦБ РФ на определенную дату
        /// </summary>
        /// <param name="date">Дата для получения курсов валют</param>
        /// <returns>Возвращает объект класса ValCursData соответствующий XSD схеме ответа сервера ЦБ</returns>
        /// <overloads>Загрузка курсов валют на текущую дату</overloads>
        /// <exception cref="HttpRequestException">Выбрасывается когда сервер возвращает пустые данные</exception>
        /// <seealso cref="LoadCurrencyRates()"/>
        public async Task<ValCursData> LoadCurrencyRates(DateOnly date)
        {
            string url = $"{BaseUrl}?date_req={date:dd/MM/yyyy}";
            string xml = await GetXmlAsync(url);
            var data = ParceCurrencyXml(xml);

            if (data.Valute == null || data.Valute.Count == 0)
                throw new HttpRequestException($"Сервер вернул пустые данные за {date:dd.MM.yyyy}");

            return data;
        }


        /// <summary>
        /// Загрузка курсов валют с API ЦБ РФ
        /// </summary>
        /// <returns>Возвращает объект класса ValCursData соответствующий XSD схеме ответа сервера ЦБ</returns>
        /// <overloads>Загрузка курсов валют на определенную дату</overloads>
        /// <seealso cref="LoadCurrencyRates(DateOnly)"/> 
        public async Task<ValCursData> LoadCurrencyRates()
        {
            string xml = await GetXmlAsync(BaseUrl);
            return ParceCurrencyXml(xml);
        }
        /// <summary>
        /// Парсинг XML-документа, полученного от сервера ЦБ РФ, в объект ValCursData.
        /// </summary>
        /// <param name="xml">XML ответ от сервера ЦБ</param>
        /// <returns>Возвращает объект класса ValCursData соответствующий XSD схеме ответа сервера ЦБ</returns>
        /// <exception cref="InvalidDataException">Выбрасывается когда входная строка пустая</exception>
        public ValCursData ParceCurrencyXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
                throw new InvalidDataException("Пустой xml от сервера ЦБ.");
            var serialazer = new XmlSerializer(typeof(ValCursData));
            using var reader = new StringReader(xml);
            return (ValCursData)serialazer.Deserialize(reader);
        }
        
        /// <summary>
        /// Получение курсов валют с сервера ЦБ РФ
        /// </summary>
        /// <param name="url">Ссылка на API</param>
        /// <returns></returns>
        private async Task<string> GetXmlAsync(string url)
        {
            
            var response = await _http.Client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            using var stream = await response.Content.ReadAsStreamAsync();
            using var reader = new StreamReader(stream, Encoding.GetEncoding("windows-1251"));
            return await reader.ReadToEndAsync();
        }
    }
}
