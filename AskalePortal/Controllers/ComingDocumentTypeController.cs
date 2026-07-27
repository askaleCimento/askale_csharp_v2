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
    public class ComingDocumentTypeController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public ComingDocumentTypeController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getAll
        [HttpPost("getAll")]
        public ActionResult<object> getAll()
        {
            BLLActions.IncomingDocumentTypes bllIncomingDocumentTypes = new BLLActions.IncomingDocumentTypes(_configuration, _env);

            List<IncomingDocumentType>? list = (bllIncomingDocumentTypes.GetAll());

            return Ok(list);
        }
        #endregion



        #region filterByPageable
        [HttpPost("filterByPageable")]
        public ActionResult<PageReturn<ComingDocumentTypeDto>> filterPageable([FromForm] FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
        {
            BLLActions.IncomingDocumentTypes bllIncomingDocumentTypes = new BLLActions.IncomingDocumentTypes(_configuration, _env);
            PageReturn<ComingDocumentTypeDto> dto = bllIncomingDocumentTypes.listByPageable(filterPageParam);
            return Ok(dto);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.IncomingDocumentTypes bllIncomingDocumentTypes = new BLLActions.IncomingDocumentTypes(_configuration, _env);

            IncomingDocumentType? incomingDocument = (bllIncomingDocumentTypes.GetByID(id));

            return Ok(incomingDocument);
        }
        #endregion

        #region delete 
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.IncomingDocumentTypes bllIncomingDocumentTypes = new BLLActions.IncomingDocumentTypes(_configuration, _env);
                bllIncomingDocumentTypes.Delete(id);
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
        public async Task<ActionResult<IncomingDocumentTypeSaveDto?>> save([FromForm] IncomingDocumentTypeSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.IncomingDocumentTypes bllIncomingDocumentTypes = new BLLActions.IncomingDocumentTypes(_configuration, _env);

                if (entity?.id != 0)
                {

                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllIncomingDocumentTypes.Update(_mapper.Map<Data.Models.IncomingDocumentType>(entity));
                    return Ok(entity);
                }
                else
                {

                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllIncomingDocumentTypes.Add(_mapper.Map<Data.Models.IncomingDocumentType>(entity));
                    return Ok(entity);
                }
            }
            return Ok(null);
        }
        #endregion

    }

}
