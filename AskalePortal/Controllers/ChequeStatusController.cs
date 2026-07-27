using AskalePortal.BLL;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChequeStatusController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ChequeStatusController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        [HttpPost("getCheques")]
        public ActionResult<List<Data.SAP.OutputParams.ChequeStatusModel>?> getCheque([FromForm] string tarih)
        {
            BLLActions.ChequeStatus bllChequeStatus = new BLLActions.ChequeStatus(_configuration, _env);
            Data.SAP.OutputParams.ChequeStatus? chequeStatusModel = bllChequeStatus.getCheques(tarih);
            if (chequeStatusModel?.listChequeStatusModel == null)
            {
                return Ok(null);
            }
            else
            {
                return Ok(chequeStatusModel?.listChequeStatusModel.ToList());
            }

        }
    }

}

