// -----------------------------------------------------------------------
// <copyright file="SupervisorNameModel.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Supervisor Name Model.
    /// </summary>
    public class SupervisorNameModel
    {
        public int OwnerId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public string MailGroup { get; set; }

        public int ActionType { get; set; }
    }
}
