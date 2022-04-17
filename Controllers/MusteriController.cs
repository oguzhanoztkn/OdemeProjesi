using OdemeProjesi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OdemeProjesi.Controllers
{
    public class MusteriController : Controller
    {
        OdemeProjesiDBEntities2 db = new OdemeProjesiDBEntities2();

        // GET: Musteri
        public ActionResult Index()
        {
            return View();
        }
        //Girilen tcnoya ait faturalar
        public ActionResult MusteriListele(string p)
        {
            List<tblAbone> abone = db.tblAbone.ToList();
            List<tblFatura> fatura = db.tblFatura.ToList();
            var result = from abn in abone
                         join ftr in fatura on abn.AboneId equals ftr.AboneId
                         where abn.TCNo == p
                         select new MultipleClass
                         {
                             abone = abn,
                             fatura = ftr
                         };

            result.ToList();
            return View(result);
        }
        //fatura öde
        public ActionResult FaturaOde(int id)
        {
            var guncellenecekFatura = db.tblFatura.Find(id);
            guncellenecekFatura.FaturaDurum = true;
            db.SaveChanges();
            return RedirectToAction("MusteriListele", new RouteValueDictionary(//using System.Web.Routing
            new { controller = "Musteri", action = "MusteriListele", p = guncellenecekFatura.tblAbone.TCNo }));
        } 
    }
}