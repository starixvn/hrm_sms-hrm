namespace Fanex.NexHR.Models.Request
{
    using System.Collections.Generic;

    public class ApproveRequestsParam
    {
        public IEnumerable<RequestsXML> Requests { get; set; }

        public int ActorId { get; set; }
        public byte RoleId { get; set; }
        public string Comment { get; set; }
        public bool IsIgnoredViolate { get; set; }
        public bool IsConfirmed => !IsIgnoredViolate;
    }
}