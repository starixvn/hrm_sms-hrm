namespace HRM.Letstalk.Client.Extentions.XML
{
    using System;

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class XMLNameAttribute : Attribute
    {
        public XMLNameAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; }
    }
}
