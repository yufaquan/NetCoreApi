using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace NetCoreAPI.Controllers.Management
{
    [Route("management/[controller]/[action]")]
    [ApiController]
    public class ManagementApiController : ControllerBase
    {
    }
}
