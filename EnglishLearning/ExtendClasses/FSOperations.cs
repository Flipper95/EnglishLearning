using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EnglishLearning.ExtendClasses
{
    public class FSOperations
    {

        public string SaveFile(HttpPostedFileBase file, string extension, string directory, HttpServerUtilityBase server)
        {
            var path = "";
            if (file != null && file.ContentLength > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                fileName = Guid.NewGuid().ToString() + extension;
                path = directory;
                var tempPath = Path.Combine(server.MapPath(path), fileName);
                path = path + fileName;
                file.SaveAs(tempPath);
            }
            return path;
        }

        public bool CheckExtension(string[] extensions, string realExtension) {
            return true;
        }

        public bool DeleteFile(string path, HttpServerUtilityBase server) {
            if (!string.IsNullOrWhiteSpace(path))
            {
                if (System.IO.File.Exists(server.MapPath(path)))
                    System.IO.File.Delete(server.MapPath(path));
            }
            return true;
        }

    }
}