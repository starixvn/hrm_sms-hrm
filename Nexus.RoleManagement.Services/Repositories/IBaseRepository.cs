namespace Nexus.RoleManagement.Services
{
    using Fanex.Data.Repository;

    public interface IBaseRepository
    {
        Result Insert(ICriteria criteria, bool hasGetOutputParams = false);

        Result Update(ICriteria criteria, bool hasGetOutputParams = false);

        Result Delete(ICriteria criteria, bool hasGetOutputParams = false);
    }
}
