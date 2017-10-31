namespace Nexus.RoleManagement.Services
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class BaseRepository : IBaseRepository
    {
        public Result Insert(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        public Result Update(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        public Result Delete(ICriteria criteria, bool hasGetOutputParams = false)
        {
            return ExecuteNonQuery(criteria, hasGetOutputParams);
        }

        private Result ExecuteNonQuery(ICriteria criteria, bool hasGetOutputParams = false)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                Hashtable hash = criteria.GetSpParams();
                hash = (new ResultCriteria()).AddSpParams(hash);
                int result = db.ExecuteNonQuery(hash);
                Result commandResult = GetCommandResult(hash);

                if (result == -1 && commandResult.IsSuccessful && hasGetOutputParams)
                {
                    SetValueForOutputProperty(criteria, hash);
                }

                return commandResult;
            }
        }

        private Result GetCommandResult(Hashtable hash)
        {
            var code = hash["ErrorCode"];
            var message = hash["ErrorMessage"];

            return new Result
            {
                Code = (code is DBNull) ? 0 : Convert.ToInt32(code),
                Message = (message is DBNull) ? string.Empty : Convert.ToString(message)
            };
        }

        private static void SetValueForOutputProperty(ICriteria criteria, Hashtable hash)
        {
            IEnumerable<PropertyInfo> properties = criteria.GetType().GetProperties().Where(
                prop => Attribute.IsDefined(prop, typeof(Attribute)));

            foreach (PropertyInfo property in properties)
            {
                if (property.CanWrite)
                {
                    var value = hash[property.Name.ToLowerInvariant()];
                    property.SetValue(criteria, value, null);
                }
            }
        }
    }
}