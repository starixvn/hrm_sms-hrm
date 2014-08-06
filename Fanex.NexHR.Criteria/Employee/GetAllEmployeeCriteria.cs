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
                int? disableUsers = null;
                if (EnableUser.HasValue)
                {
                    if (EnableUser.Value)
                    {
                        disableUsers = 0;
                    }
                    else
                    {
                        disableUsers = 1;
                    }
                }
                
                return disableUsers;
            }
        }

        public int? IsDelete 
        {
            get
            {
                int? deleteUsers = null;
                if (IsDeleteUser.HasValue)
                {
                    if (IsDeleteUser.Value)
                    {
                        deleteUsers = 1;
                    }
                    else
                    {
                        deleteUsers = 0;
                    }
                }

                return deleteUsers;
            }
        }

        public int? IsLocal 
        {
            get
            {
                int? localUser = null;
                if (IsLocalUser.HasValue)
                {
                    if (IsLocalUser.Value)
                    {
                        localUser = 1;
                    }
                    else
                    {
                        localUser = 0;
                    }
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
