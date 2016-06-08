namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.Data.Repository;
    using Fanex.NexHR.Models;

    public interface ICommandService
    {
        CommandResult Insert(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Update(ICriteria criteria, bool hasGetOutputParams = false);

        CommandResult Delete(ICriteria criteria, bool hasGetOutputParams = false);
    }
}