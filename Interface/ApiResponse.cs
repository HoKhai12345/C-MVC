namespace NET_MVC.Interface
{
    public class ApiResponse<T>
    {
        public bool Status { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(bool status, int code, string message, T data = default)
        {
            Status = status;
            Code = code;
            Message = message;
            Data = data;
        }
    }
}
