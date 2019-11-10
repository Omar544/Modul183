using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Modul183.Models;
using System.Data.Entity;
using System.IO;

namespace Modul183.Controllers
{
    public class EmployeeController : Controller
    {
        // Logger wird deklariert
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        //
        // GET: /Employee/
        public ActionResult Index()
        {
            // Loger wird mit dem Config-File verbunden
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/log4net.config")));
            // Wenn User nicht eingeloggt ist, dann ist diese Seite nicht zugäglich
            if (Session["userID"] != null)
            {
                return View();
            }
            else
            {
                log.Fatal("Versuchter Zugriff ohne erlaubter Zugriff !!");
                return RedirectToAction("Index", "Login");
            }
        }

        // Ladet Employee von der Datenbank und speichert diese in eine Liste
        public ActionResult GetData()
        {
            using (DBModel db = new DBModel())
            {
                List<Employee> empList = db.Employee.ToList<Employee>();
                return Json(new { data = empList }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        // Erstellt einen leeren Dialog
        public ActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Employee());
            else
            {
                using (DBModel db = new DBModel())
                {
                    return View(db.Employee.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>());
                }
            }
        }

        [HttpPost]
        // Erstellt einen Dialog mit vorhandenen Daten
        public ActionResult AddOrEdit(Employee emp)
        {
            using (DBModel db = new DBModel())
            {
               
                    if (emp.EmployeeID == 0)
                    {
                        db.Employee.Add(emp);
                        db.SaveChanges();
                        return Json(new { success = true, message = "Saved Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        db.Entry(emp).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(new { success = true, message = "Updated Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                

            }


        }

        [HttpPost]
        // Löscht den ausgewählten Eintrag
        public ActionResult Delete(int id)
        {
            using (DBModel db = new DBModel())
            {
                Employee emp = db.Employee.Where(x => x.EmployeeID == id).FirstOrDefault<Employee>();
                db.Employee.Remove(emp);
                db.SaveChanges();
                log.Info("Eintrag gelöscht !!");
                return Json(new { success = true, message = "Deleted Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}