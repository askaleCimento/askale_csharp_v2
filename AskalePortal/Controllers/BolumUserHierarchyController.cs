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
    public class BolumUserHierarchyController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public BolumUserHierarchyController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getbyuserId
        [HttpPost("getbyuserId")]
        public ActionResult<object> getbyuserId()
        {
            BLLActions.CeoTable bllCeoTable = new BLLActions.CeoTable(_configuration, _env);
            Data.Models.CeoTable? ceoTable = bllCeoTable.GetByID(1);
            BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
            List<BolumUserHierarchyTable> list = [];
            if (ceoTable != null)
            {
                list = bllBolumUserHierarchyTable.GetByUserId(ceoTable!.userId);
            }

            return Ok(list);
        }
        #endregion
       
        #region getbymanagerid
        [HttpPost("getbymanagerid")]
        public ActionResult<object> getbymanagerid([FromForm] int managerId)
        {
            BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
            List<BolumUserHierarchyTable> liste = bllBolumUserHierarchyTable.getbymanagerid(true, managerId);
            return Ok(liste);
        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] BolumUserHierarchyTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);

                if (entity?.id != null)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllBolumUserHierarchyTable.Update(_mapper.Map<BolumUserHierarchyTable>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    await bllBolumUserHierarchyTable.Add(_mapper.Map<BolumUserHierarchyTable>(entity));
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
            BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);

            List<BolumUserHierarchyTable>? listBolumUserHierarchyTable = bllBolumUserHierarchyTable.GetAll().OrderByDescending(u => u.Id).ToList();
            return Ok(listBolumUserHierarchyTable);

        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);
                bllBolumUserHierarchyTable.Delete(id);
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
            BLLActions.BolumUserHierarchyTable bllBolumUserHierarchyTable = new BLLActions.BolumUserHierarchyTable(_configuration, _env);

            BolumUserHierarchyTable? bolumUserHierarchyTable = bllBolumUserHierarchyTable.GetByID(id);
            if (bolumUserHierarchyTable == null)
            {
                return NotFound();
            }
            return Ok(bolumUserHierarchyTable);


        }
        #endregion



    }
}
