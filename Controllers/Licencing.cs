using JiwaFinancials.Jiwa.JiwaServiceModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace JiwaCustomerPortal.Controllers
{
    [Route("downloadlicences")]
    [ApiController]
    public class Licencing : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Getdownloadlicences(string CompanyName, string Version)
        {
            //var data = new { Name = "Blazor", Framework = "ASP.NET Core", Version = "9.0" };
            //return Ok(data); // Returns JSON

            LicenceRenewGETRequest licenceRenewGETRequest = new LicenceRenewGETRequest() { CompanyName = CompanyName, Version = Version };
            List<JiwaFinancials.Jiwa.JiwaServiceModel.Licencing.Licence> licences = await JiwaAPI.GetAsync(licenceRenewGETRequest, jiwaAPIKey: Config.JiwaAPIKey);

            return Ok(licences);
            //try
            //{
            //    salesOrder = await JiwaAPI.GetAsync(salesOrderGETRequest, WebPortalUserSessionStateContainer.WebPortalUserSession.Id, null);
            //    SelectedHistory = salesOrder.Histories[0];

            //    if (QueryHelpers.ParseQuery(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).Query).TryGetValue("SnapshotNo", out var snapshotNoParam))
            //    {
            //        short snapno = short.Parse(snapshotNoParam);

            //        if (snapno <= salesOrder.Histories.Count)
            //        {
            //            SelectedHistory = salesOrder.Histories[snapno - 1];
            //        }
            //    }
            //}
        }
    }
}
