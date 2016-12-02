using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebhookDemo.Controllers
{
    [EnableCors(origins: "http://localhost:14651", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }


        [HttpPost]
        public HttpResponseMessage WebHook([FromBody]StripeEvent stripeEvent)
        {
            // MVC3/4: Since Content-Type is application/json in HTTP POST from Stripe
            // we need to pull POST body from request stream directly
          
            if (stripeEvent == null)
                return new HttpResponseMessage(HttpStatusCode.BadRequest);

            switch (stripeEvent.Type)
            {
                case "charge.refunded":
                    // do work
                    break;

                case "customer.subscription.updated":
                case "customer.subscription.deleted":
                case "customer.subscription.created":
                    // do work
                    break;
            }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

     

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }

    public class StripeEvent 
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("request")]
        public string Request { get; set; }
    }

    internal class StripeDateTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteRawValue(@"""\/Date(" + EpochTime.ConvertDateTimeToEpoch((DateTime)value).ToString() + @")\/""");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null) return null;

            if (reader.TokenType == JsonToken.Integer)
                return EpochTime.ConvertEpochToDateTime((long)reader.Value);

            return DateTime.Parse(reader.Value.ToString());
        }
    }

    internal static class EpochTime
    {
        private static DateTime _epochStartDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime ConvertEpochToDateTime(long seconds)
        {
            return _epochStartDateTime.AddSeconds(seconds);
        }

        public static long ConvertDateTimeToEpoch(this DateTime datetime)
        {
            if (datetime < _epochStartDateTime) return 0;

            return Convert.ToInt64(datetime.Subtract(_epochStartDateTime).TotalSeconds);
        }
    }
}
