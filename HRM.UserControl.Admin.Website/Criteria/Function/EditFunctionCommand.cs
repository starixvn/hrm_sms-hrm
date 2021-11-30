namespace HRM.UserControl.Admin.Website.Criteria.Function
{
    public class EditFunctionCommand : BaseNonQueryCommand
    {
        public int FunctionId { get; set; }
        public short SiteId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionKey { get; set; }
        public int? ParentId { get; set; }
        public byte? Policy { get; set; }
        public short DisplayOrder { get; set; }
        public string Description { get; set; }
        public int ModifiedBy { get; set; }
        public bool IsActive { get; set; }

        public override string GetSettingKey() => "Function_Update";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(FunctionName) && ModifiedBy > 0;
    }
}
