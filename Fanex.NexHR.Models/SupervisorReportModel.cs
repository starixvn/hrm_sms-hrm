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
                            return "VERIFIED BY";
                        case 3:
                            return "APPROVED BY";
                        case 4:
                            return "SHIFT ARRANGEMENT";
                        case 5:
                            return "ON BEHALF REQUEST";
                    }
                }

                return string.Empty;
            }
        }
    }
}
