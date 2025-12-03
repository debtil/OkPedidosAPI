using static OkPedidosAPI.Helpers.Enums;

namespace OkPedidos.Models.Models.Base
{
    public class ErrorModel
    {
        public string Title { get; set; }
        public string Code { get; set; }
        public string Field { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public ErrorType Type { get; set; }
    }
}
