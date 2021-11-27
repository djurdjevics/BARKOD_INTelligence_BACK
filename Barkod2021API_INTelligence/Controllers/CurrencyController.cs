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
        [HttpGet]
        [Route("/Convert")]
        public async Task<ActionResult> ConvertCurrencies([FromQuery] CurrencyModel currencyModel)
        {
            HttpClient httpClient = new HttpClient();
            string url = "https://api.currencylayer.com/convert?from="+currencyModel.fromCurr+"&to="+currencyModel.toCurr+"&amount="+currencyModel.amount+"&access_key=96c7a03cce11e464756d645302fbb324";
            var response = await httpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(responseString);
            var result = jObject.SelectToken("result");
            decimal resultDecimal = Decimal.Parse(result.ToString());
            resultDecimal = Math.Round(resultDecimal, 2);
            return Ok(resultDecimal.ToString());
        }

        [HttpGet]
        [Route("/convertWithHistoricalDate")]
        public async Task<ActionResult<string>> getHistoricalCurrencies([FromQuery] CurrencyModel model)
        {
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://api.currencylayer.com/historical?date=" + model.time + "&source=" + model.fromCurr + "&access_key=96c7a03cce11e464756d645302fbb324");
            var responseString = await response.Content.ReadAsStringAsync();
            var key = model.fromCurr + model.toCurr;
            JObject jObject = JObject.Parse(responseString);
            var currencies = jObject.SelectToken("quotes");
            var currencyString = currencies.SelectToken(key).ToString();
            decimal result = model.amount * Decimal.Parse(currencyString);
            result = Math.Round(result, 2);
            return Ok(result.ToString());
        }
    }
}
