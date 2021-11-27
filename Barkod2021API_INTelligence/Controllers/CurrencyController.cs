using Barkod2021API_INTelligence.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Barkod2021API_INTelligence.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController:ControllerBase
    {
        private const string _ACCESSKEY = "96c7a03cce11e464756d645302fbb324";


        [HttpGet]
        [Route("/Convert")]
        public async Task<ActionResult> ConvertCurrencies([FromQuery] CurrencyModel currencyModel)
        {
            HttpClient httpClient = new HttpClient();
            ConversionResponseModel responseModel = new ConversionResponseModel();
            string url = "https://api.currencylayer.com/convert?from=" + currencyModel.fromCurr + "&to=" + currencyModel.toCurr + "&amount=" + currencyModel.amount + "&access_key=96c7a03cce11e464756d645302fbb324";
            var response = await httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseString);
            var result = jObject.SelectToken("result");
            DateTime date = DateTime.Now;
            string dateToChange = date.ToString();
            DateTimeOffset dateTimeOffset = DateTimeOffset.Parse(dateToChange);
            long unixTimeStamp = dateTimeOffset.ToUnixTimeMilliseconds();
            responseModel.result = currencyModel.amount * Decimal.Parse(result.ToString());
            responseModel.result = Math.Round(responseModel.result, 2);
            responseModel.timeStamp = unixTimeStamp;
            return Ok(responseModel);
        }

        [HttpGet]
        [Route("/convertWithHistoricalDate")]
        public async Task<ActionResult<string>> getHistoricalCurrencies([FromQuery] CurrencyModel model)
        {
            HttpClient httpClient = new HttpClient();
            ConversionResponseModel responseModel = new ConversionResponseModel();
            var response = await httpClient.GetAsync("https://api.currencylayer.com/historical?date=" + model.time + "&source=" + model.fromCurr + "&access_key=96c7a03cce11e464756d645302fbb324");
            var responseString = await response.Content.ReadAsStringAsync();
            var key = model.fromCurr + model.toCurr;
            JObject jObject = JObject.Parse(responseString);
            var currencies = jObject.SelectToken("quotes");
            var currencyString = currencies.SelectToken(key).ToString();
            responseModel.result = model.amount * Decimal.Parse(currencyString);
            responseModel.result = Math.Round(responseModel.result, 2);
            responseModel.timeStamp = (long.Parse((jObject.SelectToken("timestamp")).ToString()))*1000;
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(responseModel.timeStamp);
            DateTime date = dateTimeOffset.DateTime;
            return Ok(responseModel);
        }

        [HttpGet]
        [Route("/GetRSDQuotes")]
        public async Task<ActionResult<string>> GetRSDQuotes()
        {
            HttpClient httpClient = new HttpClient();
            DateTime dateTime = DateTime.Now;
            string date = dateTime.Year + "-" + dateTime.Month + "-" + dateTime.Day;
            string url = "https://api.currencylayer.com/historical?date="+date+"&source=RSD&access_key="+_ACCESSKEY;
            var response = await httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseString);
            var quotes = jObject.SelectToken("quotes");
            return Ok(quotes.ToString());
        }
    }
}
