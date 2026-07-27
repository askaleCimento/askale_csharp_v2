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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SozlesmeTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public SozlesmeTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region pageablelist
        [HttpPost("pageablelist")]
        public ActionResult<PageReturn<SozlesmeTableDto>> listPageableDto([FromForm] FilterPageParam<SozlesmeTableListDtoParameter> filterPageParam)
        {
            BLLActions.SozlesmeTable bllSozlesmeTable = new BLLActions.SozlesmeTable(_configuration, _env, _mapper);
            PageReturn<SozlesmeTableDto>? liste = bllSozlesmeTable.FilterPageableDto(filterPageParam);
            return Ok(liste);


        }
        #endregion

        #region downloadPicture
        [HttpPost("downloadPicture")]
        public ActionResult<List<IntegerAndResponseByteArrayDto>?> downloadPictureAll([FromForm] int targetId)
        {
            List<IntegerAndResponseByteArrayDto> sozlesmeFiles = new List<IntegerAndResponseByteArrayDto>();
            BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);
            List<AttachedFile> attachedFiles = bllAttachedFiles.getByModuleIdAndTargetId((int)CommonConstants.MODULES.SOZLESMEGIRIS,
                    targetId);

            foreach (AttachedFile file in attachedFiles)
            {

                string filename = file.title;
                if (filename.Equals(null))
                {
                    return Ok(null);
                }

                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "documents\\");

                IntegerAndResponseByteArrayDto dto = new IntegerAndResponseByteArrayDto();
                dto.userId = file.Id;
                ResponseByteArray response = FileConverter.convertByte(filePath, filename, filename);
                dto.responseByteArray = (response);
                sozlesmeFiles.Add(dto);
            }

            return Ok(sozlesmeFiles);
        }
        #endregion


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<SozlesmeTableSaveDto?>> save([FromForm] SozlesmeTableSaveDto entity)
        {
            BLLActions.SozlesmeTable bllSozlesmeTable = new BLLActions.SozlesmeTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            SozlesmeTableSaveDto saveSozlesme = await bllSozlesmeTable.save(entity, userId);

            return Ok(saveSozlesme);
        }
        #endregion

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

                        string fileName = Path.GetFileNameWithoutExtension(formFile.FileName) + "-" +
                            DateTimeOffset.Now.ToUnixTimeSeconds() + Path.GetExtension(formFile.FileName);

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
                        f.moduleId = (int)CommonConstants.MODULES.SOZLESMEGIRIS;
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
        #region completed
        [HttpPost("completed")]
        public async Task<ActionResult<object>> completedSozlesme([FromForm] int id)
        {
            BLLActions.SozlesmeTable bllSozlesmeTable = new BLLActions.SozlesmeTable(_configuration, _env, _mapper);
            SozlesmeTable sozlesmeTable = await bllSozlesmeTable.completedSozlesme(id);
            return Ok(sozlesmeTable);
        }
        #endregion

        #region delete 
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.SozlesmeTable bllSozlesmeTable = new BLLActions.SozlesmeTable(_configuration, _env, _mapper);
                bllSozlesmeTable.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

        #region getPDFList 
        [HttpPost("getPDFList")]
        public ActionResult<List<SozlesmeTablePdfDto>> getPdfList()
        {
            BLLActions.SozlesmeTable bllSozlesmeTable = new BLLActions.SozlesmeTable(_configuration, _env, _mapper);
            List<SozlesmeTablePdfDto> list = bllSozlesmeTable.getPdfList();
            return Ok(list);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<Data.ResponseModels.SozlesmeTableSaveDto?> getById([FromForm] int id)
        {
            BLLActions.SozlesmeTable bllSozlesmeTable = new BLLActions.SozlesmeTable(_configuration, _env, _mapper);

            Data.ResponseModels.SozlesmeTableSaveDto? saveSozlesme = _mapper.Map<Data.ResponseModels.SozlesmeTableSaveDto?>(bllSozlesmeTable.GetByID(id));

            return Ok(saveSozlesme);
        }
        #endregion



        #region uploadTeminat

        [HttpPost]
        [Route("uploadTeminat")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> uploadTeminat()
        {
            IFormFileCollection files = Request.Form.Files;
            int targetId = int.Parse(Request.Form["targetId"].ToString());
            long size = files.Sum(f => f.Length);
            //DateTimeFormatter sdf = DateTimeFormatter.ofPattern("yyyyMMddHHmmss");
            foreach (var file in files)
            {
                string fName = "";
                if (file.FileName.Length >= 25)
                {
                    fName = file.FileName.Substring(0, 25);
                }
                else
                {
                    fName = file.FileName;
                }

                string newFileName = fName + "-" + (DateTime.Now.ToString("yyyyMMddHHmmss")) + "."
                        + Path.GetExtension(file.FileName);

                string directoryName = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                        _configuration["FilePath:test"]!, "documents\\");


                string fileFull = Path.Combine(directoryName, newFileName);
                using (var stream = System.IO.File.Create(fileFull))
                {
                    await file.CopyToAsync(stream);
                }
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);



                AttachedFile f = new AttachedFile();
                f.moduleId = (int)CommonConstants.MODULES.SOZLESMETEMINAT;
                f.enabled = true;
                f.targetId = targetId;
                f.filePath = newFileName;
                f.createdUserId = userId;
                f.createdDate = DateTime.Now;
                f.title = file.FileName;
                await bllAttachedFiles.Add(f);

            }
            return Ok(new { count = 1, size });

        }
        #endregion

    }






}
