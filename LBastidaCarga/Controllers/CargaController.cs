using BL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace LBastidaCarga.Controllers
{
    public class CargaController : Controller
    {
        // GET: Carga
        public ActionResult Cargar()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Cargar(HttpPostedFileBase file)
        {
            file = Request.Files["CSV"];
            if (file != null && file.ContentLength > 0)
                try
                {

                    string extension = Path.GetExtension(file.FileName);
                    
                    string path = Path.Combine(Server.MapPath("~/"), Path.GetFileName(file.FileName));

                    BL.Producto.Carga(path);

                    
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();

           
        }


    }
}
