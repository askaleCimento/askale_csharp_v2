using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComingDocumentSourceController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public ComingDocumentSourceController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.IncomingDocumentSources bllIncomingDocumentSources = new BLLActions.IncomingDocumentSources(_configuration, _env);

            List<IncomingDocumentSource>? list = (bllIncomingDocumentSources.GetAll());

            return Ok(list);
        }
        #endregion

        #region filterByPageable
        [HttpPost("filterByPageable")]
        public ActionResult<PageReturn<ComingDocumentSourceDto>> filterPageable([FromForm] FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
        {
            BLLActions.IncomingDocumentSources bllIncomingDocumentSources = new BLLActions.IncomingDocumentSources(_configuration, _env);
            PageReturn<ComingDocumentSourceDto> dto = bllIncomingDocumentSources.listByPageable(filterPageParam);
            return Ok(dto);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.IncomingDocumentSources bllIncomingDocumentSources = new BLLActions.IncomingDocumentSources(_configuration, _env);

            IncomingDocumentSource? incomingDocument = (bllIncomingDocumentSources.GetByID(id));

            return Ok(incomingDocument);
        }
        #endregion

        #region delete 
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.IncomingDocumentSources bllIncomingDocumentSources = new BLL.BLLActions.IncomingDocumentSources(_configuration, _env);
                bllIncomingDocumentSources.Delete(id);
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
        public async Task<ActionResult<IncomingDocumentSourceSaveDto?>> save([FromForm] IncomingDocumentSourceSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.IncomingDocumentSources bllIncomingDocumentSources = new BLL.BLLActions.IncomingDocumentSources(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllIncomingDocumentSources.Update(_mapper.Map<Data.Models.IncomingDocumentSource>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    IncomingDocumentSource? saveEntity = await bllIncomingDocumentSources.Add(_mapper.Map<Data.Models.IncomingDocumentSource>(entity));
                    return Ok(saveEntity);
                }
            }
            return Ok(null);
        }
        #endregion
    }
}
