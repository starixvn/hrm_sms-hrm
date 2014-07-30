// -----------------------------------------------------------------------
// <copyright file="BaseRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.16">Created</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Criteria;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// The Base Repository.
    /// </summary>
    public class BaseRepository : IBaseRepository
    {
        public CommandResult Insert(ICriteria criteria)
        {
            return ExecuteNonQuery(criteria);
        }

        public CommandResult Update(ICriteria criteria)
        {
            return ExecuteNonQuery(criteria);
        }

        public CommandResult Delete(ICriteria criteria)
        {
            return ExecuteNonQuery(criteria);
        }

        /// <summary>
        /// Function to insert, update or delete.
        /// </summary>
        /// <param name="criteria">Criteria contains parameter throw stored procedure.</param>
        /// <returns>Command Result.</returns>
        private static CommandResult ExecuteNonQuery(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                db.ExecuteNonQuery(hash);

                return GetCommandResult(hash);
            }
        }

        private static CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errcode"];
            var errMsg = hash["errmsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}
