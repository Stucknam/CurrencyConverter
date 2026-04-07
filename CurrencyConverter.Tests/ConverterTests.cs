using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CurrencyConverter.Currency;

namespace CurrencyConverter.Tests
{
    public class ConverterTests
    {
        private readonly ConverterService _converter = new ConverterService();

        [Fact]
        public void FormRubles_ShouldReturnCorrectValue()
        {
            decimal amount = 100;
            decimal rate = 50;
            decimal expected = 2;
            decimal result = _converter.FormRubles(amount, rate);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToRubles_ShouldReturnCorrectValue()
        {
            decimal amount = 2;
            decimal rate = 50;
            decimal expected = 100;
            decimal result = _converter.ToRubles(amount, rate);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_ShouldReturnCorrectValue()
        {
            decimal amount = 100;
            decimal fromRate = 50;
            decimal toRate = 25;
            decimal expected = 200;
            decimal result = _converter.Convert(amount, fromRate, toRate);
            Assert.Equal(expected, result);

        }

        [Fact]
        public void Convert_ShouldReturnSameValue_WhenFromAndToRatesAreEqual()
        {
            decimal amount = 100;
            decimal rate = 50;
            decimal expected = 100;
            decimal result = _converter.Convert(amount, rate, rate);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_ShouldReturnZero_WhenAmountIsZero()
        {
            decimal amount = 0;
            decimal fromRate = 50;
            decimal toRate = 25;
            decimal expected = 0;
            decimal result = _converter.Convert(amount, fromRate, toRate);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Convert_LargeAmount_ShouldReturnCorrectValue()
        {
            decimal amount = 1000000;
            decimal fromRate = 50;
            decimal toRate = 25;
            decimal expected = 2000000;
            decimal result = _converter.Convert(amount, fromRate, toRate);
            Assert.Equal(expected, result);
        }
    }
}
