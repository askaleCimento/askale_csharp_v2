using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseParams;
using Azure;
using Microsoft.AspNetCore.Mvc;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.RequestModel;
using AskalePortal.BLL;
using AskalePortal.Data.Models;
using System.Security.Claims;
using AutoMapper;
using AskalePortal.Constants;
using AskalePortal.Data.Functions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SureliIslerTakipController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public SureliIslerTakipController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region mylist
        [HttpPost("mylist")]
        public ActionResult<PageReturn<SureliIsTakipDto>> sureliIsTakipList([FromForm] FilterPageParam<SureliIslerTakipDtoParameter> filterPageParam)
        {
            BLLActions.SureliIsTakipTable bllSureliIsTakipTable = new BLLActions.SureliIsTakipTable(_configuration, _env, _mapper);
            PageReturn<SureliIsTakipDto>? liste = bllSureliIsTakipTable.FilterPageableDto(filterPageParam);
            return Ok(liste);
        }
        #endregion
        #region save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] SureliIsTakipSaveDto entity, [FromForm] SureliIsTakipSaveDto? eski)
        {
            BLLActions.SureliIsTakipTable bllSureliIsTakipTable = new BLLActions.SureliIsTakipTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            SureliIsTakipTable saveTable = await bllSureliIsTakipTable.save(entity, eski, userId);
            return Ok(saveTable);
        }
        #endregion

        #region deleteData
        [HttpPost("deleteData")]
        public async Task<ActionResult<object>> deleteData([FromForm] int id)
        {
            BLLActions.SureliIsTakipTable bllSureliIsTakipTable = new BLLActions.SureliIsTakipTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            SureliIsTakipTable? table = await bllSureliIsTakipTable.deleteData(id, userId);
            return Ok(table);
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
            BLLActions.SureliIsTakipTable bllSureliIsTakipTable = new BLLActions.SureliIsTakipTable(_configuration, _env, _mapper);
            SureliIsTakipTable? sureliIsTakipTable = bllSureliIsTakipTable.GetByID(targetId);
            String filesNames = "";
            foreach (var file in files)
            {
                if (file.Length > 0)
                {

                    string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                        _configuration["FilePath:test"]!, "SureliIslerTakip\\");
                    if (filePath == null)
                    {

                    }
                    else
                    {


                        string fileFull = Path.Combine(filePath, file.FileName);
                        using (var stream = System.IO.File.Create(fileFull))
                        {
                            await file.CopyToAsync(stream);
                        }
                        int userId = 0;
                        if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                        {
                            userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                        }
                        if (sureliIsTakipTable?.files == null || sureliIsTakipTable.files.Trim().Equals(""))
                        {

                            filesNames = file.FileName;
                        }
                        else
                        {
                            filesNames = sureliIsTakipTable.files + "$" + file.FileName;
                        }

                    }

                }

            }
            if (filesNames.EndsWith("$"))
            {
                filesNames = filesNames.Substring(0, filesNames.Length - 1);
            }
            sureliIsTakipTable.files = filesNames;
            await bllSureliIsTakipTable.Update(sureliIsTakipTable);
            return Ok(new { count = 1, size });

        }
        #endregion


        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.SureliIsTakipTable bllSureliIsTakipTable = new BLLActions.SureliIsTakipTable(_configuration, _env, _mapper);

            SureliIsTakipTable? sureliIsTakipTable = bllSureliIsTakipTable.GetByID(id);

            return Ok(sureliIsTakipTable);
        }
        #endregion

        #region download
        [HttpPost("download")]
        public ActionResult<ResponseByteArray?> downloadPicture([FromForm] string file)
        {
            if (file.Equals(null) || file.Equals(""))
            {
                return Ok(null);
            }
            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                       _configuration["FilePath:test"]!, "SureliIslerTakip\\");
            if (filePath == null)
            {
                return Ok(null);
            }
            else
            {
                ResponseByteArray response = FileConverter.convertByte(filePath, file, file);
                return Ok(response);
            }
        }
        #endregion

    }

}
