using DDxHub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using DDxApi.Models;
using Newtonsoft.Json;
using System.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace DDxHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> IndexAsync(string id)
        {
            // Get default Tests and Symptoms
            // It simply assigns the same Test Procedures and Symptoms collections every time when the app started.
            // You can develop your method to store them into browser cookies or your database.
            if (Startup.symptoms.Count == 0 || Startup.tests.Count == 0)
                Startup.GetTestsSymptomsDefault();

            TestsSymptomsModel TestsSymptoms = new TestsSymptomsModel();
            TestsSymptoms.Tests = Startup.tests;
            TestsSymptoms.Symptoms = Startup.symptoms;

            return View(TestsSymptoms);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Get Tests and Symptoms datasets from DiagnosisApi Web API
        public async Task<IActionResult> GetTestsSymptoms()
        {
            if (Startup.tblAllTests.Rows.Count == 0)
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Startup.baseAddress + "GetTests" + Startup.AuthenticationID);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    //HTTP GET Tests
                    HttpResponseMessage response = await client.GetAsync(Startup.baseAddress + "GetTests" + Startup.AuthenticationID);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        Startup.tblAllTests = JsonConvert.DeserializeObject<DataTable>(data);
                        // Populate tests Dropdown List
                        SetTestsDropdownList();
                    }

                    //HTTP GET Panels
                    response = await client.GetAsync(Startup.baseAddress + "GetPanels" + Startup.AuthenticationID);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        string[] strAllPanels = data.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(",");
                        // Populate panels Dropdown List
                        Startup.panelsDropdownList.Add(new PanelsDropdownList() { sid = "0", panel = "ALL" });
                        Startup.panelsDropdownList.Add(new PanelsDropdownList() { sid = "0.1", panel = "CBC - Complete Blood Count" });
                        foreach (string str in strAllPanels)
                        {
                            Startup.panelsDropdownList.Add(new PanelsDropdownList()
                            {
                                sid = str.ToString().Substring(0, 6).Trim(),
                                panel = str.ToString().Substring(6).Trim()
                            });
                        }
                    }

                    //HTTP GET Symptoms
                    response = await client.GetAsync(Startup.baseAddress + "GetSymptoms" + Startup.AuthenticationID);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        Startup.tblAllSymptoms = JsonConvert.DeserializeObject<DataTable>(data);
                        // Populate symptoms Dropdown List
                        SetSymptomsDropdownList();
                    }

                    //HTTP GET Categories
                    response = await client.GetAsync(Startup.baseAddress + "GetCategories" + Startup.AuthenticationID);
                    if (response.IsSuccessStatusCode)
                    {
                        var data = await response.Content.ReadAsStringAsync();
                        string[] strAllCategories = data.ToString().Replace("[", "").Replace("]", "").Replace("\"", "").Split(",");
                        // Populate categories Dropdown List
                        Startup.categoriesDropdownList.Add(new SymptomCategoriesDropdownList() { sid = "0", category = "ALL" });
                        int i = 1;
                        foreach (string str in strAllCategories)
                        {
                            Startup.categoriesDropdownList.Add(new SymptomCategoriesDropdownList()
                            {
                                sid = i.ToString(),
                                category = str.ToString().Trim()
                            });
                            i++;
                        }
                    }

                }
            return RedirectToAction(nameof(IndexAsync));
        }

        [HttpGet]
        public int TestsSymptomsGet()
        {
            // Get Tests and Symptoms datasets from DiagnosisApi Web API
            GetTestsSymptoms();

            // Show progress loader at start up only
            if (Startup.started)
                return 1;
            else
            {
                for (var i = 1; i <= 90; i++)
                    if (Startup.tblAllTests.Rows.Count <= 0 || Startup.tblAllSymptoms.Rows.Count <= 0)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    else
                    {
                        Startup.started = true;
                        return 0;
                    }
                // Break after 90 seconds attempting to retrieve data from DiagnosisApi Web API
                return 2;
            }
        }

        // GET: tests/AddTest
        public ActionResult AddTest()
        {
            SetTestsDropdownList();

            return View();
        }

        // POST: tests/AddTest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTest([Bind("ID", "Value", "LowRangeValue", "HighRangeValue", "ReferenceType")] TestGetType test)
        {
            if (ModelState.IsValid)
            {
                if (test.ID <= 0)
                {
                    ViewBag.Message = "Please Enter a Test Procedure Name";
                    return View();
                }
                if ((test.Value ?? "") == string.Empty)
                {
                    ViewBag.Message = "Please Enter a Value";
                    return View();
                }
                var tst = Startup.tests.FindIndex(t => t.ID == test.ID);
                if (tst != -1)
                {
                    var procedure = Startup.tblAllTests.Select("id = '" + test.ID.ToString() + "'")[0][1].ToString();
                    ViewBag.Message = "The " + procedure + " test is already added to the list of your Tests!";
                }
                else
                {
                    TestPostType testPost = new TestPostType(test.ID, test.Value, test.LowRangeValue, test.HighRangeValue, test.ReferenceType);
                    Startup.tests.Add(testPost);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        // GET: tests/Delete/5
        public ActionResult DeleteTest(int? id)
        {
            if (id != null)
            {
                var tst = Startup.tests.FindIndex(t => t.ID == id);
                if (tst != -1)
                    Startup.tests.RemoveAt(tst);
            }
            return RedirectToAction("Index");
        }

        // GET: tests/EditTest/5
        public ActionResult EditTest(int? id)
        {
            if (id == null)
            {
                return View();
            }
            TestPostType test = Startup.tests.Find(t => t.ID == id);
            if (test != null)
            {
                Startup.testEditStartID = id;
                DataRow row = Startup.tblAllTests.Select("id = '" + id.ToString() + "'")[0];
                TestGetType testGet = new TestGetType();
                testGet.ID = test.ID;
                testGet.Value = test.Value;
                testGet.Status = "0";
                testGet.LowRangeValue = test.LowRangeValue;
                testGet.HighRangeValue = test.HighRangeValue;
                testGet.ComboValuesList = row["comboValuesList"].ToString();
                testGet.NegativeComboValue = row["negativeComboValue"].ToString();
                testGet.Comment = row["descriptionHigh"].ToString();
                testGet.Entered = true;
                testGet.ReferenceType = test.ReferenceType;
                string units = row["unitsType"].ToString();
                string untSI = row["unitsTypeSI"].ToString();
                if ("boolean;combo;value".IndexOf(units.ToLower()) < 0)
                {
                    units = "Units: " + units;
                    untSI = "Units: " + untSI;
                    if (testGet.Value == null)
                        testGet.Value = "0";
                    if (isNumeric(testGet.Value))
                    {
                        double dblValue = Convert.ToDouble(test.Value);
                        if (dblValue < Convert.ToDouble(test.LowRangeValue))
                            testGet.Status = "-1";
                        if (dblValue > Convert.ToDouble(test.HighRangeValue))
                            testGet.Status = "1";
                    }
                    else
                        testGet.Value = "0";
                    if (units.Trim().ToLower() != untSI.Trim().ToLower())
                    {
                        testGet.UnitsTypeSI = untSI;
                        testGet.LowRangeValueSI = row["LowRangeValue"].ToString() + ";" + row["HighRangeValue"].ToString();
                        testGet.HighRangeValueSI = row["LowRangeValueSI"].ToString() + ";" + row["HighRangeValueSI"].ToString();
                    }
                }
                if ("boolean".IndexOf(units.ToLower()) == 0)
                {
                    if (testGet.Value == null)
                        testGet.Value = "Negative";
                    if (testGet.Value.ToLower() == "positive")
                        testGet.Status = "1";
                }
                if ("combo".IndexOf(units.ToLower()) == 0)
                {
                    if (testGet.Value == null)
                        testGet.Value = testGet.NegativeComboValue;
                    if (testGet.Value.ToLower() != testGet.NegativeComboValue.ToLower() && testGet.Value.Trim() != string.Empty)
                        testGet.Status = "1";
                }
                testGet.UnitsType = units;
                if (testGet.Status == "-1")
                    testGet.Comment = row["descriptionLow"].ToString();
                if (id == 114) // Exception for ABO grouping; 'Value' unitType
                {
                    testGet.UnitsType = "Combo";
                    testGet.NegativeComboValue = "O";
                    testGet.ComboValuesList = "A;B;AB";
                }
                SetTestsDropdownList();

                return View(testGet);
            }
            return View();
        }

        // POST: tests/EditTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTest([Bind("ID", "Value", "LowRangeValue", "HighRangeValue", "ReferenceType")] TestGetType test)
        {
            if (ModelState.IsValid)
            {
                if (test.ID <= 0)
                {
                    ViewBag.Message = "Please Enter a Test Procedure Name";
                    return View();
                }
                if ((test.Value ?? "") == string.Empty)
                {
                    ViewBag.Message = "Please Enter a Value";
                    return View();
                }
                var ts_ = Startup.tests.FindIndex(t => t.ID == test.ID && t.ID != Startup.testEditStartID);
                if (ts_ != -1)
                {
                    var procedure = Startup.tblAllTests.Select("id = '" + test.ID.ToString() + "'")[0][1].ToString();
                    ViewBag.Message = "The " + procedure + " test is already added to the list of your Tests!";
                }
                else
                {
                    TestPostType tst = Startup.tests.Find(t => t.ID == Startup.testEditStartID);
                    if (test != null)
                    {
                        Startup.tests.Find(t => t.ID == Startup.testEditStartID).Value = test.Value;
                        Startup.tests.Find(t => t.ID == Startup.testEditStartID).LowRangeValue = test.LowRangeValue;
                        Startup.tests.Find(t => t.ID == Startup.testEditStartID).HighRangeValue = test.HighRangeValue;
                        Startup.tests.Find(t => t.ID == Startup.testEditStartID).ReferenceType = test.ReferenceType;
                        if (Startup.testEditStartID != test.ID)
                            Startup.tests.Find(t => t.ID == Startup.testEditStartID).ID = test.ID;

                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult TestChangeGet(TestGetType test)
        {
            if (ModelState.IsValid)
            {
                if (test.ID <= 0 && Startup.symptomEditStartID > 0)
                    test.ID = (int)Startup.testEditStartID;

                if (test.ID == 0)
                    return View();

                var procedure = Startup.tblAllTests.Select("id = '" + test.ID.ToString() + "'")[0][1].ToString();
                if ((procedure ?? "") != "")
                {
                    var model = new TestGetType();
                    DataRow row = Startup.tblAllTests.Select("id = '" + test.ID.ToString() + "'")[0];
                    model.ID = test.ID;
                    model.Value = test.Value;
                    model.LowRangeValue = row["lowRangeValue"].ToString();
                    model.HighRangeValue = row["highRangeValue"].ToString();
                    model.ComboValuesList = row["comboValuesList"].ToString();
                    model.NegativeComboValue = row["negativeComboValue"].ToString();
                    model.Comment = row["descriptionHigh"].ToString();
                    model.Entered = true;
                    model.ReferenceType = test.ReferenceType;
                    model.Status = "0";
                    string units = row["unitsType"].ToString();
                    string untSI = row["unitsTypeSI"].ToString();
                    if ("boolean;combo;value".IndexOf(units.ToLower()) < 0)
                    {
                        units = "Units: " + units;
                        untSI = "Units: " + untSI;
                        if (model.Value == null)
                            model.Value = "0";
                        if (isNumeric(test.Value))
                        {
                            double dblValue = Convert.ToDouble(model.Value);
                            if (dblValue < Convert.ToDouble(model.LowRangeValue))
                                model.Status = "-1";
                            if (dblValue > Convert.ToDouble(model.HighRangeValue))
                                model.Status = "1";
                        }
                        else
                            model.Value = "0";
                        if (units.Trim().ToLower() != untSI.Trim().ToLower())
                        {
                            model.UnitsTypeSI = untSI;
                            model.LowRangeValueSI = row["LowRangeValue"].ToString() + ";" + row["HighRangeValue"].ToString();
                            model.HighRangeValueSI = row["LowRangeValueSI"].ToString() + ";" + row["HighRangeValueSI"].ToString();
                        }
                    }
                    if ("boolean".IndexOf(units.ToLower()) == 0)
                    {
                        if (model.Value == null)
                            model.Value = "Negative";
                        if (model.Value.ToLower() == "positive")
                            model.Status = "1";
                    }
                    if ("combo".IndexOf(units.ToLower()) == 0)
                    {
                        if (model.Value == null)
                            model.Value = model.NegativeComboValue;
                        if (model.Value.ToLower() != model.NegativeComboValue.ToLower() && model.Value.Trim() != string.Empty)
                            model.Status = "1";
                    }
                    model.UnitsType = units;
                    if (model.Status == "-1")
                        model.Comment = row["descriptionLow"].ToString();

                    // Check if the test is already added to the list of Tests
                    var ts_ = Startup.tests.FindIndex(t => t.ID == test.ID && t.ID != Startup.testEditStartID);
                    if (ts_ != -1)
                    {
                        model.ID = (int)Startup.testEditStartID;
                        model.Comment = "The " + procedure + " test is already added to the list of your Tests!";
                        TestPostType testPrior = Startup.tests.Find(t => t.ID == Startup.testEditStartID);
                        if (test != null && testPrior != null)
                        {
                            row = Startup.tblAllTests.Select("id = '" + testPrior.ID.ToString() + "'")[0];
                            model.ID = testPrior.ID;
                            model.Value = testPrior.Value;
                            units = row["unitsType"].ToString();
                            untSI = row["unitsTypeSI"].ToString();
                            model.UnitsType = units;
                            model.UnitsTypeSI = "";
                            model.LowRangeValue = testPrior.LowRangeValue;
                            model.HighRangeValue = testPrior.HighRangeValue;
                            model.ComboValuesList = row["comboValuesList"].ToString();
                            model.NegativeComboValue = row["negativeComboValue"].ToString();
                            model.Comment = row["descriptionHigh"].ToString();
                            model.Entered = true;
                            model.ReferenceType = testPrior.ReferenceType;
                            model.Status = "0";
                            if ("boolean;combo;value".IndexOf(units.ToLower()) < 0)
                            {
                                units = "Units: " + units;
                                untSI = "Units: " + untSI;
                                if (isNumeric(test.Value))
                                {
                                    double dblValue = Convert.ToDouble(test.Value);
                                    if (dblValue < Convert.ToDouble(model.LowRangeValue))
                                        model.Status = "-1";
                                    if (dblValue > Convert.ToDouble(model.HighRangeValue))
                                        model.Status = "1";
                                }
                                else
                                    test.Value = "0";
                                if (units.Trim().ToLower() != untSI.Trim().ToLower())
                                {
                                    model.UnitsTypeSI = untSI;
                                    model.LowRangeValueSI = row["LowRangeValue"].ToString() + ";" + row["HighRangeValue"].ToString();
                                    model.HighRangeValueSI = row["LowRangeValueSI"].ToString() + ";" + row["HighRangeValueSI"].ToString();
                                }
                            }
                            if ("boolean".IndexOf(units.ToLower()) == 0)
                                if (model.Value.ToLower() == "positive")
                                    model.Status = "1";
                            if ("combo".IndexOf(units.ToLower()) == 0)
                                if (model.Value.ToLower() != model.NegativeComboValue.ToLower() && model.Value.Trim() != string.Empty)
                                    model.Status = "1";
                            model.UnitsType = units;
                            if (model.Status == "-1")
                                model.Comment = row["descriptionLow"].ToString();
                        }
                    }

                    if (model.ID == 114) // Exception for ABO grouping; 'Value' unitType
                    {
                        model.UnitsType = "Combo";
                        model.NegativeComboValue = "O";
                        model.ComboValuesList = "A;B;AB";
                    }
                    return Json(model);
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult SymptomChangeGet(int? symptomID)
        {
            if (ModelState.IsValid)
            {
                if ((symptomID ?? 0) <= 0 && Startup.symptomEditStartID > 0)
                    symptomID = Startup.symptomEditStartID;

                if ((symptomID ?? 0) == 0)
                    return View();

                var symptomname = Startup.tblAllSymptoms.Select("id = '" + symptomID.ToString() + "'")[0]["symptom"].ToString();
                if ((symptomname ?? "") != "")
                {
                    var model = new SymptomGetType();
                    DataRow row = Startup.tblAllSymptoms.Select("id = '" + symptomID.ToString() + "'")[0];
                    model.ID = (int)symptomID;
                    model.SimilarSymptomsList = row["similarSymptomsList"].ToString();
                    model.Entered = true;
                    model.Category = row["category"].ToString();

                    // Check if the symptom is already added to the list of Symptoms
                    var ts_ = Startup.symptoms.FindIndex(t => t.ID == symptomID && t.ID != Startup.symptomEditStartID);
                    if (ts_ != -1)
                    {
                        model.ID = (int)Startup.symptomEditStartID;
                        model.SimilarSymptomsList = "The '" + symptomname + "' symptom is already added to the list of your Symptoms!";
                        SymptomPostType symptomPrior = Startup.symptoms.Find(t => t.ID == Startup.symptomEditStartID);
                        if ((symptomID ?? 0) > 0 && symptomPrior != null)
                        {
                            row = Startup.tblAllSymptoms.Select("id = '" + symptomPrior.ID.ToString() + "'")[0];
                            model.ID = symptomPrior.ID;
                            model.SimilarSymptomsList += row["similarSymptomsList"].ToString();
                            model.Entered = true;
                            model.Category = row["category"].ToString();
                        }
                    }
                    return Json(model);
                }
            }
            return View();
        }

        public IActionResult PanelChangeGet(string sid)
        {
            if (ModelState.IsValid)
            {
                string comboValuesList = "";
                // Populate tests Dropdown List
                SetTestsDropdownList(sid);
                foreach (var tst in Startup.testsDropdownList)
                    comboValuesList += tst.id + "|" + tst.procedure + ";";

                return Json(comboValuesList);
            }
            return View();
        }

        public IActionResult CategoryChangeGet(string category)
        {
            if (ModelState.IsValid)
            {
                string comboValuesList = "";
                // Populate tests Dropdown List
                SetSymptomsDropdownList(category);
                foreach (var ctg in Startup.symptomsDropdownList)
                    comboValuesList += ctg.id + "|" + ctg.symptom + ";";

                return Json(comboValuesList);
            }
            return View();
        }

        // GET: symptoms/AddSymptom
        public ActionResult AddSymptom()
        {
            SetSymptomsDropdownList();

            return View();
        }

        // POST: symptoms/AddSymptom
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSymptom([Bind("ID", "Symptom", "Entered", "SimilarSymptomsList", "Category")] SymptomGetType symptom)
        {
            if (ModelState.IsValid)
            {
                if (symptom.ID <= 0)
                {
                    ViewBag.Message = "Please Enter a Symptom";
                    return View();
                }
                var tst = Startup.symptoms.FindIndex(t => t.ID == symptom.ID);
                if (tst != -1)
                {
                    var symptomname = Startup.tblAllSymptoms.Select("id = '" + symptom.ID.ToString() + "'")[0][1].ToString();
                    ViewBag.Message = "The '" + symptomname + "' symptom is already added to the list of your Symptoms!";
                }
                else
                {
                    SymptomPostType symptomPost = new SymptomPostType(symptom.ID);
                    Startup.symptoms.Add(symptomPost);
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        // GET: symptoms/Delete/5
        public ActionResult DeleteSymptom(int? id)
        {
            if (id != null)
            {
                var smt = Startup.symptoms.FindIndex(t => t.ID == id);
                if (smt != -1)
                    Startup.symptoms.RemoveAt(smt);
            }
            return RedirectToAction("Index");
        }

        // GET: symptoms/EditSymptom/5
        public ActionResult EditSymptom(int? id)
        {
            if (id == null)
            {
                return View();
            }
            SymptomPostType symptom = Startup.symptoms.Find(t => t.ID == id);
            if (symptom != null)
            {
                Startup.symptomEditStartID = id;
                DataRow row = Startup.tblAllSymptoms.Select("id = '" + id.ToString() + "'")[0];
                SymptomGetType symptomGet = new SymptomGetType();
                symptomGet.ID = symptom.ID;
                symptomGet.Symptom = row["symptom"].ToString();
                symptomGet.Entered = true;
                symptomGet.SimilarSymptomsList = row["similarSymptomsList"].ToString();
                symptomGet.Category = row["category"].ToString();
                SetSymptomsDropdownList();

                return View(symptomGet);
            }
            return View();
        }

        // POST: symptoms/EditSymptom/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSymptom([Bind("ID", "Symptom", "Entered", "SimilarSymptomsList", "Category")] SymptomGetType symptom)
        {
            if (symptom.ID <= 0)
            {
                ViewBag.Message = "Please First Select a Symptom";
                return View();
            }

            if (ModelState.IsValid && symptom.ID > 0)
            {
                if (symptom.ID <= 0 && Startup.symptomEditStartID > 0)
                    symptom.ID = (int)Startup.symptomEditStartID;

                var ts_ = Startup.symptoms.FindIndex(t => t.ID == symptom.ID && t.ID != Startup.symptomEditStartID);
                if (ts_ != -1)
                {
                    DataRow row = Startup.tblAllSymptoms.Select("id = '" + symptom.ID.ToString() + "'")[0];
                    ViewBag.Message = "The " + row["symptom"].ToString() + " symptom is already added to the list of your Symptoms!";
                }
                else
                {
                    SymptomPostType tst = Startup.symptoms.Find(t => t.ID == Startup.symptomEditStartID);
                    if (symptom != null)
                    {
                        if (Startup.symptomEditStartID != symptom.ID)
                            Startup.symptoms.Find(t => t.ID == Startup.symptomEditStartID).ID = symptom.ID;

                        return RedirectToAction("Index");
                    }
                }
            }
            return View();
        }

        // Populate tests Dropdown List
        private void SetTestsDropdownList(string sid = "0")
        {
            Startup.testsDropdownList.Clear();
            foreach (DataRow row in Startup.tblAllTests.Rows)
            {
                int id = Convert.ToInt32(row["id"].ToString().Trim());
                if (sid == "0" || row["sid"].ToString().Trim().IndexOf(sid) == 0 ||
                   (sid == "0.1" && (id == 16 || (id > 7 && id < 12)))) // CBC exception IDs: 8, 9, 10, 11, 16
                {
                    bool exist = false;
                    foreach (var tst in Startup.testsDropdownList)
                        if (tst.id == row["id"].ToString().Trim() || tst.procedure == row["procedure"].ToString().Trim())
                        {
                            exist = true;
                            break;
                        }

                    if (!exist)
                        Startup.testsDropdownList.Add(new TestsDropdownList()
                        {
                            id = row["id"].ToString().Trim(),
                            procedure = row["procedure"].ToString().Trim()
                        });
                }
            }

            if (sid == "0" && Startup.testsDropdownListAll.Count == 0)
            {
                Startup.testsDropdownListAll = Startup.testsDropdownList;
                Startup.testsDropdownListAll = Startup.testsDropdownListAll.OrderBy(o => o.procedure).ToList();
            }
            else
            {
                Startup.testsDropdownList = Startup.testsDropdownList.OrderBy(o => o.procedure).ToList();
            }
        }

        // Populate symptoms Dropdown List
        private void SetSymptomsDropdownList(string category = "ALL")
        {
            Startup.symptomsDropdownList.Clear();
            foreach (DataRow row in Startup.tblAllSymptoms.Rows)
            {
                if (category == "ALL" || row["category"].ToString().Trim().IndexOf(category) == 0)
                {
                    bool exist = false;
                    foreach (var spt in Startup.symptomsDropdownList)
                        if (spt.symptom == row["symptom"].ToString().Trim())
                        {
                            exist = true;
                            break;
                        }

                    if (!exist)
                    {
                        string strId = row["id"].ToString().Trim();
                        Startup.symptomsDropdownList.Add(new SymptomsDropdownList()
                        {
                            id = strId,
                            symptom = row["symptom"].ToString().Trim()
                        });
                    }
                }
            }

            if (category == "ALL" && Startup.symptomsDropdownListAll.Count == 0)
            {
                Startup.symptomsDropdownListAll = Startup.symptomsDropdownList;
                Startup.symptomsDropdownListAll = Startup.symptomsDropdownListAll.OrderBy(o => o.symptom).ToList();
            }
            else
            {
                Startup.symptomsDropdownList = Startup.symptomsDropdownList.OrderBy(o => o.symptom).ToList();
            }
        }

        public bool isNumeric(string numString)
        {
            decimal number3 = 0;
            return decimal.TryParse(numString, out number3);
        }
    }
}
