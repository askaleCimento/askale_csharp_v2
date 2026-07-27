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
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InternalCorrespondenceController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public InternalCorrespondenceController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> approvalCount([FromForm] int userId)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int count = bllDahiliYazismaTable.approvalCount(userId);
            return Ok(count);
        }
        #endregion

        #region approvalKanalGorusuCount
        [HttpPost("approvalKanalGorusuCount")]
        public ActionResult<int> approvalKanalGorusuCount([FromForm] int userId)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int count = bllDahiliYazismaTable.approvalKanalGorusuCount(userId);

            return Ok(count);
        }
        #endregion

        #region kanalGorusuBitenCount
        [HttpPost("kanalGorusuBitenCount")]
        public ActionResult<int> kanalGorusuBitenCount([FromForm] int userId)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int count = bllDahiliYazismaTable.kanalGorusuBitenCount(userId);

            return Ok(count);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<InternalCorrespondenceSaveDto> getById([FromForm] int id)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);

            Data.Models.DahiliYazismaTable? dahiliYazismaTable = bllDahiliYazismaTable.GetByID(id);
            if (dahiliYazismaTable == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<InternalCorrespondenceSaveDto>(dahiliYazismaTable));


        }
        #endregion


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<InternalCorrespondenceSaveDto?>> save([FromForm] InternalCorrespondenceSaveDto? entity)
        {

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            if (entity != null)
            {
                InternalCorrespondenceSaveDto save = await bllDahiliYazismaTable.save(entity!, userId);
                return Ok(save);
            }
            else
            {
                return Ok(null);
            }

        }
        #endregion
        #region list
        [HttpPost("list")]
        public ActionResult<PageReturn<InternalCorrespondenceDto>> list([FromForm] FilterPageParam<InternalCorrespondenceListParameterDto> filterPageParam)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            PageReturn<InternalCorrespondenceDto>? page = bllDahiliYazismaTable.list(filterPageParam);
            return Ok(page);
        }
        #endregion


        #region getDetail
        [HttpPost("getDetail")]

        public ActionResult<InternalCorrespondenceDetailDto> getDetail(
            [FromForm] InternalCorrespondenceDto internalCorrespondenceDto)
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            InternalCorrespondenceDetailDto? dto = bllDahiliYazismaTable.getDetail(internalCorrespondenceDto, userId);
            return Ok(dto);
        }
        #endregion

        #region listPageableBilgi
        [HttpPost("listPageableBilgi")]
        public ActionResult<PageReturn<InternalCorrespondenceDto>> listPageableBilgi([FromForm] FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            PageReturn<InternalCorrespondenceDto> page = bllDahiliYazismaTable.listPageableBilgi(filterPageParam);
            return Ok(page);
        }
        #endregion

        #region mylist
        [HttpPost("mylist")]
        public ActionResult<PageReturn<InternalCorrespondenceDto>> mylist([FromForm] FilterPageParam<InternalCorrespondenceListParameterDto> filterPageParam)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            PageReturn<InternalCorrespondenceDto> page = bllDahiliYazismaTable.mylist(filterPageParam);

            return Ok(page);
        }
        #endregion
        #region mylistcanal
        [HttpPost("mylistcanal")]
        public ActionResult<PageReturn<InternalCorrespondenceDto>> mylistcanal([FromForm] FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            PageReturn<InternalCorrespondenceDto> page = bllDahiliYazismaTable.mylistcanal(filterPageParam);

            return Ok(page);
        }
        #endregion
        #region approve
        [HttpPost("approve")]
        public async Task<ActionResult<int>> approved([FromForm] ResponseMyList responseMyList)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllDahiliYazismaTable.approve(responseMyList, userId);
            return Ok(returnInteger);
        }
        #endregion
        #region approvecanal
        [HttpPost("approvecanal")]
        public async Task<ActionResult<int>> approvecanal([FromForm] ResponseMyList responseMyList)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllDahiliYazismaTable.approvecanal(responseMyList, userId);
            return Ok(returnInteger);
        }
        #endregion
        #region lastoperationapprove
        [HttpPost("lastoperationapprove")]
        public async Task<ActionResult<int>> lastoperationapprove([FromForm] ResponseMyList responseMyList)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllDahiliYazismaTable.lastoperationapprove(responseMyList, userId);
            return Ok(returnInteger);
        }
        #endregion
        #region mylastoperation
        [HttpPost("mylastoperation")]
        public ActionResult<PageReturn<InternalCorrespondenceDto>> mylastoperation([FromForm] FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            PageReturn<InternalCorrespondenceDto> page = bllDahiliYazismaTable.mylastoperation(filterPageParam);
            return Ok(page);
        }
        #endregion

        #region backtoceo
        [HttpPost("backtoceo")]
        public async Task<ActionResult<int>> backtoceo([FromForm] ResponseMyList responseMyList)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllDahiliYazismaTable.backtoceo(responseMyList, userId);
            return Ok(returnInteger);
        }
        #endregion

        #region red
        [HttpPost("red")]
        public async Task<ActionResult<int>> red([FromForm] InternalCorrespondenceSaveDto dahiliYazismaTable)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllDahiliYazismaTable.red(dahiliYazismaTable, userId);
            return Ok(returnInteger);
        }
        #endregion

        #region gerigonder
        [HttpPost("gerigonder")]
        public async Task<ActionResult<int>> gerigonder([FromForm] ResponseMyList responseMyList)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllDahiliYazismaTable.gerigonder(responseMyList, userId);
            return Ok(returnInteger);
        }
        #endregion

        #region endit
        [HttpPost("endit")]
        public async Task<ActionResult<int>> endit([FromForm] ResponseMyList responseMyList)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllDahiliYazismaTable.endit(responseMyList, userId);
            return Ok(returnInteger);
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
                        string fileName = Path.GetFileNameWithoutExtension(formFile.FileName) + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + Path.GetExtension(formFile.FileName);

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
                        f.moduleId = (int)CommonConstants.MODULES.DAHILIYAZISMA;
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

        #region saveNote
        [HttpGet("saveNote")]
        public async Task<ActionResult> saveNote(int id, string note, int noteUserId)
        {
            BLLActions.DahiliYazismaTable bllDahiliYazismaTable = new BLLActions.DahiliYazismaTable(_configuration, _env, _mapper);
            await bllDahiliYazismaTable.saveNotes(id, note, noteUserId);
            return Ok();
        }
        #endregion

    }
}
