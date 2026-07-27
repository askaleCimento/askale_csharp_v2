using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalProcessController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ApprovalProcessController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("listApprovalCompanyIdAndTypeId")]
        public ActionResult<object> listApprovalCompanyIdAndTypeId([FromForm] int companyId,
            [FromForm] int typeId)
        {
            BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
            ApprovalProcess listApprovalProcess = bllApprovalProcesses.findByCompanyIdAndTypeIdAndEnabled(companyId, typeId);
            return Ok(listApprovalProcess);
        }

        [HttpPost("getAll")]
        public ActionResult<List<ApprovalProcessSaveDto>> listByEnabled()
        {
            BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
            List<ApprovalProcessSaveDto> liste = bllApprovalProcesses.listAllByEnabled(true);
            return Ok(liste);
        }

        [HttpPost("getById")]
        public ActionResult<ApprovalProcessSaveDto> getById([FromForm] int id)
        {

            BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper);
            ApprovalProcessSaveDto adminUser = _mapper.Map<ApprovalProcessSaveDto>(bllApprovalProcesses.GetByID(id));

            return Ok(adminUser);
        }

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.ApprovalProcesses bllApprovalProcesses = new BLLActions.ApprovalProcesses(_configuration, _env, _mapper  );
                bllApprovalProcesses.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

    }
}
