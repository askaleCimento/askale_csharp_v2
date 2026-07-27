using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.Design;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalProcessDetailController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ApprovalProcessDetailController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getByProcessId
        [HttpPost("getByProcessId")]
        public async Task<ActionResult<object>> getByProcessId([FromForm] int processId)
        {

            BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
            List<ApprovalProcessDetail> listApprovalProcessDetail = await bllApprovalProcessDetails.GetAll(processId);
            return Ok(listApprovalProcessDetail);
        }
        #endregion
        #region changeOrder
        [HttpPost("changeOrder")]
        public async Task<ActionResult<bool>> changeOrder([FromForm] int processId, [FromForm] int oldIndex, [FromForm] int newIndex)
        {
            BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env,_mapper);
            bool deger = await bllApprovalProcessDetails.changeOrder(processId, oldIndex, newIndex);
            return Ok(deger);
        }
        #endregion


        #region save
        [HttpPost("save")]
        public async Task<ActionResult<ApprovalProcessDetailSaveDto>> save([FromForm] ApprovalProcessDetailSaveDto entity)
        {
            BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            ApprovalProcessDetailSaveDto dto =await bllApprovalProcessDetails.save(entity, userId);
            return Ok(dto);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
            ApprovalProcessDetail? detail = bllApprovalProcessDetails.GetByID(id);
            return Ok(detail);
        }
        #endregion

        #region delete
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {

            try
            {
                BLLActions.ApprovalProcessDetails bllApprovalProcessDetails = new BLLActions.ApprovalProcessDetails(_configuration, _env, _mapper);
                bllApprovalProcessDetails.Delete(id);
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
