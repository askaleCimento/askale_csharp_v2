using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentArchiveController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public DocumentArchiveController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }



        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<DocumentArchiveSaveDto?>> save([FromForm] DocumentArchiveSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.DocumentArchives bllDocumentArchives = new BLLActions.DocumentArchives(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllDocumentArchives.Update(_mapper.Map<DocumentArchive>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    var savedKayit = await bllDocumentArchives.Add(_mapper.Map<DocumentArchive>(entity));
                    return Ok(savedKayit);
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
                BLLActions.DocumentArchives bllDocumentArchives = new BLLActions.DocumentArchives(_configuration, _env);
                bllDocumentArchives.Delete(id);
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
            BLLActions.DocumentArchives bllDocumentArchive = new BLLActions.DocumentArchives(_configuration, _env);

            DocumentArchive? documentArchive = bllDocumentArchive.GetByID(id);
            if (documentArchive == null)
            {
                return NotFound();
            }
            return Ok(documentArchive);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLLActions.DocumentArchives bllDocumentArchive = new BLLActions.DocumentArchives(_configuration, _env);

            List<DocumentArchive>? listDocumentArchive = bllDocumentArchive.GetAll();
            return Ok(listDocumentArchive);

        }
        #endregion

        #region getAllFilter
        [HttpPost("getAllFilter")]

        public ActionResult<object> getAllFilter([FromForm] FilterParam<HelpDeskStatusListDtoParameter> filterParam)
        {
            BLLActions.DocumentArchives bllDocumentArchive = new BLLActions.DocumentArchives(_configuration, _env);

            List<DocumentArchive>? listDocumentArchive = bllDocumentArchive.GetAllFilter(filterParam);
            return Ok(listDocumentArchive);

        }
        #endregion
    }
}
