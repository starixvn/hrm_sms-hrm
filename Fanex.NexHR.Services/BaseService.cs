// -----------------------------------------------------------------------
// <copyright file="BaseService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="2014.7.17">Created.</change>
// </history>

namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;

    /// <summary>
    /// The base service.
    /// </summary>
    public class BaseService
    {
        private readonly IBaseRepository _Repository;
        private readonly IDynamicRepository _dynamicRepository;

        public BaseService()
            : this(null, null)
        {
        }

        public BaseService(IDynamicRepository dynamicRepository, IBaseRepository repository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _Repository = repository ?? new BaseRepository();
        }

        public IBaseRepository Repository
        {
            get
            {
                return _Repository;
            }
        }

        /// <summary>
        /// Function to get list object from database.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="criteria">Criteria to query database.</param>
        /// <returns>List Object.</returns>
        public virtual IEnumerable<T> GetData<T>(ICriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            if (criteria.IsValid())
            {
                return _dynamicRepository.Fetch<T>(criteria);
            }

            return new List<T>();
        }

        /// <summary>
        /// Function to get object from database.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="criteria">Criteria to query database.</param>
        /// <returns>The object.</returns>
        public virtual T GetDataEntity<T>(ICriteria criteria)
        {
            if (criteria.IsValid())
            {
                return _dynamicRepository.Get<T>(criteria);
            }

            return default(T);
        }
    }
}
