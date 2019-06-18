namespace Fanex.NexHR.Services
{
    using System;
    using Fanex.NexHR.Models;

    public interface IDashboardService
    {
        MySpaceModel GetMySpace(int employeeId, DateTime date);
    }
}