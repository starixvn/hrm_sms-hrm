namespace GO.Core.Models
{
    using System;

    [Serializable]
    public class CommandResult
    {
        public int ObjectId { get; set; }
        public int Code { get; set; } = 1;
        public string Message { get; set; } = "Invalid data.";
        public bool IsSuccessful => Code == 0;
    }
}