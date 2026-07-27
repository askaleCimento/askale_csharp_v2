using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DieselPriceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public DieselPriceController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region delete
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
                bllDieselPrice.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);

            DieselPrice? dieselPrice = bllDieselPrice.GetByID(id);
            if (dieselPrice == null)
            {
                return NotFound();
            }
            return Ok(dieselPrice);


        }
        #endregion

        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            int count = bllDieselPrice.approvalCount(userId);
            return Ok(count);
        }
        #endregion

        #region activeList
        [HttpPost("active")]
        public ActionResult<object> listActive([FromForm] int userId)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            List<DieselPrice> liste = bllDieselPrice.listActive(userId);
            return Ok(liste);
        }
        #endregion

        #region activemyapprovallist
        [HttpPost("activemyapprovallist")]
        public ActionResult<object> activeMyApprovalList([FromForm] FilterParam<DieselPriceListDtoParameter> filterParam)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            List<DieselPrice> list = bllDieselPrice.activeMyApprovalList(filterParam);
            return Ok(list);
        }
        #endregion
        #region completedlist
        [HttpPost("completed")]
        public ActionResult<object> listCompleted([FromForm] int userId)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            List<DieselPrice> list = bllDieselPrice.listCompleted(userId);

            return Ok(list);
        }
        #endregion

        #region save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] DieselPriceDto entity)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            DieselPrice? saveDieselprice =await bllDieselPrice.save(entity, userId);


            return Ok(saveDieselprice);


        }
        #endregion

        #region confirmsave
        [HttpPost("confirmsave")]
        public async Task<ActionResult<int>> confirmSave([FromForm] int id, [FromForm] int userId)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            int deger = await bllDieselPrice.confirmSave(id, userId);
            return Ok(deger);
        }
        #endregion

        #region dieselPriceDate
        [HttpPost("dieselPriceDate")]
        public ActionResult<object> dieselPriceDate([FromForm] DateTime date, [FromForm] int companyId)
        {
            BLLActions.DieselPrice bllDieselPrice = new BLLActions.DieselPrice(_configuration, _env, _mapper);
            DieselPrice? dieselPrice = bllDieselPrice.dieselPriceDate(date, companyId);

            return Ok(dieselPrice);
        }
        #endregion

        #region rejectsave
        [HttpPost("rejectsave")]
        public async Task<ActionResult<int>> rejectSave([FromForm] int id, [FromForm] int userId)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            int deger = await bllDieselPrice.rejectSave(id, userId);
            return Ok(deger);
        }
        #endregion
        #region dieselPriceByDate
        [HttpPost("dieselPriceByDate")]

        public ActionResult<object> dieselPriceByDate([FromForm] string date, [FromForm] int companyId)
        {
            BLL.BLLActions.DieselPrice bllDieselPrice = new BLL.BLLActions.DieselPrice(_configuration, _env, _mapper);
            DateTime tarih = DateTime.ParseExact(date, "dd.MM.yyyy", System.Globalization.CultureInfo.InvariantCulture);
            DieselPrice? listDieselPrice = bllDieselPrice.dieselPriceByDate(tarih, companyId);
            return Ok(listDieselPrice);
        }
        #endregion

    }
}
