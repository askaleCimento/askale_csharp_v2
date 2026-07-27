using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginLogsController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public LoginLogsController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getAllLogs
        [HttpPost("getAllLogs")]

        public ActionResult<PageReturn<LoginLogFilterDto>> listAll([FromForm] FilterPageParam<LoginLogDtoRequest> filterPageParam)
        {
            BLLActions.LoginLogs bllLoginLogs = new BLLActions.LoginLogs(_configuration, _env);
            PageReturn<LoginLogFilterDto>? liste = bllLoginLogs.FilterPageableDto(filterPageParam);
            return Ok(liste);
        }
        #endregion
    }
}
