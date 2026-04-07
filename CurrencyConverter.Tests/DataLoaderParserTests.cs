using CurrencyConverter.Currency;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter.Tests
{
    public class DataLoaderParserTests
    {
        private readonly DataLoader _loader = new DataLoader(new HttpClientProvider(new HttpClient()));
        [Fact]
        public void Parse_ShouldReturnCorrectModel()
        {
            // Arrange
            string xml = File.ReadAllText("TestData/sample.xml");

            // Act
            var result = _loader.ParceCurrencyXml(xml);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Valute);

            var usd = result.Valute[0];
            Assert.Equal("USD", usd.CharCode);
            Assert.Equal("Доллар США", usd.Name);
            Assert.Equal("92,34", usd.Value);
        }

        [Fact]
        public void Parse_InvalidXml_ShouldThrow()
        {
            string xml = "<invalid>";

            Assert.ThrowsAny<Exception>(() => _loader.ParceCurrencyXml(xml));
        }

        [Fact]
        public void Parse_EmptyXml_ShouldThrow()
        {
            string xml = "";

            Assert.ThrowsAny<Exception>(() => _loader.ParceCurrencyXml(xml));
        }


    }
}
