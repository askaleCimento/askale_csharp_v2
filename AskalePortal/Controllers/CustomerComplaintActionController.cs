using AskalePortal.BLL;
using AskalePortal.Constants;
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
    public class CustomerComplaintActionController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public CustomerComplaintActionController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<CustomerComplaintActionSaveDto?>> save([FromForm] CustomerComplaintActionSaveDto entity)
        {

            BLLActions.MusteriSikayetAction bllMusteriSikayetAction = new BLLActions.MusteriSikayetAction(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity?.FindFirst("userId")?.Value ?? "0");
            }
            CustomerComplaintActionSaveDto dto = await bllMusteriSikayetAction.save(entity, userId);
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
                        f.moduleId = (int)CommonConstants.MODULES.MUSTERI_SIKAYET_AKSIYON;
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
    }
}
