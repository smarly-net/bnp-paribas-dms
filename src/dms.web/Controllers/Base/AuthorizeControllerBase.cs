using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DMS.Web.Controllers.Base;

[Authorize]
[ApiController]
public class AuthorizeControllerBase : ControllerBase
{

}