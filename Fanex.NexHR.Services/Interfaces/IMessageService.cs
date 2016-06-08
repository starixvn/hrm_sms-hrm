namespace Fanex.NexHR.Services.Interfaces
{
    using Fanex.NexHR.Criteria.Message;
    using Fanex.NexHR.Models;
    using System.Collections.Generic;

    public interface IMessageService
    {
        IList<Message> GetMessageByCreator(GetMessageByCreatorCriteria criteria);

        Message GetMessageById(GetMessageByIdCriteria criteria);

        IList<Message> GetMessageByEmployee(GetMessageByEmployeeCriteria criteria);

        Message GetDetailMessageByEmployee(GetMessageOfEmployeeByIdCriteria criteria);

        IList<Message> GetUnreadMessageOfEmployee(GetUnreadMessageOfEmployeeCriteria criteria);

        void SendMailToStaff(Message model);
    }
}
