namespace Fanex.NexHR.Models
{
    using System;

    public class EmployeeWorkFromHome
    {
        public int EmployeeWorkFromHomeId { get; set; }

        public int EmpId { get; set; }

        public string Nickname { get; set; }

        public string Fullname { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ModifiedBy { get; set; }

        public string Modifier { get; set; }

        public DateTime ModifiedDate { get; set; }

        public string Remark { get; set; }
    }
}