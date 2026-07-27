using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRExpenseAmountController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public HRExpenseAmountController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region listExpenseAmount
        [HttpPost("listExpenseAmount")]
        public ActionResult<PageReturn<HRExpenseAmountDto>> listExpenseAmount([FromForm] FilterPageParam<HRExpenseAmountRequestDto> filterPageParam)
        {
            BLLActions.HRExpenseAmount bllHRExpenseAmount = new BLLActions.HRExpenseAmount(_configuration, _env);

            PageReturn<HRExpenseAmountDto>? page = bllHRExpenseAmount.listExpenseAmount(filterPageParam);
            return Ok(page);

        }
        #endregion

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.HRExpenseAmount bllHRExpenseAmount = new BLL.BLLActions.HRExpenseAmount(_configuration, _env);

            HRExpenseAmount? hrExpenseAmount = bllHRExpenseAmount.GetByID(id);
            if (hrExpenseAmount == null)
            {
                return NotFound();
            }
            return Ok(hrExpenseAmount);


        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.HRExpenseAmount bllHRExpenseAmount = new BLL.BLLActions.HRExpenseAmount(_configuration, _env);
                bllHRExpenseAmount.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HRExpenseAmountSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.HRExpenseAmount bllHRExpenseAmount = new BLL.BLLActions.HRExpenseAmount(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHRExpenseAmount.Update(_mapper.Map<HRExpenseAmount>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHRExpenseAmount.Add(_mapper.Map<HRExpenseAmount>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

        #region getbycalisanturuidandharcamaturuid
        [HttpPost("getbycalisanturuidandharcamaturuid")]
        public ActionResult<object> getbycalisanturuidandharcamaturuid([FromForm] int calisanTuruId,
            [FromForm] int harcamaTuruId, [FromForm] string? harcamaTarihi)
        {
            BLLActions.HRExpenseAmount bllHRExpenseAmount = new BLLActions.HRExpenseAmount(_configuration, _env);
            HRExpenseAmount amount = bllHRExpenseAmount.getbycalisanturuidandharcamaturuid(calisanTuruId, harcamaTuruId, harcamaTarihi);
            return Ok(amount);
        }
        #endregion
    }
}
