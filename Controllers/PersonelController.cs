using FluentValidation.Results;
using OdemeProjesi.Models;
using OdemeProjesi.ValidationRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OdemeProjesi.Controllers
{
    public class PersonelController : Controller
    {
        // GET: Personel
        OdemeProjesiDBEntities2 db = new OdemeProjesiDBEntities2();

        public ActionResult Index()
        {
            return View();
        }

        //BÜTÜN ABONELERİ LİSTELE
        [HttpGet]
        public ActionResult AboneListele()
        {
            List<tblAbone> abone = db.tblAbone.ToList();
            List<tblFatura> fatura = db.tblFatura.ToList();
            var result = abone.ToList();
            return View(result);
        }
        //INPUTA GİRİLEN TCNO'YA GÖRE ABONEYİ LİSTELE
        [HttpPost]
        public ActionResult AboneListele(string p)
        {
            List<tblAbone> abone = db.tblAbone.ToList();
            List<tblFatura> fatura = db.tblFatura.ToList();
            var result = abone.Where(x => x.TCNo == p).ToList();
            return View(result);
        }
        //ABONE EKLE tc-ad-soyad
        [HttpGet]
        public ActionResult AboneEkle()
        {
            return View();
        }
        //post ile gelen bilgilere göre ekleme işlemini yap
        [HttpPost]
        public ActionResult AboneEkle(tblAbone p)
        {
            AboneValidator aboneValidator = new AboneValidator();
            ValidationResult sonuclar = aboneValidator.Validate(p);
            int tcNoTekrar = db.tblAbone.Where(x => x.TCNo == p.TCNo).Count();
            if (tcNoTekrar == 0)
            {
                if (sonuclar.IsValid)
                {
                    tblAbone tb = new tblAbone();
                    tb.AboneAdi = p.AboneAdi;
                    tb.AboneSoyadi = p.AboneSoyadi;
                    tb.TCNo = p.TCNo;
                    db.tblAbone.Add(tb);
                    db.SaveChanges();
                    return RedirectToAction("AboneListele");
                }
                else
                {
                    foreach (var item in sonuclar.Errors) //resulttan gelen errorları döndür
                    {
                        ModelState.AddModelError(item.PropertyName, item.ErrorMessage);
                    }
                }
            }
            else
            {
                ViewBag.TekrarKayit = "Abone zaten kayıtlı!";
            }
            return View();
        }
        //SEÇİLEN ABONENİN FATURALARINI GETİR
        [HttpGet]
        public ActionResult AboneFaturaGetir(int id)
        {
            var sonuclar = db.tblFatura.Where(x => x.AboneId == id).ToList();
            return View(sonuclar);
        }
        //FATURA ÖDEME İŞLEMİ
        [HttpGet]
        public ActionResult FaturaOdeme(int id)
        {
            var guncellenecekFatura = db.tblFatura.Find(id);
            guncellenecekFatura.FaturaDurum = true;
            db.SaveChanges();
            int aboneId = Convert.ToInt32(guncellenecekFatura.tblAbone.AboneId);
            return RedirectToAction("AboneFaturaGetir", new RouteValueDictionary(new { controller = "Personel", action = "AboneFaturaGetir", id = aboneId })); //RouteValueDictionary---using System.Web.Routing
        }
        //ABONE LİSTESİNDEN SEÇİLEN ABONEYİ İPTAL ETME
        [HttpGet]
        public ActionResult AbonelikİptalEt(int id)
        {
            List<tblAbone> abone = db.tblAbone.ToList();
            List<tblFatura> fatura = db.tblFatura.ToList();
            var result = (from abn in abone
                          join ftr in fatura on abn.AboneId equals ftr.AboneId
                          where (abn.AboneId == id && ftr.FaturaDurum == false)
                          select new MultipleClass
                          {
                              abone = abn,
                              fatura = ftr
                          }).Count();
            if (result == 0)//ödenmemiş fatura yok, abonelik iptal
            {
                var iptalAbone = db.tblAbone.Find(id);
                iptalAbone.AboneDurum = false;
                iptalAbone.DepozitoTutari = iptalAbone.DepozitoTutari - iptalAbone.DepozitoTutari;
                db.SaveChanges();
                return RedirectToAction("AboneListele");
            }
            else
            {
                TempData["mesaj"] = "Ödenmemiş fatura mevcut, abonelik iptal edilemez";
                return RedirectToAction("AboneListele");
                //abonelik iptal edilemez
            }
        }
        //FATURA ID'ye FATURA GETİR
        public ActionResult FaturaSorgulamaSayfasi()
        {
            return View();
        }
        //FATURAYI LİSTELE
        [HttpGet]
        public ActionResult FaturaOdemeSayfasi(string p)
        {
            List<tblAbone> abone = db.tblAbone.ToList();
            List<tblFatura> fatura = db.tblFatura.ToList();
            int a = Convert.ToInt32(p);
            var result = from abn in abone
                         join ftr in fatura on abn.AboneId equals ftr.AboneId
                         where ftr.FaturaId == a
                         select new MultipleClass
                         {
                             abone = abn,
                             fatura = ftr
                         };
            result.ToList();
            return View(result);
        }
        //FATURAYI ÖDE VE LİSTELEME SAYFASINA GÖNDER
        [HttpGet]
        public ActionResult FaturaOdemeSayfasiOdendi(int id)
        {
            var guncellenecekFatura = db.tblFatura.Find(id);
            guncellenecekFatura.FaturaDurum = true;
            string p = guncellenecekFatura.FaturaId.ToString();
            db.SaveChanges();
            return RedirectToAction("FaturaOdemeSayfasi", new RouteValueDictionary(new { controller = "Personel", action = "FaturaOdemeSayfasi", p = p }));
        }

    }
}