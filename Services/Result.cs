namespace Facturon.Services
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public static Result Ok(string message = "")
        {
            return new Result { Success = true, Message = message };
        }

        public static Result Fail(string message)
        {
            return new Result { Success = false, Message = message };
        }
    }
}
