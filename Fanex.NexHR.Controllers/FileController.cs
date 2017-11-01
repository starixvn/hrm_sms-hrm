namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    public class FileController : BaseController
    {
        [HttpPost]
        public string UploadFile(IEnumerable<HttpPostedFileBase> files, int categoryId, bool isOverride)
        {
            string pathAttachFiles = string.Empty;

            foreach (HttpPostedFileBase file in files)
            {
                if (file.ContentLength > 0)
                {
                    string pathAttach = SaveFileToServer(file, categoryId, isOverride);

                    pathAttachFiles = (string.IsNullOrEmpty(pathAttachFiles)) ? pathAttach : string.Concat(pathAttachFiles, ";/", pathAttach);
                }
            }

            return pathAttachFiles;
        }

        private string SaveFileToServer(HttpPostedFileBase file, int categoryId, bool isOverride)
        {
            string pathAttach = string.Empty;
            var fileName = Path.GetFileName(file.FileName);
            var dir = "~/file/uploads/" + categoryId;
            pathAttach = dir + "/" + fileName;

            if (isOverride)
            {
                while (System.IO.File.Exists(Server.MapPath(pathAttach)))
                {
                    fileName = "(1)" + fileName;
                    pathAttach = dir + "/" + fileName;
                }
            }

            if (!Directory.Exists(Server.MapPath(dir)))
            {
                Directory.CreateDirectory(Server.MapPath(dir));
            }

            var path = Path.Combine(Server.MapPath(dir), fileName);

            file.SaveAs(path);

            return pathAttach.Substring(2);
        }

        public bool Delete(string fileName)
        {
            System.IO.File.Delete(Server.MapPath("~/" + fileName));
            return true;
        }
    }
}