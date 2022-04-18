using System.Collections.Generic;
using System.Web;

namespace Fanex.NexHR.Services
{
    public interface IAttachFileService
    {
        void UploadFile(IList<HttpPostedFileBase> files, long fileId, string pathFolder);

        List<string> GetAttachFile(long fileId, string pathFolder);

        void DeleteFile(List<string> files, long fileId, string pathFolder);

        string GetInfoAttachFile(long fileId, string pathFolder);

        void DeleteFolderAttachFile(long id, string pathFolder);
    }
}
