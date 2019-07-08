using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LeaveWebApp.Models;
using Amazon.DynamoDBv2;
using Amazon;
using System.Net.Http;
using Newtonsoft.Json;

using Amazon.XRay.Recorder.Core;
using Amazon.XRay.Recorder.Handlers.System.Net;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using System;

namespace LeaveWebApp.Controllers
{
    public class HomeController : Controller
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient(RegionEndpoint.USEast1);
        private static string APIURL = "APIURL";

        private HttpClient httpClient;

        public HomeController()
        {
            this.httpClient = new HttpClient(new HttpClientXRayTracingHandler(new HttpClientHandler()));
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult LeaveRequest()
        {
            ViewData["SaveStatus"] = null;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LeaveRequest(Leave leave)
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            leave.LeaveStatus = "Pending";
            ViewData["SaveStatus"] = (await this.httpClient.PostAsJsonAsync(APIURL, leave)).IsSuccessStatusCode.ToString();
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> LeavesList()
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            var leavesList = await this.httpClient.GetStringAsync(APIURL);
            var myobject = JsonConvert.DeserializeObject<Leave[]>(leavesList);
            return View(myobject);
        }

        [HttpGet]
        public async Task<IActionResult> Approve(Leave leave)
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            leave.LeaveStatus = "Approved";
            ViewData["SaveStatus"] = (await this.httpClient.PostAsJsonAsync(APIURL, leave)).IsSuccessStatusCode.ToString();
            return View(leave);
        }

        [HttpGet]
        public async Task<IActionResult> Reject(Leave leave)
        {
            AWSSDKHandler.RegisterXRayForAllServices();
            leave.LeaveStatus = "Rejected";
            ViewData["SaveStatus"] = (await this.httpClient.PostAsJsonAsync(APIURL, leave)).IsSuccessStatusCode.ToString();
            return View("Approve", leave);
        }

        [HttpGet]
        public async Task<IActionResult> ParkAside(Leave leave)
        {
            throw new NotImplementedException("Feature is not implemented yet.");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}