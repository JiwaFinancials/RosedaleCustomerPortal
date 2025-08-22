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
            LicenceRenewGETRequest licenceRenewGETRequest = new LicenceRenewGETRequest() { CompanyName = CompanyName, Version = Version };
            List<JiwaFinancials.Jiwa.JiwaServiceModel.Licencing.Licence> licences = await JiwaAPI.GetAsync(licenceRenewGETRequest, jiwaAPIKey: Config.JiwaAPIKey);

            return Ok(licences);            
        }
    }
}
