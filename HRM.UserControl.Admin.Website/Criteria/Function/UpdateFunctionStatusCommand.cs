namespace HRM.UserControl.Admin.Website.Criteria.Function
{
    public class UpdateFunctionStatusCommand : BaseNonQueryCommand
    {
        public int FunctionId { get; set; }
        public bool IsActive { get; set; }
        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "Function_UpdateStatus";

        public override bool IsValid() => FunctionId > 0 && ModifiedBy > 0;
    }
}
