using System.Net;
using System.Text.Json.Serialization;

namespace OkPedidos.Core.Result
{
    public class ResultBase
    {
        [JsonPropertyOrder(-4)]
        public string Title
        {
            get
            {
                if ((int)StatusCode is >= 200 and <= 299)
                    return "Sucess";
                else
                    return "Error";
            }
        }

        [JsonPropertyOrder(-3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SourceData { get; set; }


        [JsonPropertyOrder(-2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Code { get; set; }

        [JsonPropertyOrder(-1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Message { get; set; }

        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}
