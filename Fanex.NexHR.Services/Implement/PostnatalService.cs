namespace Fanex.NexHR.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Models;
    using Core.Services;
    using Criteria.Postnatal;
    using Models;

    public class PostnatalService : IPostnatalService
    {
        private readonly IBaseService baseService;

        public PostnatalService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public CommandResult Delete(int id)
        {
            var criteria = new DeletePostnatalCriteria { PostnatalId = id };

            return baseService.ExecuteNonQuery(criteria);
        }

        public CommandResult Update(UpdatePostnatalCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        public IList<EmployeePostnatal> GetAll(int year)
        {
            var criteria = new GetPostnatalCriteria { Year = year };
            IList<EmployeePostnatal> postnatals = null;
            if (criteria.IsValid())
            {
                postnatals = baseService.GetData<EmployeePostnatal>(criteria).ToList();
            }

            return postnatals ?? new List<EmployeePostnatal>();
        }

        public EmployeePostnatal GetById(int id)
        {
            var criteria = new GetPostnatalByIdCriteria { PostnatalId = id };

            EmployeePostnatal postnatal = null;
            if (criteria.IsValid())
            {
                postnatal = baseService.GetDataEntity<EmployeePostnatal>(criteria);
            }
            return postnatal ?? new EmployeePostnatal();
        }

        public CommandResult Insert(InsertPostnatalCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}