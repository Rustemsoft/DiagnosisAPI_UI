using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using DDxApi.Models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using System.Net.Http.Json;
using System.Data;

namespace DDxHub.Controllers
{
    public class AnalyzeController : Controller
    {
        // GET: AnalyzeController1
        public ActionResult Index()
        {
            Startup.itemAdded = false;
            Startup.itemGotten = false;
            
            // Create a unique number as a client's fingerprinting identifier.
            // If you decide to use a constant like itemID = "1", the data 
            // entered by your different clients can be intercepted and mixed.
            string itemID = Startup.uniqueMark; 
            AddItem(itemID);

            // To make async operation as a regular sync - wait for 45 seconds when new data item added to API
            for (var i = 1; i <= 90; i++)
                if (Startup.itemAdded)
                {
                    GetItem(itemID);
                    break;
                }
                else
                    System.Threading.Thread.Sleep(500);
            // Break after 45 seconds attempting to add data item to DiagnosisApi Web API
            if (!Startup.itemAdded)
            {
                ViewBag.Message = "Something went wrong. Please Try Again Later.";
                return View();
            }

            // To make async operation as a regular sync - wait for 45 seconds when new Disorders calculated
            for (var i = 1; i <= 90; i++)
                if (Startup.itemGotten)
                {
                    List<Disorder> Diss = new List<Disorder>();
                    foreach (DataRow row in Startup.tblAllDisorders.Rows)
                    {
                        Disorder dis = new Disorder();
                        dis.Id = Convert.ToInt32(row["id"]);
                        dis.Name = row["name"].ToString();
                        Diss.Add(dis);
                    }
                    if (Diss.Count <= 0)
                        ViewBag.Message = "Not enough data entered to make analysis...";
                    return View(Diss);
                }
                else
                    System.Threading.Thread.Sleep(500);
            // Break after 45 seconds attempting to get Disorders from DiagnosisApi Web API
            if (!Startup.itemGotten)
                ViewBag.Message = "Something went wrong. Please Try Again Later.";

            return View();
        }

        public async void AddItem(string itemID)
        {
            Startup.itemAdded = false;
            var strTests = JsonConvert.SerializeObject(Startup.tests);

            var strSymptoms = JsonConvert.SerializeObject(Startup.symptoms);

            var item = new DDxItem()
            {
                Id = itemID,
                Tests = strTests,
                Symptoms = strSymptoms
            };

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Startup.baseAddress + Startup.AuthenticationID);
                //HTTP POST
                var response = await client.PostAsJsonAsync<DDxItem>("", item);
                if (response.IsSuccessStatusCode)
                    Startup.itemAdded = true;
            }
        }

        public async void GetItem(string itemID)
        {
            Startup.itemGotten = false;
            using (HttpClient client = new HttpClient())
            {
                // Build Url Base Address with AuthenticationID
                client.BaseAddress = new Uri(Startup.baseAddress + itemID + Startup.AuthenticationID);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                //HTTP GET
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    Startup.tblAllDisorders = JsonConvert.DeserializeObject<DataTable>(data);
                    Startup.itemGotten = true;
                }
            }
        }

        // GET: AnalyzeController1/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AnalyzeController1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AnalyzeController1/Create
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

        // GET: AnalyzeController1/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AnalyzeController1/Edit/5
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

        // GET: AnalyzeController1/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AnalyzeController1/Delete/5
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
