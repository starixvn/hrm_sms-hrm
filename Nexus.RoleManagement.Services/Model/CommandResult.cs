namespace Nexus.RoleManagement.Services
{
    using Fanex.NexHR.Resources;

    public class Result
    {
        public Result()
        {
            Code = 1;
            Message = Messages.InvalidInputData;
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
