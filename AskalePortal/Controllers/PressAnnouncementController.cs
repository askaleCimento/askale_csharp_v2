using AskalePortal.BLL;
using AskalePortal.Data.Functions;
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
    public class PressAnnouncementController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public PressAnnouncementController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] PressAnnouncementSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.PressAnnouncements bllPressAnnouncement = new BLL.BLLActions.PressAnnouncements(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllPressAnnouncement.Update(_mapper.Map< PressAnnouncement > (entity));
                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now;
                    entity.createdUserId = userId;
                    entity.enabled = true;
                    await bllPressAnnouncement.Add(_mapper.Map< PressAnnouncement >(entity));
                    return Ok(entity);
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
                BLL.BLLActions.PressAnnouncements bllPressAnnouncement = new BLL.BLLActions.PressAnnouncements(_configuration, _env);
                bllPressAnnouncement.Delete(id);
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
            BLL.BLLActions.PressAnnouncements bllPressAnnouncement = new BLL.BLLActions.PressAnnouncements(_configuration, _env);

            PressAnnouncement? pressAnnouncement = bllPressAnnouncement.GetByID(id);
            if (pressAnnouncement == null)
            {
                return NotFound();
            }
            return Ok(pressAnnouncement);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.PressAnnouncements bllPressAnnouncement = new BLL.BLLActions.PressAnnouncements(_configuration, _env);

            List<PressAnnouncement>? listPressAnnouncement = bllPressAnnouncement.GetAll();
            return Ok(listPressAnnouncement);

        }
        #endregion

        #region listPageablePressAnnouncementPicture
        [HttpPost("listPageablePressAnnouncementPicture")]
        public ActionResult<PageReturn<PressAnnouncementDto>> listPageablePressAnnouncementPicture(
             FilterPageParam<PressAnnouncementDtoParameter> filterPageParam)
        {
            BLL.BLLActions.PressAnnouncements bllPressAnnouncement = new BLL.BLLActions.PressAnnouncements(_configuration, _env);

            PageReturn<PressAnnouncementDto>? dto = bllPressAnnouncement.FilterPageableDto(filterPageParam);
            return Ok(dto);
        }
        #endregion



        #region listTop8PressAnnouncementPicture
        [HttpPost("listTop8PressAnnouncementPicture")]
        public ActionResult<object> listTop8PressAnnouncementPicture()
        {

            BLL.BLLActions.PressAnnouncements bllPressAnnouncement = new BLL.BLLActions.PressAnnouncements(_configuration, _env);

            List<PressAnnouncement>? dto = bllPressAnnouncement.ListTop8Picture();
            return Ok(dto);
        }
        #endregion


        #region downloadPicture
        [HttpPost("downloadPicture")]
        public ActionResult<ResponseByteArray> downloadPicture([FromForm] int announCementId)
        {
            BLLActions.PressAnnouncements bllPressAnnouncements = new BLLActions.PressAnnouncements(_configuration, _env);
            PressAnnouncement? pressAnnouncement = bllPressAnnouncements.GetByID(announCementId);
            string file = pressAnnouncement?.imageUrl ?? "";
            if (file.Equals(""))
            {
                return Ok(null);
            }
            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "uploads\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion

        #region downloadPictureAll
        [HttpPost("downloadPictureAll")]
        public ActionResult<List<IntegerAndResponseByteArrayDto>> downloadPictureAll([FromForm] int targetId,
           [FromForm] int moduleId)
        {
            List<IntegerAndResponseByteArrayDto> usersPictureDtos = new List<IntegerAndResponseByteArrayDto>();
            BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
            List<AttachedFile> attachedFiles = bllAttachedFiles.getByModuleIdAndTargetId(moduleId, targetId);
            foreach (AttachedFile attachedFile in attachedFiles)
            {
                IntegerAndResponseByteArrayDto dto = new IntegerAndResponseByteArrayDto();
                dto.userId = (attachedFile.Id);

                string file = attachedFile.filePath;
                if (file.Equals(null) || file.Equals(""))
                {
                    return Ok(null);
                }
                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                  _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "uploads\\");
                ResponseByteArray response = FileConverter.convertByte(filePath, file, file);
                dto.responseByteArray = (response);
                usersPictureDtos.Add(dto);
            }

            return Ok(usersPictureDtos);
        }
        #endregion

    }
}
