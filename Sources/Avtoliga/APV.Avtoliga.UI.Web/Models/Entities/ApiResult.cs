namespace APV.Avtoliga.UI.Web.Models.Entities
{
    public sealed class ApiResult
    {
        public ApiResult()
        {
        }

        public ApiResult(bool success, string message = null)
        {
            Success = success;
            Message = message;
        }

        public bool Success { get; set; }

        public int SuccessValue
        {
            get { return (Success ? 1 : 0); }
        }

        public string Message { get; set; }
    }
}