namespace HRM.UserControl.Client.Core.Models
{
    using System;

    [Serializable]
    public class FunctionPermission
    {
        public int FunctionId { get; set; }
        public string FunctionName { get; set; }
        public string FunctionKey { get; set; }
        public bool IsRead { get; set; }
        public bool IsHost { get; set; }
    }
}