using DDxApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;

namespace DDxHub.Controllers
{
    public class DisorderController : Controller
    {
        // GET: DisorderController
        public ActionResult Index(int id)
        {
            DisorderTestsSymptomsModel selectedDisorder = new DisorderTestsSymptomsModel();
            DataRow row = Startup.tblAllDisorders.Select("id = '" + id.ToString() + "'")[0];
            selectedDisorder.disorder = new Disorder()
            {
                Id = id,
                Name = row["Name"].ToString(),
                Description = row["Description"].ToString(),
                ICD9 = row["ICD9"].ToString(),
                ICD10 = row["ICD10"].ToString(),
                ICD11 = row["ICD11"].ToString(),
                Weight = Convert.ToInt32(row["Weight"]),
                Tests = row["Tests"].ToString(),
                Symptoms = row["Symptoms"].ToString()
            };
           
            selectedDisorder.Tests = JsonConvert.DeserializeObject<List<TestGetType>>(selectedDisorder.disorder.Tests);
            selectedDisorder.Symptoms = JsonConvert.DeserializeObject<List<SymptomGetType>>(selectedDisorder.disorder.Symptoms);

            return View(selectedDisorder);
        }

        // GET: DisorderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DisorderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: DisorderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DisorderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DisorderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: DisorderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: DisorderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
