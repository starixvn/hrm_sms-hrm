namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;
    using System.Collections.Generic;
    using System.Text;

    public class AssignFunctionToRoleCriteria : CriteriaBase
    {
        private const string _FunctionTpl = "<Function FunctionId='{0}' IsRead='{1}' IsHost='{2}'></Function>";

        public int RoleId { get; set; }

        [SpParam(Ignored = true)]
        public List<FunctionViewModel> Functions { get; set; }

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
                        function.IsHost ? function.IsHost : function.IsRead,
                        function.IsHost);
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
