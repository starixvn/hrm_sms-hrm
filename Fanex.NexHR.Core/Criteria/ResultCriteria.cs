namespace Fanex.NexHR.Core.Criteria
{
    using Fanex.Data.Repository;

    public class ResultCriteria
    {
        public ResultCriteria()
        {
            ErrCode = 0;
            ErrMsg = string.Empty;
        }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}
