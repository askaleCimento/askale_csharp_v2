using AskalePortal.BLL;
using AskalePortal.Constants;
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
                    BLLActions.AdminUsers bllAdminUsers =
                        new BLLActions.AdminUsers(_configuration, _env, _mapper);

                    AdminUser? user = bllAdminUsers.GetByID(userId);

                    entity!.createdByUserName = user?.username ?? "";
                    entity.createdDate = DateTime.Now;
                    entity.createdUserId = userId;
                    entity.enabled = true;

                    // DTO -> Entity
                    PressAnnouncement pressAnnouncement =
                        _mapper.Map<PressAnnouncement>(entity);

                    // Kaydet ve DB'den dönen entity'yi al
                    PressAnnouncement savedPressAnnouncement =
                        await bllPressAnnouncement.Add(pressAnnouncement);

                    // Entity -> DTO
                    PressAnnouncementSaveDto result =
                        _mapper.Map<PressAnnouncementSaveDto>(savedPressAnnouncement);

                    return Ok(result);
                }
            }
            return Ok(null);
        }
        #endregion

        #region Upload picture
        [HttpPost("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult<PressAnnouncement>> upload(
            [FromForm] IFormFile file,
            [FromForm] int id)
        {
            if (file == null || file.Length <= 0)
            {
                return BadRequest("Yüklenecek görsel bulunamadı.");
            }

            BLLActions.PressAnnouncements bllPressAnnouncement =
                new BLLActions.PressAnnouncements(_configuration, _env);
            PressAnnouncement? pressAnnouncement = bllPressAnnouncement.GetByID(id);

            if (pressAnnouncement == null)
            {
                return NotFound($"Basın duyurusu bulunamadı. Id: {id}");
            }

            string directoryPath = GetUploadsDirectory();
            Directory.CreateDirectory(directoryPath);

            string newFileName = BuildFileName(
                file.FileName,
                $"_{id}_{DateTime.Now:dd_MM_yyyy_HH_mm_ss_fff}");
            string fileFullPath = Path.Combine(directoryPath, newFileName);

            await using (FileStream stream = new FileStream(
                fileFullPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None))
            {
                await file.CopyToAsync(stream, HttpContext.RequestAborted);
            }

            int userId = GetCurrentUserId();
            pressAnnouncement.imageUrl = newFileName;
            pressAnnouncement.updatedDate = DateTime.Now;
            pressAnnouncement.updatedUserId = userId == 0 ? null : userId;

            try
            {
                await bllPressAnnouncement.Update(pressAnnouncement);
            }
            catch
            {
                if (System.IO.File.Exists(fileFullPath))
                {
                    System.IO.File.Delete(fileFullPath);
                }
                throw;
            }

            return Ok(pressAnnouncement);
        }
        #endregion

        #region Upload attached files
        [HttpPost("uploadFiles")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> uploadFiles(
            [FromForm] IFormFile[] file,
            [FromForm] int targetId)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Yüklenecek dosya bulunamadı.");
            }

            BLLActions.PressAnnouncements bllPressAnnouncement =
                new BLLActions.PressAnnouncements(_configuration, _env);
            if (bllPressAnnouncement.GetByID(targetId) == null)
            {
                return NotFound($"Basın duyurusu bulunamadı. Id: {targetId}");
            }

            string directoryPath = GetUploadsDirectory();
            Directory.CreateDirectory(directoryPath);

            int userId = GetCurrentUserId();
            BLLActions.AttachedFiles bllAttachedFiles =
                new BLLActions.AttachedFiles(_configuration, _env);
            int uploadedCount = 0;
            long uploadedSize = 0;

            foreach (IFormFile formFile in file)
            {
                if (formFile == null || formFile.Length <= 0)
                {
                    continue;
                }

                string newFileName = BuildFileName(
                    formFile.FileName,
                    $"-{DateTime.Now:yyyyMMddHHmmssfff}-{Guid.NewGuid():N}",
                    maxBaseNameLength: 25);
                string fileFullPath = Path.Combine(directoryPath, newFileName);

                await using (FileStream stream = new FileStream(
                    fileFullPath,
                    FileMode.CreateNew,
                    FileAccess.Write,
                    FileShare.None))
                {
                    await formFile.CopyToAsync(stream, HttpContext.RequestAborted);
                }

                AttachedFile attachedFile = new AttachedFile
                {
                    createdDate = DateTime.Now,
                    createdUserId = userId,
                    enabled = true,
                    filePath = newFileName,
                    moduleId = (int)CommonConstants.MODULES.PRESS_ANNOUNCEMENTS,
                    title = newFileName,
                    targetId = targetId,
                    visitorCount = 0
                };

                try
                {
                    await bllAttachedFiles.Add(attachedFile);
                }
                catch
                {
                    if (System.IO.File.Exists(fileFullPath))
                    {
                        System.IO.File.Delete(fileFullPath);
                    }
                    throw;
                }

                uploadedCount++;
                uploadedSize += formFile.Length;
            }

            if (uploadedCount == 0)
            {
                return BadRequest("Yüklenebilir bir dosya bulunamadı.");
            }

            return Ok(new { count = uploadedCount, size = uploadedSize });
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
        public ActionResult<PageReturn<PressAnnouncementDto>> listPageablePressAnnouncementPicture([FromForm]
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

            List<PressAnnouncementDto>? dto = bllPressAnnouncement.ListTop8Picture();
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

        private int GetCurrentUserId()
        {
            if (HttpContext.User.Identity is not ClaimsIdentity claimsIdentity)
            {
                return 0;
            }

            return int.TryParse(claimsIdentity.FindFirst("userId")?.Value, out int userId)
                ? userId
                : 0;
        }

        private string GetUploadsDirectory()
        {
            string basePath = _env.IsDevelopment()
                ? _configuration["FilePath:local"]!
                : _env.IsProduction()
                    ? _configuration["FilePath:server"]!
                    : _configuration["FilePath:test"]!;

            return Path.Combine(basePath, "uploads");
        }

        private static string BuildFileName(
            string originalFileName,
            string suffix,
            int maxBaseNameLength = 100)
        {
            string safeOriginalFileName = Path.GetFileName(originalFileName);
            string extension = Path.GetExtension(safeOriginalFileName);
            string baseName = Path.GetFileNameWithoutExtension(safeOriginalFileName);

            foreach (char invalidChar in Path.GetInvalidFileNameChars())
            {
                baseName = baseName.Replace(invalidChar, '_');
            }

            baseName = string.IsNullOrWhiteSpace(baseName) ? "file" : baseName.Trim();
            if (baseName.Length > maxBaseNameLength)
            {
                baseName = baseName[..maxBaseNameLength];
            }

            return $"{baseName}{suffix}{extension.ToLowerInvariant()}";
        }

    }
}
