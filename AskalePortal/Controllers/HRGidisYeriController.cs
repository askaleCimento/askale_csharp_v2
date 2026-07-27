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
    public class HRGidisYeriController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public HRGidisYeriController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
           BLLActions.HRGidisYeri bllHRGidisYeri = new BLLActions.HRGidisYeri(_configuration, _env);

            List<HRGidisYeri>? listHRGidisYeri = bllHRGidisYeri.GetAll().OrderByDescending(u => u.Id).ToList();
            return Ok(listHRGidisYeri);

        }
        #endregion

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRGidisYeri bllHRGidisYeri = new BLLActions.HRGidisYeri(_configuration, _env);

            HRGidisYeri? hrGidisYeri = bllHRGidisYeri.GetByID(id);
            if (hrGidisYeri == null)
            {
                return NotFound();
            }
            return Ok(hrGidisYeri);


        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
               BLLActions.HRGidisYeri bllHRGidisYeri = new BLLActions.HRGidisYeri(_configuration, _env);
                bllHRGidisYeri.Delete(id);
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
        public async Task<ActionResult<object>> save([FromForm] HRGidisYeriSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.HRGidisYeri bllHRGidisYeri = new BLLActions.HRGidisYeri(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllHRGidisYeri.Update(_mapper.Map<HRGidisYeri>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllHRGidisYeri.Add(_mapper.Map<HRGidisYeri>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

    }
}
