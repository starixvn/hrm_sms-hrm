// --------------------------------------------------------------------------
// <copyright file="LookupDataRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// Lookup Data Repository.
    /// </summary>
    public class LookupDataRepository : ILookupDataRepository
    {
        /// <summary>
        /// Get All Memo.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>List of LookupData Entity.</returns>
        public IList<LookupData> GetAll(ICriteria criteria)
        {
            IList<LookupData> listEntity = new List<LookupData>();
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (IDataReader reader = db.ExecuteReader(criteria.GetSpParams()))
                {
                    if (reader != null)
                    {
                        while (reader.Read())
                        {
                            var entity = new LookupData
                            {
                                Id = reader.ToInt("Id"),
                                LookupId = reader.ToInt("LookupId"),
                                Code = reader.ToString("Code"),
                                Name = reader.ToString("Name"),
                                Value = reader.ToInt("Value")
                            };
                            listEntity.Add(entity);
                        }
                    }
                }
            }

            return listEntity;
        }

        public IList<LookupData> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<LookupData> GetJobTitle(string jobTitle)
        {
            using (var db = new ObjectDb("Lookup_GetJobTitle"))
            {
                return db.Query<LookupData>(new { jobTitle }).ToList();
            }
        }

        public bool Delete(int id)
        {
            using (ObjectDb db = new ObjectDb("LookupData_Delete"))
            {
                return db.ExecuteNonQuery(new { id }) != 0;
            }
        }

        public bool Insert(string name, string lookupCode)
        {
            using (ObjectDb db = new ObjectDb("LookupData_Insert"))
            {
                return db.ExecuteNonQuery(new { name, lookupCode }) != 0;
            }
        }

        public bool Update(int id, string name)
        {
            using (ObjectDb db = new ObjectDb("LookupData_Update"))
            {
                return db.ExecuteNonQuery(new { id, name }) != 0;
            }
        }
    }
}
