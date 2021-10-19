namespace HRM.UserControl.Admin.Website.Models.Session
{
    using System;
    using HRM.UserControl.Admin.Website.Enumerations;

    [Serializable]
    public class FunctionSession
    {
        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string Name { get; set; }

        public Policy Policy { get; set; }
    }
}