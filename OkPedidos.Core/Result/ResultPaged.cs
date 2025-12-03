using OkPedidos.Models.Models.Base;
using System.Net;
using System.Text.Json.Serialization;

namespace OkPedidos.Core.Result
{
    public class ResultPaged<TData> : Result<TData>
    {
        public ResultPaged() { }

        [JsonConstructor]
        public ResultPaged(TData data, int totalCount, int currentPage = 1, int pageSize = Configuration.DefaultPageSize, bool useCache = false) : base(HttpStatusCode.OK) //: base(data)
        {
            Data = data;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
            SourceData = (useCache ? $"cache" : $"database");
        }

        public ResultPaged(List<ErrorModel> error, string message = null) : base(HttpStatusCode.BadRequest)
        {
            Message = message;
            Errors = error;
        }

        public int CurrentPage { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public int PageSize { get; set; } = Configuration.DefaultPageSize;
        public int TotalCount { get; set; }
    }
}
