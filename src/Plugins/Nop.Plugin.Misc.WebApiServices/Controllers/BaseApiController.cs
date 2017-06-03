using Nop.Core.Domain.Catalog;
using Nop.Core.Infrastructure;
using Nop.Plugin.Api.Attributes;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Nop.Plugin.Misc.WebApiServices.Controllers
{
    [BearerTokenAuthorize]
    public class BaseApiController : ApiController
    {

    }
}
