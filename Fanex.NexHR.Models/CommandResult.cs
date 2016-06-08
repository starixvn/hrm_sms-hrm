namespace Fanex.NexHR.Models
{
    public class CommandResult
    {
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
