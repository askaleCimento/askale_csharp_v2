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
using System.Collections.Generic;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class HRExpenseTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public HRExpenseTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);

            HRExpenseTable? hrExpenseTable = bllHRExpenseTable.GetByID(id);
            if (hrExpenseTable == null)
            {
                return NotFound();
            }
            return Ok(hrExpenseTable);
        }
        #endregion
        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);

            int deger = bllHRExpenseTable.approvalCount(userId);

            return Ok(deger);
        }
        #endregion
        #region getByTripId
        [HttpPost("getByTripId")]
        public ActionResult<object> getByTripId([FromForm] int tripId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            List<HRExpenseTable> liste = bllHRExpenseTable.listByTripId(tripId);
            return Ok(liste);

        }
        #endregion
        #region save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] HRExpenseTableSaveDto entity)
        {

            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            HRExpenseTable expensetable = await bllHRExpenseTable.save(entity, userId);

            return Ok(expensetable);
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

                        BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);

                        HRExpenseTable? hrExpenseTable = bllHRExpenseTable.GetByID(int.Parse(gelenId));
                        if (hrExpenseTable != null)
                        {
                            hrExpenseTable.fileNames = fileName;
                            await bllHRExpenseTable.Update(hrExpenseTable);
                        }


                    }

                }

            }

            return Ok(new { count = 1, size });

        }
        #endregion

        #region myListdto
        [HttpPost("myListdto")]
        public ActionResult<PageReturn<HRExpenseTripDto>> myList([FromForm] FilterPageParam<HRExpenseTripTableMyListDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripDto> page = bllHRExpenseTable.mylist(filterPageParam);
            return Ok(page);
        }
        #endregion
        #region myListExpense
        [HttpPost("myListExpense")]

        public ActionResult<List<HRExpenseDto>> myListExpense([FromForm] int tripId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            List<HRExpenseDto> liste = bllHRExpenseTable.mylistExpense(tripId);
            return Ok(liste);


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
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseTable.confirmAll(tripId, userId);
            return Ok(deger);
        }
        #endregion
        #region changeLimit
        [HttpPost("changeLimit")]
        public async Task<ActionResult<HRExpenseTableSaveDto>> changeLimit([FromForm] HRExpenseTableSaveDto entity)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            HRExpenseTableSaveDto saveModel = await bllHRExpenseTable.changeLimit(entity);
            return Ok(saveModel);
        }
        #endregion
        #region rejectAll
        [HttpPost("rejectAll")]
        public async Task<ActionResult<int>> rejectAll([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            int yanit = await bllHRExpenseTable.rejectAll(tripId, userId);
            return Ok(yanit);
        }
        #endregion
        #region geriGonder
        [HttpPost("geriGonder")]
        public async Task<ActionResult<int>> geriGonder([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            int yanit = await bllHRExpenseTable.geriGonder(tripId, userId);

            return Ok(yanit);
        }
        #endregion
        #region active
        [HttpPost("active")]
        public ActionResult<List<HRExpenseTableSaveDto>> active([FromForm] FilterPageParam<HRExpenseTableActiveListDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);

            List<HRExpenseTableSaveDto> page = bllHRExpenseTable.listActive(filterPageParam);
            return Ok(page);
        }
        #endregion
        #region myListApprovealStatusdto
        [HttpPost("myListApprovealStatusdto")]
        public ActionResult<PageReturn<HRExpenseTripDto>> myListAprovalStatus([FromForm] FilterPageParam<HRExpenseTableApprovalStatusDtoParameter> filterPageParam)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            PageReturn<HRExpenseTripDto> page = bllHRExpenseTable.mylistAprovalStatus(filterPageParam);
            return Ok(page);
        }
        #endregion
        #region ceoOnayLimitTutari
        [HttpPost("ceoOnayLimitTutari")]
        public async Task<ActionResult<int>> ceoOnayLimitTutari([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            int deger =await bllHRExpenseTable.ceoOnayLimitTutari(tripId, userId);
            return Ok(deger);
        }
        #endregion

        #region ceoOnayFaturaTutari
        [HttpPost("ceoOnayFaturaTutari")]
        public async Task<ActionResult<int>> ceoOnayFaturaTutari([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseTable.ceoOnayFaturaTutari(tripId, userId);
            return Ok(deger);
        }
        #endregion

        #region ceoOnayAmirOnayi
        [HttpPost("ceoOnayAmirOnayi")]
        public async Task<ActionResult<int>> ceoOnayAmirOnayi([FromForm] int tripId, [FromForm] int userId)
        {
            BLLActions.HRExpenseTable bllHRExpenseTable = new BLLActions.HRExpenseTable(_configuration, _env, _mapper);
            int deger = await bllHRExpenseTable.ceoOnayAmirOnayi(tripId, userId);
            return Ok(deger);
        }

        #endregion

    }
}
