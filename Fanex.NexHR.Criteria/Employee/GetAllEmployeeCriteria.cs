// --------------------------------------------------------------------------
// <copyright file="GetAllEmployeeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Bradley.Mac" date="2014.7.30">Created</change>
// </history>

namespace Fanex.NexHR.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get All Employee Criteria.
    /// </summary>
    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public string GroupIds { get; set; }

        public int? Enable 
        {
            get
            {
                int? disableUsers;
                if (EnableUser == true)
                {
                    disableUsers = 0;
                }
                else if (EnableUser == false)
                {
                    disableUsers = 1;
                }
                else
                {
                    disableUsers = null;
                }

                return disableUsers;
            }
        }

        public int? IsDelete 
        {
            get
            {
                int? deleteUsers;
                if (IsDeleteUser == true)
                {
                    deleteUsers = 1;
                }
                else if (IsDeleteUser == false)
                {
                    deleteUsers = 0;
                }
                else
                {
                    deleteUsers = null;
                }

                return deleteUsers;
            }
        }

        public int? IsLocal 
        {
            get
            {
                int? localUser;
                if (IsLocalUser == false)
                {
                    localUser = 0;
                }
                else if (IsLocalUser == true)
                {
                    localUser = 1;
                }
                else
                {
                    localUser = null;
                }

                return localUser;
            }
        }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public int? TotalRow { get; set; }

        public int? TitleId { get; set; }

        [SpParam(Ignored = true)]
        public bool? EnableUser { get; set; }

        [SpParam(Ignored = true)]
        public bool? IsDeleteUser { get; set; }

        [SpParam(Ignored = true)]
        public bool? IsLocalUser { get; set; }

        public override string GetSettingKey()
        {
            return "GetEmployees";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
