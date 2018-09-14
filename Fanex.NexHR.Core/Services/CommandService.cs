namespace Fanex.NexHR.Core.Services
{
    using Core.Models;
    using Core.Repositories;
    using Fanex.Data.Repository;
    using Fanex.NexHR.Resources;

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
            CommandResult result;

            result = (criteria != null && criteria.IsValid()) ?
                    repository.Insert(criteria, hasGetOutputParams) : GetInvalidInput();

            return result;
        }

        public CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false)
        {
            CommandResult result;

            result = (criteria != null && criteria.IsValid()) ?
                repository.Update(criteria, hasGetOutputParams) : GetInvalidInput();

            return result;
        }

        public CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false)
        {
            CommandResult result;

            result = (criteria != null && criteria.IsValid()) ?
                repository.Delete(criteria, hasGetOutputParams) : GetInvalidInput();

            return result;
        }

        private CommandResult GetInvalidInput()
        {
            return new CommandResult
            {
                Code = 1,
                Message = Messages.InvalidInputData
            };
        }
    }
}