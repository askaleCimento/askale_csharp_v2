using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRTripDescriptionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRTripDescriptionController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.HRTripDescription bllHRTripDescription = new BLLActions.HRTripDescription(_configuration, _env);

            List<HRTripDescription>? listHRTripDescription = bllHRTripDescription.GetAll().OrderByDescending(u => u.Id).ToList();
            return Ok(listHRTripDescription);

        }
        #endregion


        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRTripDescription bllHRTripDescription = new BLLActions.HRTripDescription(_configuration, _env);

            HRTripDescription? hrTripDescription = bllHRTripDescription.GetByID(id);
            if (hrTripDescription == null)
            {
                return NotFound();
            }
            return Ok(hrTripDescription);


        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.HRTripDescription bllHRTripDescription = new BLLActions.HRTripDescription(_configuration, _env);
                bllHRTripDescription.Delete(id);
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
        public async Task<ActionResult<object>> save([FromForm] HRTripDescriptionSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HRTripDescription bllHRTripDescription = new BLLActions.HRTripDescription(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHRTripDescription.Update(_mapper.Map<HRTripDescription>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHRTripDescription.Add(_mapper.Map<HRTripDescription>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion


    }
}
