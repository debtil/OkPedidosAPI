using OkPedidos.Models.Models.Base;
using OkPedidos.Shared.Constants.Models;
using System.Net;
using System.Text.Json.Serialization;

namespace OkPedidos.Core.Result
{
    public class Result<TData> : ResultBase
    {
        public Result() { }

        [JsonConstructor]
        public Result(HttpStatusCode statusCode, MessageDetail errorMsg)
        {
            StatusCode = statusCode;
            Code = errorMsg.Code;
            Message = errorMsg.Message;

        }

        public Result(TData data, HttpStatusCode statusCode, MessageDetail errorMsg)
        {
            StatusCode = statusCode;
            Data = data;
            Code = errorMsg.Code;
            Message = errorMsg.Message;
        }

        public Result(HttpStatusCode statusCode, string message = null)
        {
            StatusCode = statusCode;
            Code = null;
            Message = message;

        }
        public Result(TData data, HttpStatusCode statusCode, string message = null)
        {
            StatusCode = statusCode;
            Data = data;
            Message = message;
        }

        public Result(HttpStatusCode statusCode, List<ErrorModel> errors)
        {
            StatusCode = statusCode;
            Message = "One or more errors occurred.";
            Errors = errors;
        }


        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(998)]
        public TData Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyOrder(999)]
        public List<ErrorModel> Errors { get; set; }
    }
}
