namespace Nexus.RoleManagement.Services
{
    public class Result
    {
        public Result()
        {
            Code = 1;
            Message = "Invalid Data";
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful
        {
            get
            {
                return Code == 0;
            }
        }
    }
}
