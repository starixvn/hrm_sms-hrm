namespace Fanex.NexHR.Services
{
    using Fanex.Common.Helper;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;
    using Fanex.NexHR.Repositories;
    using Fanex.NexHR.Repositories.Interfaces;
    using Fanex.NexHR.Resources;
    using Fanex.NexHR.Services.Interfaces;

    public class CommandService : ICommandService
    {
        private readonly IBaseRepository repository;

        public CommandService()
            : this(null)
        {
        }

        public CommandService(IBaseRepository repository)
        {
            this.repository = repository ?? new BaseRepository();
        }

        public CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false)
        {
            CommandResult result = null;
            if (criteria != null && criteria.IsValid())
            {
                result = repository.Insert(criteria, hasGetOutputParams);
            }
            else
            {
                result = GetInvalidInput();
            }

            return result;
        }

        public CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false)
        {
            CommandResult result = null;
            if (criteria != null && criteria.IsValid())
            {
                result = repository.Update(criteria, hasGetOutputParams);
            }
            else
            {
                result = GetInvalidInput();
            }

            return result;
        }

        public CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false)
        {
            CommandResult result = null;
            if (criteria != null && criteria.IsValid())
            {
                result = repository.Delete(criteria, hasGetOutputParams);
            }
            else
            {
                result = GetInvalidInput();
            }

            return result;
        }

        private CommandResult GetInvalidInput()
        {
            return new CommandResult()
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
        }
    }
}
