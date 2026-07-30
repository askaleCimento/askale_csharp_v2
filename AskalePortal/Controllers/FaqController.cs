using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.Functions;
using AskalePortal.Data.ResponseModels;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NuGet.Protocol;
using System.Security.Claims;
using AskalePortal.BLL;
using AutoMapper;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class FaqController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public FaqController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }



        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<FaqSaveDto?>> save([FromForm] FaqSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.Faqs bllFaq = new BLLActions.Faqs(_configuration, _env);

                if (entity?.id != null)
                {

                    entity.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    await bllFaq.Update(_mapper.Map<Faq>(entity));

                    return Ok(entity);
                }
                else
                {

                    entity!.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;
                    var saveData = await bllFaq.Add(_mapper.Map<Faq>(entity));
                    return Ok(saveData);
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
                BLLActions.Faqs bllFaq = new BLLActions.Faqs(_configuration, _env);
                bllFaq.Delete(id);
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
            BLLActions.Faqs bllFaq = new BLLActions.Faqs(_configuration, _env);

            Faq? faq = bllFaq.GetByID(id);
            if (faq == null)
            {
                return NotFound();
            }
            return Ok(faq);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLLActions.Faqs bllFaq = new BLLActions.Faqs(_configuration, _env);

            List<Faq>? listFaq = bllFaq.GetAll();
            return Ok(listFaq);

        }
        #endregion

        #region getAllFilter
        [HttpPost("getAllFilter")]

        public ActionResult<object> getAllFilter([FromForm] FilterParam<HelpDeskStatusListDtoParameter> filterParam)
        {
            BLLActions.Faqs bllFaq = new BLLActions.Faqs(_configuration, _env);

            List<Faq>? listFaq = bllFaq.GetAllFilter(filterParam);
            return Ok(listFaq);

        }
        #endregion

        #region upload
        [HttpPost]
        [Route("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> upload()
        {
            IFormFileCollection files = Request.Form.Files;
            int targetId = int.Parse(Request.Form["targetId"].ToString());
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                        _configuration["FilePath:test"]!, "documents\\");
                    if (filePath == null)
                    {

                    }
                    else
                    {
                        string fileName = Path.GetFileNameWithoutExtension(formFile.FileName) + "-" + DateTimeOffset.Now.ToUnixTimeSeconds() + Path.GetExtension(formFile.FileName);

                        string fileFull = Path.Combine(filePath, fileName);
                        using (var stream = System.IO.File.Create(fileFull))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                        int userId = 0;
                        if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                        {
                            userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                        }
                        BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);

                        AttachedFile f = new AttachedFile();
                        f.moduleId = (int)CommonConstants.MODULES.FAQS;
                        f.enabled = true;
                        f.targetId = targetId;
                        f.filePath = filePath;
                        f.createdUserId = userId;
                        f.createdDate = DateTime.Now;
                        f.title = fileName;
                        await bllAttachedFiles.Add(f);

                    }

                }

            }

            return Ok(new { count = 1, size });

        }
        #endregion

        #region downloadPictureAll

        [HttpPost("downloadPictureAll")]
        public ActionResult<List<IntegerAndResponseByteArrayDto>> downloadPictureAll(
            [FromForm] int targetId,
            [FromForm] int moduleId)
        {
            List<IntegerAndResponseByteArrayDto> usersPictureDtos = new();

            BLLActions.AttachedFiles bllAttachedFiles =
                new BLLActions.AttachedFiles(_configuration, _env);

            List<AttachedFile> attachedFiles =
                bllAttachedFiles.getByModuleIdAndTargetId(moduleId, targetId);

            string baseFilePath =
                _env.IsDevelopment()
                    ? _configuration["FilePath:local"]!
                    : _env.IsProduction()
                        ? _configuration["FilePath:server"]!
                        : _configuration["FilePath:test"]!;

            string directoryName =
                Path.Combine(baseFilePath, "documents") +
                Path.DirectorySeparatorChar;

            foreach (AttachedFile attachedFile in attachedFiles)
            {
                string? filename = attachedFile.title;

                if (string.IsNullOrWhiteSpace(filename))
                {
                    return Ok(null);
                }

                IntegerAndResponseByteArrayDto dto = new()
                {
                    userId = attachedFile.Id
                };

                ResponseByteArray response =
                    FileConverter.convertByte(
                        directoryName,
                        filename,
                        filename);

                dto.responseByteArray = response;

                usersPictureDtos.Add(dto);
            }

            return Ok(usersPictureDtos);
        }

        #endregion

        #region downloadPictureAllModuleId
        [HttpPost("downloadPictureAllModuleId")]
        public ActionResult<List<IntegerAndResponseByteArrayDto>> downloadPictureAllModuleId([FromForm] int moduleId)
        {
            List<IntegerAndResponseByteArrayDto> usersPictureDtos = [];
            BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
            List<AttachedFile> attachedFiles = bllAttachedFiles.GetByModuleID(moduleId);

            foreach (AttachedFile attachedFile in attachedFiles)
            {
                IntegerAndResponseByteArrayDto dto = new IntegerAndResponseByteArrayDto();
                dto.userId = (attachedFile.targetId);

                string filename = attachedFile.filePath;
                if (filename.Equals(null))
                {
                    return Ok(null);
                }
                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                 _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "documents\\");

                ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, attachedFile.title, attachedFile.title);
                dto.responseByteArray = responseByteArray;


                usersPictureDtos.Add(dto);
            }

            return Ok(usersPictureDtos);
        }
        #endregion

        #region download
        [HttpPost("download")]
        public ActionResult<ResponseByteArray> download([FromForm] string file)
        {

            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "documents\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion

        #region filterPageable
        [HttpPost("filterPageable")]

        public ActionResult<object> filterPageableDto([FromForm] FilterPageParam<UserFilterDtoRequest> filterPageParam)
        {

            BLLActions.Faqs bllFaqs = new BLLActions.Faqs(_configuration, _env);
            PageReturn<Faq>? liste = bllFaqs.FilterPageableDto(filterPageParam);
            return Ok(liste);
        }
        #endregion
    }
}
