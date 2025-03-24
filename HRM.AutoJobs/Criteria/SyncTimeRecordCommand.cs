using System;
using Fanex.Data.Repository;

namespace HRM.AutoJobs.Criteria;

public class SyncTimeRecordCommand : NonQueryCommand
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string TimeRecordXml { get; set; }
    public override string GetSettingKey() => "Sync_TimeRecord_Import";

    public override bool IsValid() => true;
}