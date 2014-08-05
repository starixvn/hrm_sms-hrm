// --------------------------------------------------------------------------
// <copyright file="LookupDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Services.Interfaces
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.Common.Helper;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Criteria.Lookup;
    using Fanex.NexHR.Models;

    /// <summary>
    /// LookupData Service.
    /// </summary>
    public class LookupDataService : BaseService, ILookupDataService
    {
        private const string _CacheKey_AllLookUpData = "CacheKey_AllLookUpData";
        private ICache _cacheManager;

        public LookupDataService()
        {
            this._cacheManager = CacheService.CreateInstance();
        }

        /// <summary>
        /// Get all lookup data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The list lookup data.</returns>
        public IList<LookupData> GetAll(GetAllLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<LookupData> lookupDataList = null;

            if (criteria.IsValid())
            {
                IEnumerable<LookupData> rawLookupData = GetData<LookupData>(criteria);

                if (rawLookupData != null)
                {
                    lookupDataList = rawLookupData.ToList();
                }
            }

            return lookupDataList;
        }

        /// <summary>
        /// Get lookup by code and id.
        /// </summary>
        /// <param name="lookupCode">Lookup code.</param>
        /// <param name="lookupId">Lookup id.</param>
        /// <returns>Lookup data.</returns>
        public LookupData GetLookupByCodeAndId(string lookupCode, int lookupId)
        {
            Check.Argument.IsNotNegativeOrZero(lookupId, "lookupId");
            LookupData lookupData = new LookupData();
            var lookupDataList = this.GetByLookupCode(lookupCode);
            if (lookupDataList != null && lookupDataList.Count > 0)
            {
                lookupData = lookupDataList.Single(data => data.Id == lookupId);
            }

            return lookupData;
        }

        /// <summary>
        /// Get Job Title.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The enumerator Job Title.</returns>
        public IEnumerable<LookupData> GetJobTitle(GetJobTitleCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<LookupData> jobTitles = null;

            if (criteria.IsValid())
            {
                jobTitles = GetData<LookupData>(criteria);
            }

            return jobTitles;
        }

        /// <summary>
        /// Delete lookup data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult Delete(DeleteLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);

                if (commandResult.Code == 0)
                {
                    ResetJobTitleCache();
                }
            }

            return commandResult;
        }

        /// <summary>
        /// Insert lookup data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult Insert(InsertLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);

                if (commandResult.Code == 0)
                {
                    ResetJobTitleCache();
                }
            }

            return commandResult;
        }

        /// <summary>
        /// Update lookup data.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The Command Result.</returns>
        public CommandResult Update(UpdateLookupDataCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);

                if (commandResult.Code == 0)
                {
                    ResetJobTitleCache();
                }
            }

            return commandResult;
        }

        /// <summary>
        /// Function to reset JobTitle Cache.
        /// </summary>
        private void ResetJobTitleCache() 
        {
            var hashTable = this._cacheManager.Get<Hashtable>(_CacheKey_AllLookUpData);
            var lookUpCode = Constants.JobTitle.ToUpperInvariant();
            if (hashTable == null)
            {
                hashTable = new Hashtable();
            }

            var getAllJobTitleCriteria = new GetAllLookupDataCriteria
            {
                LookupCode = Constants.JobTitle
            };
            IList<LookupData> listJobTile = this.GetAll(getAllJobTitleCriteria);
            hashTable[lookUpCode] = listJobTile;
            this._cacheManager.Set<Hashtable>(_CacheKey_AllLookUpData, hashTable, TimeSpan.FromHours(1));
        }

        /// <summary>
        /// Get lookup data from hash table which is cached in memory.
        /// </summary>
        /// <param name="lookUpCode">Lookup code.</param>
        /// <returns>List lookup data.</returns>
        private IList<LookupData> GetByLookupCode(string lookUpCode)
        {
            Check.Argument.IsNotEmpty(lookUpCode, "lookUpCode");

            var hashTable = this._cacheManager.Get<Hashtable>(_CacheKey_AllLookUpData);
            lookUpCode = lookUpCode.ToUpperInvariant();
            if (hashTable == null)
            {
                hashTable = new Hashtable();
            }

            if (!hashTable.ContainsKey(lookUpCode))
            {
                var criteria = new GetAllLookupDataCriteria
                {
                    LookupCode = lookUpCode
                };
                IList<LookupData> lookUpDataList = GetAll(criteria);

                if (lookUpDataList != null && lookUpDataList.Count > 0)
                {
                    hashTable.Add(lookUpCode.ToUpperInvariant(), lookUpDataList);
                    this._cacheManager.Set<Hashtable>(_CacheKey_AllLookUpData, hashTable, TimeSpan.FromHours(1));
                }
            }

            return (IList<LookupData>)hashTable[lookUpCode];
        }
    }
}