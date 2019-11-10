using Modul183.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace Modul183.Controllers
{
    public class LoginController : Controller
    {
        // Logger wird deklariert
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /Login/
        public ActionResult Index()
        {
            // Loger wird mit dem Config-File verbunden
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/log4net.config")));
            return View();
        }

        [HttpPost]
        // Eingegebene Daten werden mit der Datenbank geprüft
        public ActionResult Autherize(Modul183.Models.User userModel)
        {
            using (DBModel db = new DBModel())
            {
                var userDetails = db.User.Where(x => x.UserName == userModel.UserName && x.Password == userModel.Password).FirstOrDefault();
                if (userDetails == null)
                {
                    log.Warn("Gescheiterter Login!!");
                    userModel.LoginErrorMessage = "Falscher Username oder Passwort";
                    return View("Index", userModel);
                }
                else
                {
                    log.Info("Erfolgreicher Login");
                    Session["userID"] = userDetails.UserID;
                    Session["userName"] = userDetails.UserName;
                    return RedirectToAction("Index", "Employee");
                }
            }
        }

        // Session wird geschlossen und man wird auf die Logginseite geschickt
        public ActionResult LogOut()
        {
            int userId = (int)Session["userID"];
            log.Info(Session["userName"] + " hat sich ausgeloggt");
            Session.Abandon();
            return RedirectToAction("Index", "Login");
        }

    }
}