namespace Fanex.NexHR.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetOvertimeRecordCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EmpId")]
        public int? EmployeeId { get; set; }

        public DateTime Month { get; set; }

        public bool IsDebug { get; set; }

        [SpParam(Ignored = true)]
        public int Calendar { get; set; }

        public override string GetSettingKey()
        {
            return Calendar == 1 ? "ExportExcel_OvertimeFiscal" : "ExportExcel_OvertimeNormal";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}