namespace Fanex.NexHR.Controllers
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using System.Web.Mvc;

    public class FileController : BaseController
    {
        [HttpPost]
        public string UploadFile(IEnumerable<HttpPostedFileBase> files, int categoryID, int overRide)
        {
            string pathAttachFiles = string.Empty;

            foreach (HttpPostedFileBase file in files)
            {
                string pathAttach = string.Empty;

                if (file.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var dir = "~/file/uploads/" + categoryID;
                    pathAttach = dir + "/" + fileName;

                    if (overRide == 0)
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

                    pathAttach = pathAttach.Substring(2);

                    if (string.IsNullOrEmpty(pathAttachFiles))
                    {
                        pathAttachFiles = pathAttach;
                    }
                    else
                    {
                        pathAttachFiles += ";/" + pathAttach;
                    }

                    file.SaveAs(path);
                }
            }

            return pathAttachFiles;
        }

        public bool Delete(string fileName)
        {
            System.IO.File.Delete(Server.MapPath("~/" + fileName));
            return true;
        }
    }
}
