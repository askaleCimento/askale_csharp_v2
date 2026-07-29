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
    public class CompanySectionController : ControllerBase
    {
        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public CompanySectionController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<CompanySectionSaveDto>> save(
            [FromForm] CompanySectionSaveDto? entity)
        {

            if (entity != null)
            {
                int userId = 0;

                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(
                        claimsIdentity?.FindFirst("userId")?.Value ?? "0"
                    );
                }
                BLL.BLLActions.CompanySection bllCompanySection = new BLL.BLLActions.CompanySection(_configuration, _env);

             
                if (entity.id != null)
                {
                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;

                    await bllCompanySection.Update(
                        _mapper.Map<CompanySection>(entity)
                    );

                    return Ok(entity);
                }
                else
                {
                    entity.createdDate = DateTime.Now;
                    entity.createdUserId = userId == 0 ? null : userId;
                    entity.enabled = true;

                    CompanySection? addedCompanySection =
                        await bllCompanySection.Add(
                            _mapper.Map<CompanySection>(entity)
                        );

                    CompanySectionSaveDto returnDto =
                        _mapper.Map<CompanySectionSaveDto>(addedCompanySection);

                    return Ok(returnDto);
                }
            }

            return Ok(null);
        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.CompanySection bllCompanySection = new BLLActions.CompanySection(_configuration, _env);
                bllCompanySection.Delete(id);
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
            BLLActions.CompanySection bllCompanySection = new BLLActions.CompanySection(_configuration, _env);

            CompanySection companySection = bllCompanySection.GetByID(id);
            if (companySection == null)
            {
                return NotFound();
            }
            return Ok(companySection);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLLActions.CompanySection bllCompanySection = new BLLActions.CompanySection(_configuration, _env);

            List<CompanySection>? listCompanySection = bllCompanySection.GetAll();
            return Ok(listCompanySection);

        }
        #endregion


        #region listGraph
        [HttpPost("listGraph")]
        public ActionResult<object> listGraph()
        {
            BLLActions.CompanySection bllCompanySection = new BLLActions.CompanySection(_configuration, _env);
            List<CompanySection>? listCompanySection = bllCompanySection.listGraph();
            return Ok(listCompanySection);
        }
        #endregion
    }
}
