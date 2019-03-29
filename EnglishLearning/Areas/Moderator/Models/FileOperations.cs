using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace EnglishLearning.Areas.Moderator.Models
{
    public static class FileOperations
    {

        public static string SaveFile(HttpPostedFileBase file, HttpServerUtilityBase server, string pathToSave, params string[] extensions)
        {
            var path = "";
            if (file != null && file.ContentLength > 0)
            {
                var extension = Path.GetExtension(file.FileName);
                if(extensions.Contains(extension))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    fileName = Guid.NewGuid().ToString() + extension;
                    path = pathToSave;
                    var tempPath = Path.Combine(server.MapPath(path), fileName);
                    path = path + fileName;
                    file.SaveAs(tempPath);
                }
            }
            return path;
        }

        public static bool DeleteIfExist(HttpServerUtilityBase server, string filePath) {
            if (!string.IsNullOrWhiteSpace(filePath))
            {
                if (File.Exists(server.MapPath(filePath)))
                {
                    File.Delete(server.MapPath(filePath));
                    return true;
                }
            }
            return false;
        }

    }
}