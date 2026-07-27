using AskalePortal.BLL;
using AskalePortal.Constants;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
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
    public class IncomingDocumentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public IncomingDocumentController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.IncomingDocuments bllIncomingDocuments = new BLLActions.IncomingDocuments(_configuration, _env, _mapper);

            IncomingDocument? incomingDocument = (bllIncomingDocuments.GetByID(id));

            return Ok(incomingDocument);
        }
        #endregion


        #region filterByPageable
        [HttpPost("filterByPageable")]
        public ActionResult<PageReturn<IncomingDocumentDto>> filterPageable([FromForm] FilterPageParam<IncomingDocumentDtoRequest> filterPageParam)
        {
            BLLActions.IncomingDocuments bllIncomingDocuments = new BLLActions.IncomingDocuments(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity?.FindFirst("userId")?.Value ?? "0");
            }
            PageReturn<IncomingDocumentDto> page = bllIncomingDocuments.listByPageable(filterPageParam, userId);
            return Ok(page);
        }
        #endregion

        #region getMyEdit
        [HttpPost("getMyEdit")]

        public ActionResult<IncomingDocumentMyEditDto> getMyEdit(int id, bool isOutgoing)
        {
            BLLActions.IncomingDocuments bllIncomingDocuments = new BLLActions.IncomingDocuments(_configuration, _env, _mapper);
            IncomingDocumentMyEditDto dto = bllIncomingDocuments.getMyEdit(id, isOutgoing);
            return Ok(dto);
        }
        #endregion

        #region saveMyEdit
        [HttpPost("saveMyEdit")]
        public async Task<ActionResult<int>> saveMyEdit([FromForm] int id, [FromForm] string notes,
            [FromForm] bool isCompleted)
        {
            BLLActions.IncomingDocuments bllIncomingDocuments = new BLLActions.IncomingDocuments(_configuration, _env, _mapper);
            int deger = await bllIncomingDocuments.saveMyEdit(id, notes, isCompleted);
            return Ok(deger);
        }
        #endregion

        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<IncomingDocumentSaveDto?>> save([FromForm] IncomingDocumentSaveDto incomingDocument)
        {

            if (incomingDocument != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.IncomingDocuments bllIncomingDocuments = new BLL.BLLActions.IncomingDocuments(_configuration, _env, _mapper);

                if (incomingDocument?.id != 0)
                {

                    incomingDocument!.updateDate = DateTime.Now.ToString();
                    incomingDocument.updatedUserId = userId == 0 ? null : userId;
                    await bllIncomingDocuments.Update(_mapper.Map<Data.Models.IncomingDocument>(incomingDocument));
                    return Ok(incomingDocument);
                }
                else
                {

                    incomingDocument.createdDate = DateTime.Now.ToString();
                    incomingDocument.createdUserId = userId;
                    incomingDocument.enabled = true;
                    await bllIncomingDocuments.Add(_mapper.Map<Data.Models.IncomingDocument>(incomingDocument));
                    return Ok(incomingDocument);
                }
            }
            return Ok(null);
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
                        BLL.BLLActions.AttachedFiles bllAttachedFiles = new BLL.BLLActions.AttachedFiles(_configuration, _env);

                        AttachedFile f = new AttachedFile();
                        f.moduleId = (int)CommonConstants.MODULES.INCOMING_DOCUMENTS;
                        f.enabled = true;
                        f.targetId = targetId;
                        f.filePath = filePath;
                        f.createdUserId = userId;
                        f.createdDate = DateTime.Now;
                        f.title = formFile.FileName;
                        await bllAttachedFiles.Add(f);

                    }

                }

            }

            return Ok(new { count = 1, size });

        }
        #endregion


        #region download
        [HttpGet("download")]
        public ActionResult<ResponseByteArray> download([FromForm] string file)
        {

            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "documents\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion


        #region delete 
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.IncomingDocuments bllIncomingDocuments = new BLL.BLLActions.IncomingDocuments(_configuration, _env, _mapper);
                bllIncomingDocuments.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }

        #endregion
    }
}
