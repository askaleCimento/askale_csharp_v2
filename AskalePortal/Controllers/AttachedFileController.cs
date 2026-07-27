using AskalePortal.BLL;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AttachedFileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public AttachedFileController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<object>> save([FromForm] AttachedFileSaveDto? entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.AttachedFiles bllAttachedFile = new BLL.BLLActions.AttachedFiles(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now;
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllAttachedFile.Update(_mapper.Map<AttachedFile >(entity));
                    return Ok(entity);
                }
                else
                {
                    if (entity != null)
                    {
                        entity.createdDate = DateTime.Now;
                        entity.createdUserId = userId;
                        entity.enabled = true;
                        await bllAttachedFile.Add(_mapper.Map<AttachedFile>(entity));
                        return Ok(entity);
                    }
                  
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
                BLL.BLLActions.AttachedFiles bllAttachedFile = new BLL.BLLActions.AttachedFiles(_configuration, _env);
                bllAttachedFile.Delete(id);
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
            BLL.BLLActions.AttachedFiles bllAttachedFile = new BLL.BLLActions.AttachedFiles(_configuration, _env);

            AttachedFile? attachedFile = bllAttachedFile.GetByID(id);
            if (attachedFile == null)
            {
                return NotFound();
            }
            return Ok(attachedFile);

        }
        #endregion
        #region getByModuleIdAndTargetId
        [HttpPost("getByModuleIdAndTargetId")]
        public ActionResult<object> getByModuleIdAndTargetId([FromForm] int moduleId, [FromForm] int targetId)
        {
            BLL.BLLActions.AttachedFiles bllAttachedFile = new BLL.BLLActions.AttachedFiles(_configuration, _env);

            List<AttachedFile> listAttachedFiles = bllAttachedFile.getByModuleIdAndTargetId(moduleId, targetId);
            return Ok(listAttachedFiles);

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
