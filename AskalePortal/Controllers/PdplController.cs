using AskalePortal.BLL;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdplController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public PdplController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region gettopid
        [HttpPost("gettopid")]

        public ActionResult<object> gettopid([FromForm] int topid)
        {
            BLLActions.KVKDocuments bllKVKDocuments = new BLLActions.KVKDocuments(_configuration, _env , _mapper);
            List<KVKDocument>? liste = bllKVKDocuments.getByTopId(topid);
            return Ok(liste ?? []);
        }
        #endregion
        #region createfolderleft
        [HttpPost("createfolderleft")]

        public async Task<ActionResult<CorporateDocumentsDto>> createfolderleft([FromForm] CorporateDocumentsDto kurumsalDocumentsDto)
        {

            KVKDocument kurumsalDocument = _mapper.Map<Data.Models.KVKDocument>(kurumsalDocumentsDto);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? user = bllAdminUsers.GetByID(userId);
            kurumsalDocument.documentId = (Guid.NewGuid());
            kurumsalDocument.createdUserId = (user.Id);
            kurumsalDocument.createdByUserName = (user.name);
            kurumsalDocument.createdDate = (DateTime.Now);
            kurumsalDocument.archiveId = (0);
            kurumsalDocument.enabled = (true);
            if (kurumsalDocumentsDto.typeID == 1)
            {
                kurumsalDocument.fileSize = (0);
            }
            BLLActions.KVKDocuments bllKVKDocuments = new BLLActions.KVKDocuments(_configuration, _env, _mapper);
            CorporateDocumentsDto dto = await bllKVKDocuments.saveFolder(kurumsalDocument);
            return Ok(dto);
        }

        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.KVKDocuments bllKVKDocuments = new BLLActions.KVKDocuments(_configuration, _env, _mapper);

            KVKDocument? kvkDocuments = (bllKVKDocuments.GetByID(id));

            return Ok(kvkDocuments);
        }
        #endregion



        #region upload
        [HttpPost]
        [Route("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> upload()
        {
            IFormFileCollection files = Request.Form.Files;
            string fileName = Request.Form["fileName"].ToString();
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                        _configuration["FilePath:test"]!, "KVKDocuments\\");
                    if (filePath == null)
                    {

                    }
                    else
                    {
                       
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
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "KVKDocuments\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion
    }
}
