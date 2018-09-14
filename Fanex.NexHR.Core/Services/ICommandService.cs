namespace Fanex.NexHR.Core.Services
{
    using Core.Models;
    using Fanex.Data.Repository;

    public interface ICommandService
    {
        CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false);
    }
}