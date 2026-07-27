using AskalePortal.BLL;
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using AskalePortal.Data.Functions;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class KurumsalDocumentsController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public KurumsalDocumentsController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] KurumsalDocumentSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.KurumsalDocuments bllKurumsalDocument = new BLL.BLLActions.KurumsalDocuments(_configuration, _env);

                if (entity?.id != 0)
                {
                    KurumsalDocument kurumsalDocument = _mapper.Map<KurumsalDocument>(entity);
                    kurumsalDocument!.updatedDate = DateTime.Now;
                    kurumsalDocument.updatedUserId = userId == 0 ? null : userId;
                    await bllKurumsalDocument.Update(kurumsalDocument);
                    return Ok(entity);
                }
                else
                {
                    KurumsalDocument kurumsalDocument = _mapper.Map<KurumsalDocument>(entity);

                    kurumsalDocument.createdDate = DateTime.Now;
                    kurumsalDocument.createdUserId = userId;
                    kurumsalDocument.enabled = true;
                    await bllKurumsalDocument.Add(kurumsalDocument);
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
                BLL.BLLActions.KurumsalDocuments bllKurumsalDocument = new BLL.BLLActions.KurumsalDocuments(_configuration, _env);
                bllKurumsalDocument.Delete(id);
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
            BLL.BLLActions.KurumsalDocuments bllKurumsalDocument = new BLL.BLLActions.KurumsalDocuments(_configuration, _env);

            KurumsalDocument? kurumsalDocument = bllKurumsalDocument.GetByID(id);
            if (kurumsalDocument == null)
            {
                return NotFound();
            }
            return Ok(kurumsalDocument);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.KurumsalDocuments bllKurumsalDocument = new BLL.BLLActions.KurumsalDocuments(_configuration, _env);

            List<KurumsalDocument>? listKurumsalDocument = bllKurumsalDocument.GetAll();
            return Ok(listKurumsalDocument);

        }
        #endregion


        #region createfolderleft
        [HttpPost("createfolderleft")]

        public async Task<ActionResult<Data.ResponseModels.KurumsalDocumentsDto>> createfolderleft([FromForm] Data.ResponseModels.KurumsalDocumentsDto kurumsalDocumentsDto)
        {
            try
            {

           
            KurumsalDocument kurumsalDocument = new KurumsalDocument();
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? user = bllAdminUsers.GetByID(userId);
            kurumsalDocument.title = kurumsalDocumentsDto.title;
            kurumsalDocument.typeName = kurumsalDocumentsDto.typeName;
            kurumsalDocument.topId = kurumsalDocumentsDto.topID ??0;
            kurumsalDocument.typeId=kurumsalDocumentsDto.typeID??0;
            kurumsalDocument.filename=kurumsalDocumentsDto.filename;
            kurumsalDocument.fileSize=kurumsalDocumentsDto.fileSize;
            kurumsalDocument.documentID = Guid.NewGuid();
            kurumsalDocument.createdUserId = user?.Id??0;
            kurumsalDocument.createdByUserName = user?.name ??"";
            kurumsalDocument.createdDate = DateTime.Now;
            kurumsalDocument.archiveId = 0;
            kurumsalDocument.enabled = true;
            if (kurumsalDocumentsDto.typeID == 1)
            {
                kurumsalDocument.fileSize = 0;
            }
                BLLActions.KurumsalDocuments bllKurumsalDocuments = new BLLActions.KurumsalDocuments(_configuration, _env);
            await bllKurumsalDocuments.Add(kurumsalDocument);
            return Ok(kurumsalDocument);
            }
            catch (Exception)
            {
                return Ok(null);
            }
        }
        #endregion

        #region gettopid
        [HttpPost("gettopid")]

        public ActionResult<List<Data.ResponseModels.KurumsalDocumentsDto>> gettopid([FromForm] int topid)
        {
            BLL.BLLActions.KurumsalDocuments bllKurumsalDocument = new BLL.BLLActions.KurumsalDocuments(_configuration, _env);
            List<Data.ResponseModels.KurumsalDocumentsDto> liste = bllKurumsalDocument.getByTopId(topid);
            return Ok(liste);
        }
        #endregion

        #region gettopid-route-compatibility
        [HttpPost("gettopid/{topid:int}")]
        public ActionResult<List<Data.ResponseModels.KurumsalDocumentsDto>> gettopidRoute([FromRoute] int topid)
        {
            return gettopid(topid);
        }
        #endregion

        #region getid
        [HttpPost("getid")]
        public ActionResult<Data.ResponseModels.KurumsalDocumentsDto?> getid([FromForm] int id)
        {
            BLL.BLLActions.KurumsalDocuments bllKurumsalDocument = new BLL.BLLActions.KurumsalDocuments(_configuration, _env);
            Data.ResponseModels.KurumsalDocumentsDto? dto = bllKurumsalDocument.getById(id);

            return Ok(dto ?? new KurumsalDocumentsDto());
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
                        _configuration["FilePath:test"]!, "KurumsalDocuments\\");
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
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "KurumsalDocuments\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion

    }
}