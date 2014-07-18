// --------------------------------------------------------------------------
// <copyright file="SupervisorReportModel.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.17">Created</change>
// </history>

namespace Fanex.NexHR.Models
{
    /// <summary>
    /// Supervisor Report Model.
    /// </summary>
    public class SupervisorReportModel
    {
        public int EmpId { get; set; }

        public string FullName { get; set; }

        public string NickName { get; set; }

        public int GroupIds { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Supervisors { get; set; }

        public int ActionType { get; set; }

        public string ActionName
        {
            get
            {
                if (ActionType > 0)
                {
                    switch (ActionType)
                    {
                        case 2:
                            return "Verified By";
                        case 3:
                            return "Approved By";
                        case 4:
                            return "Shift Arrangement";
                        case 5:
                            return "On Behalf Request";
                    }
                }

                return string.Empty;
            }
        }
    }
}
