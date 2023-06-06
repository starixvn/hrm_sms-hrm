namespace GO.Website.Models.Manpower
{
    using System.Collections.Generic;

    public class UpdateFlowParam
    {
        public string UnitName { get; set; }
        public IList<ManpowerFlow> Flows { get; set; }
    }
}