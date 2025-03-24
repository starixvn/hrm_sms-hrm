using System;

namespace HRM.AutoJobs.Models;

public class TimeRecord
{
    public string EmpNo { get; set; }
    public byte? InOutType { get; set; }
    public DateTime RecordTime { get; set; }
}