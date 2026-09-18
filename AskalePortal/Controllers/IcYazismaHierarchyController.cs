using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IcYazismaHierarchyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public IcYazismaHierarchyController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getbyuserId
        [HttpPost("getbyuserId")]
        public ActionResult<object> getbyuserId()
        {
            BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
            List<Data.Models.IcYazismaHierarchyTable> liste = bllIcYazismaHierarchyTable.getbyuserId(true);
            return Ok(liste);
        }
        #endregion

        #region getbymanagerid
        [HttpPost("getbymanagerid")]
        public ActionResult<object> getbymanagerid([FromForm] int managerId)
        {
            BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
            List<Data.Models.IcYazismaHierarchyTable> liste = bllIcYazismaHierarchyTable.getbymanagerid(true, managerId);
            return Ok(liste);
        }
        #endregion


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] IcYazismaHierarchyTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllIcYazismaHierarchyTable.Update(_mapper.Map<IcYazismaHierarchyTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllIcYazismaHierarchyTable.Add(_mapper.Map<IcYazismaHierarchyTable>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);

            List<IcYazismaHierarchyTable>? listIcYazismaHierarchyTable = bllIcYazismaHierarchyTable.GetAll().OrderByDescending(u => u.Id).ToList();
            return Ok(listIcYazismaHierarchyTable);

        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);
                bllIcYazismaHierarchyTable.Delete(id);
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
           BLLActions.IcYazismaHierarchyTable bllIcYazismaHierarchyTable = new BLLActions.IcYazismaHierarchyTable(_configuration, _env);

            IcYazismaHierarchyTable? icYazismaHierarchyTable = bllIcYazismaHierarchyTable.GetByID(id);
            if (icYazismaHierarchyTable == null)
            {
                return NotFound();
            }
            return Ok(icYazismaHierarchyTable);


        }
        #endregion



    }
}
