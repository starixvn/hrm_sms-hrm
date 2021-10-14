namespace HRM.UserControl.Extensions.XML
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class XmlNameAttribute : Attribute
    {
        public XmlNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}