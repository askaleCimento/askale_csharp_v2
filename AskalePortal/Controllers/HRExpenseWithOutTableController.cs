using AskalePortal.BLL;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HRExpenseWithOutTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRExpenseWithOutTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);

            HRExpenseWithOutTable? hrExpenseWithOutTable = bllHRExpenseWithOutTable.GetByID(id);
            if (hrExpenseWithOutTable == null)
            {
                return NotFound();
            }
            return Ok(hrExpenseWithOutTable);


        }
        #endregion


        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);

            int deger = bllHRExpenseWithOutTable.approvalCount(userId);

            return Ok(deger);
        }
        #endregion

        #region save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HRExpenseWithOutTableSaveDto entity)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            HRExpenseWithOutTable hrExpenseWithOutTable = await bllHRExpenseWithOutTable.save(entity, userId);

            return Ok(hrExpenseWithOutTable);
        }
        #endregion

        #region getByTripId
        [HttpPost("getByTripId")]
        public ActionResult<object> getByTripId([FromForm] int tripId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            List<HRExpenseWithOutTable> liste = bllHRExpenseWithOutTable.listByTripId(tripId);
            return Ok(liste);
        }
        #endregion

        #region myListExpense
        [HttpPost("myListExpense")]

        public ActionResult<List<HRExpenseDto>> myListExpense([FromForm] int tripId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            List<HRExpenseDto> dto = bllHRExpenseWithOutTable.mylistExpense(tripId);
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
            string gelenId = Request.Form["id"].ToString();
            string dataFormat = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");

            string extension = Path.GetExtension(file);

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);

            string newFileName = $"{fileNameWithoutExt}_{gelenId}_{dataFormat}{extension}";
            long size = files.Sum(f => f.Length);

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {

                    string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! :
                        _configuration["FilePath:test"]!, "HrExpenseFiles\\");
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

                        BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);

                        HRExpenseWithOutTable? hrExpenseWithOutTable = bllHRExpenseWithOutTable.GetByID(int.Parse(gelenId));
                        if (hrExpenseWithOutTable != null)
                        {
                            hrExpenseWithOutTable.fileNames = fileName;
                            await bllHRExpenseWithOutTable.Update(hrExpenseWithOutTable);
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
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "HrExpenseFiles\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion

        #region confirmAll
        [HttpPost("confirmAll")]
        public async Task<ActionResult<int>> confirmAll([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);

            int donenDeger = await bllHRExpenseWithOutTable.confirmAll(tripId, userId);
            return Ok(donenDeger);
        }
        #endregion

        #region changeLimit
        [HttpPost("changeLimit")]
        public async Task<ActionResult<HRExpenseWithOutTableSaveDto>> changeLimit([FromForm] HRExpenseWithOutTableSaveDto entity)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            HRExpenseWithOutTableSaveDto save = await bllHRExpenseWithOutTable.changeLimit(entity);
            return Ok(save);
        }
        #endregion

        #region ceoOnayLimitTutari
        [HttpPost("ceoOnayLimitTutari")]
        public async Task<ActionResult<int>> ceoOnayLimitTutari([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseWithOutTable.ceoOnayLimitTutari(tripId, userId);
            return Ok(deger);
        }
        #endregion

        #region ceoOnayFaturaTutari
        [HttpPost("ceoOnayFaturaTutari")]
        public async Task<ActionResult<int>> ceoOnayFaturaTutari([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseWithOutTable.ceoOnayFaturaTutari(tripId, userId);
            return Ok(deger);
        }
        #endregion

        #region ceoOnayAmirOnayi
        [HttpPost("ceoOnayAmirOnayi")]
        public async Task<ActionResult<int>> ceoOnayAmirOnayi([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseWithOutTable.ceoOnayAmirOnayi(tripId, userId);
            return Ok(deger);
        }

        #endregion

        #region geriGonder
        [HttpPost("geriGonder")]
        public async Task<ActionResult<int>> geriGonder([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseWithOutTable bllHRExpenseWithOutTable = new BLLActions.HRExpenseWithOutTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseWithOutTable.geriGonder(tripId, userId);
            return Ok(deger);
        }
        #endregion

    }
}
