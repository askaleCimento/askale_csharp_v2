

using AskalePortal.BLL;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExecutiveDocumentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public ExecutiveDocumentsController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.IcraDocuments bllIcraDocuments = new BLLActions.IcraDocuments(_configuration, _env);
                bllIcraDocuments.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

      

        #region createfolderleft
        [HttpPost("createfolderleft")]
        public async Task<ActionResult<Data.ResponseModels.ExecutiveDocumentsDto>> createfolderleft([FromForm] Data.ResponseModels.ExecutiveDocumentsDto icraDocumentsDto)
        {

            Data.Models.IcraDocument icraDocument = new Data.Models.IcraDocument();
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? user = bllAdminUsers.GetByID(userId);
            icraDocument.title = icraDocumentsDto.title ??"";
            icraDocument.typeName = icraDocumentsDto.typeName??"";
            icraDocument.topId = icraDocumentsDto.topID ?? 0;
            icraDocument.typeId = icraDocumentsDto.typeID ?? 0;
            icraDocument.filename = icraDocumentsDto.filename;
            icraDocument.fileSize = icraDocumentsDto.fileSize;
            icraDocument.documentID = Guid.NewGuid();
            icraDocument.createdUserId = user?.Id??0;
            icraDocument.createdByUserName = user?.name ??"";
            icraDocument.createdDate = DateTime.Now;
            icraDocument.archiveId = 0;
            icraDocument.enabled = true;
            if (icraDocumentsDto.typeID == 1)
            {
                icraDocument.fileSize = 0;
            }
            BLLActions.IcraDocuments bllIcraDocuments = new BLLActions.IcraDocuments(_configuration, _env);

            await bllIcraDocuments.Add(icraDocument);
            return Ok(icraDocument);

        }
        #endregion



        #region gettopid
        [HttpPost("gettopid")]
        public ActionResult<List<Data.ResponseModels.ExecutiveDocumentsDto>> gettopid([FromForm] int topid)
        {
            BLLActions.IcraDocuments bllIcraDocuments = new BLLActions.IcraDocuments(_configuration, _env);
            List<Data.ResponseModels.ExecutiveDocumentsDto> liste = bllIcraDocuments.getByTopId(topid);
            return Ok(liste);
        }
        #endregion

        #region gettopid-route-compatibility
        [HttpPost("gettopid/{topid:int}")]
        public ActionResult<List<Data.ResponseModels.ExecutiveDocumentsDto>> gettopidRoute([FromRoute] int topid)
        {
            return gettopid(topid);
        }
        #endregion

        #region getid
        [HttpPost("getid")]
        public ActionResult<Data.ResponseModels.ExecutiveDocumentsDto?> getid([FromForm] int id)
        {
            BLLActions.IcraDocuments bllIcraDocuments = new BLLActions.IcraDocuments(_configuration, _env);
            Data.ResponseModels.ExecutiveDocumentsDto? dto = bllIcraDocuments.getById(id);

            return Ok(dto);
        }
        #endregion

        #region upload
        [HttpPost]
        [Route("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> upload()
        {
            IFormFileCollection files = Request.Form.Files;
            string file = Request.Form["fileName"].ToString();
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                        _configuration["FilePath:test"]!, "IcraDocuments\\");
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


                    }

                }

            }

            return Ok(new { count = 1, size });

        }
        #endregion

        #region download
        [HttpPost("download")]
        public ActionResult<ResponseByteArray> download([FromForm] string file)
        {

            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "IcraDocuments\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion
    }
}
