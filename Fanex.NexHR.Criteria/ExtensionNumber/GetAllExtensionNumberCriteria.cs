namespace Fanex.NexHR.Criteria.ExtensionNumber
{
    using System;
    using Fanex.Data.Repository;

    public class GetAllExtensionNumberCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "ExtensionNO_GetAll";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
