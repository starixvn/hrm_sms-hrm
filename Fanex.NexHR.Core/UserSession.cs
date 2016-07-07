namespace Fanex.NexHR.Core
{
    using System;

    [Serializable]
    public class UserSession 
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public short Role { get; set; }
    }
}
