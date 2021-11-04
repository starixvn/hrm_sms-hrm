namespace HRM.UserControl.Models
{
    using System;
    using System.Xml.Serialization;
    using HRM.UserControl.Core.Extensions.XML;

    [Serializable]
    [XMLName("Row")]
    public class FunctionPermission
    {
        public int FunctionId { get; set; }

        [XmlIgnore]
        public string FunctionName { get; set; }

        [XmlIgnore]
        public string FunctionKey { get; set; }

        public bool IsRead { get; set; }
        public bool IsHost { get; set; }
    }
}