namespace Fanex.NexHR.Models.Section
{
    using System;
    using Fanex.NexHR.Core.XMLExtensions;

    [Serializable]
    [XMLName("Row")]
    public class EditSectionSupervisor
    {
        public int OwnerId { get; set; }
        public ushort RequestType { get; set; }
        public ushort DisplayOrder { get; set; }
    }
}