using DDxApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Data;

namespace DDxHub
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // Url to Diagnosis Web API
        public static string baseAddress = "https://diagnosisapi.azurewebsites.net/api/DDxItems/";

        // Your registration AuthenticationID you need to provide to GET and POST to Diagnosis API
        // For Demo development AuthenticationID = DEMO_AuthenticationID
        public static string AuthenticationID = "?AuthenticationID=DEMO_AuthenticationID";
        
        // List of all UI entered Test Procedures
        public static List<TestPostType> tests = new List<TestPostType>();

        // List of all UI entered Symptoms
        public static List<SymptomPostType> symptoms = new List<SymptomPostType>();

        // List of all Test Procedures retrieved from Diagnosis API 
        public static DataTable tblAllTests = new DataTable();

        // List of all Symptoms retrieved from Diagnosis API 
        public static DataTable tblAllSymptoms = new DataTable();

        // List of all Disorders based on Test Procedures and Symptoms retrieved from Diagnosis API 
        public static DataTable tblAllDisorders = new DataTable();

        // User Interface dropdown lists
        public static IList<TestsDropdownList> testsDropdownList = new List<TestsDropdownList>();
        public static IList<TestsDropdownList> testsDropdownListAll = new List<TestsDropdownList>();
        public static IList<PanelsDropdownList> panelsDropdownList = new List<PanelsDropdownList>();
        public static IList<SymptomsDropdownList> symptomsDropdownList = new List<SymptomsDropdownList>();
        public static IList<SymptomsDropdownList> symptomsDropdownListAll = new List<SymptomsDropdownList>();
        public static IList<SymptomCategoriesDropdownList> categoriesDropdownList = new List<SymptomCategoriesDropdownList>();

        // Some interface related public variables
        public static int? testEditStartID = 0;
        public static int? symptomEditStartID = 0;
        public static bool started = false;
        public static bool itemDeleted = false;
        public static bool itemAdded = false;
        public static bool itemGotten = false;

        // Create a unique number as a client's fingerprinting identifier
        public static string uniqueMark = getUniqeMark();

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Create a unique number as a client's fingerprinting identifier
        static string getUniqeMark() {
            Random rnd = new Random();
            return rnd.Next(1000, 9999).ToString();
        }

        // Get default Tests and Symptoms
        public static void GetTestsSymptomsDefault()
        {
            Startup.tests = new List<TestPostType>();
            //// Endocarditis case
            //tests.Add(new TestPostType(8, "46", "4", "10", 0));  //WBC
            //tests.Add(new TestPostType(9, "2.2", "4.6", "6.1", 0)); //RBC
            //tests.Add(new TestPostType(10, "6.6", "13.7", "17.5", 0)); //Hemoglobin
            //tests.Add(new TestPostType(35, "Present", "0", "0", 0)); //Anisocytosis
            //tests.Add(new TestPostType(11, "21", "40", "51", 0)); //Hematocrit
            //tests.Add(new TestPostType(24, "9.9", "1.8", "7.7", 0)); //Neutrophil Absolute
            //tests.Add(new TestPostType(29, "33", "4", "18", 0)); //ESR - Sed Rate

            // Liver problems case
            tests.Add(new TestPostType(43, "66", "8", "40", 0));  //ALT
            tests.Add(new TestPostType(44, "110", "23", "85", 0));  //Amalyse
            tests.Add(new TestPostType(45, "1", "10", "45", 0));  //AST
            tests.Add(new TestPostType(47, "22", "0.2", "1", 0));  //Total Bilirubin
            tests.Add(new TestPostType(132, "Negative", "0", "0", 0));  //GT Chlamydia
            tests.Add(new TestPostType(175, "Cloudy", "0", "0", 0));  //Appearance Urine
            tests.Add(new TestPostType(114, "B", "0", "0", 0));  //ABO grouping

            Startup.symptoms = new List<SymptomPostType>();
            //// Endocarditis case
            //symptoms.Add(new SymptomPostType(124)); //"sweating"
            //symptoms.Add(new SymptomPostType(21));  //"proteinuria"
            //symptoms.Add(new SymptomPostType(20));  //"blood in urine"
            //symptoms.Add(new SymptomPostType(123)); //"chills"
            //symptoms.Add(new SymptomPostType(164)); //"pyuria"

            // Liver problems case
            symptoms.Add(new SymptomPostType(164)); //"strong smell urine"
            symptoms.Add(new SymptomPostType(151)); //"frequent urination"
            symptoms.Add(new SymptomPostType(200)); //"yellow skin"
        }
    }
}

