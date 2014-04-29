using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.MVC.Web.Controllers
{
    public class FileController : Controller
    {
        //
        // GET: /File/
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //public ActionResult Upload(HttpPostedFile fileUpload)
        //{

        //    foreach (String upload in Request.Files)
        //    {
        //        var file = Request.Files[upload];

        //        if (file != null && file.ContentLength > 0)
        //        {
                    
        //        }
        //        else
        //        {
        //            break;
        //        }


        //        String path = AppDomain.CurrentDomain.BaseDirectory + "\\uploads\\";

        //        String fileName = Request.Files[upload].FileName;

        //        String filePath = Path.Combine(path, fileName);


        //        Request.Files[upload].SaveAs(filePath);
        //    }

        //    return RedirectToAction("Index");
        //}
    }
}