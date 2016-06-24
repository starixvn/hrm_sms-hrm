namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Text;

    public class AssignFunctionToRoleCriteria : CriteriaBase
    {
        private const string _FunctionTpl = "<Function FunctionId='{0}' IsRead='{1}' IsEdit='{2}' IsCreate='{3}' IsDelete='{4}'></Function>";

        public int RoleId { get; set; }

        [SpParam(Ignored = true)]
        public List<FunctionModel> Functions { get; set; }

        [SpParam(SqlParamName = "Functions")]
        public string FunctionsXml
        {
            get
            {
                var xml = new StringBuilder("<ROOT>");
                foreach (var function in Functions)
                {
                    xml.AppendFormat(
                        _FunctionTpl,
                        function.FunctionId,
                        function.IsRead,
                        function.IsEdit,
                        function.IsCreate,
                        function.IsDelete);
                }

                return xml.Append("</ROOT>").ToString();
            }
        }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return RoleId > 0 && Functions != null && Functions.Count > 0;
        }

        public override string GetSettingKey()
        {
            return "Function_AssignFunctionToRole";
        }
    }
}
