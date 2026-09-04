using AskalePortal.BLL;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.ReportDataset;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.Reporting.NETCore;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Data;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepresentativeExpenseTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public RepresentativeExpenseTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region getById
        [HttpPost("getById")]
        public ActionResult<object> getById([FromForm] int id)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);

            RepresentativeExpenseTable? representativeExpenseTable = bllRepresentativeExpenseTable.GetByID(id);
            if (representativeExpenseTable == null)
            {
                return NotFound();
            }
            return Ok(representativeExpenseTable);


        }
        #endregion
      
        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);

            int deger = bllRepresentativeExpenseTable.myApprovalCount(userId);

            return Ok(deger);
        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] RepresentativeExpenseTableSaveDto? entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);
                RepresentativeExpenseTable table = await bllRepresentativeExpenseTable.Save(entity, userId);
                return Ok(table);
            }
            else
            {
                return Ok(null);
            }



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
                        _configuration["FilePath:test"]!, "RepresentativeExpenseFiles\\");
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
                        BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);

                        RepresentativeExpenseTable? representativeExpense = bllRepresentativeExpenseTable.GetByID(int.Parse(gelenId));
                        if (representativeExpense != null)
                        {
                            representativeExpense.fileNames = fileName;
                            await bllRepresentativeExpenseTable.Update(representativeExpense);
                        }


                    }

                }

            }

            return Ok(new { count = 1, size });

        }
        #endregion

        #region active list
        [HttpPost("active")]
        public ActionResult<PageReturn<RepresentativeExpenseTableSaveDto>> active([FromForm] FilterPageParam<RepresentativeExpenseTableDtoParameter> filterPageParam)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);

            PageReturn<RepresentativeExpenseTableSaveDto> page = bllRepresentativeExpenseTable.listByUserIdActive(filterPageParam);
            return Ok(page);
        }
        #endregion

        #region download
        [HttpPost("download")]
        public ActionResult<ResponseByteArray> download([FromForm] string file)
        {

            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "RepresentativeExpenseFiles\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion

        #region active my approval list
        [HttpPost("activemyapprovallist")]
        public ActionResult<PageReturn<RepresentativeExpenseTableSaveDto>> activeMyApprovalList([FromForm] FilterPageParam<RepresentativeExpenseTableDtoParameter> filterPageParam)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);

            PageReturn<RepresentativeExpenseTableSaveDto> page = bllRepresentativeExpenseTable.activeMyApprovalList(filterPageParam);
            return Ok(page);
        }

        #endregion

        #region completed list
        [HttpPost("completed")]
        public ActionResult<PageReturn<RepresentativeExpenseTableSaveDto>> listCompleted([FromForm] FilterPageParam<RepresentativeExpenseTableDtoParameter> filterPageParam)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);

            PageReturn<RepresentativeExpenseTableSaveDto> page = bllRepresentativeExpenseTable.listCompleted(filterPageParam);
            return Ok(page);
        }

        #endregion

        #region reject
        [HttpPost("reject")]

        public async Task<ActionResult<int>> reject([FromForm] int repId, [FromForm] int userId)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);
            int deger = await bllRepresentativeExpenseTable.reject(repId, userId);
            return Ok(deger);
        }
        #endregion

        #region confirm
        [HttpPost("confirm")]

        public async Task<ActionResult<int>> confirm([FromForm] int repId, [FromForm] int userId)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable = new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);
            int deger = await bllRepresentativeExpenseTable.confirm(repId, userId);
            return Ok(deger);
        }

        #endregion

        #region showPdf
        [HttpPost("showPdf")]
        public ActionResult<ResponseByteArray> showPdf([FromForm] int repId)
        {
            BLLActions.RepresentativeExpenseTable bllRepresentativeExpenseTable =
                new BLLActions.RepresentativeExpenseTable(_configuration, _env, _mapper);

            byte[] pdfBytes = bllRepresentativeExpenseTable.CreatePdf(repId);

            ResponseByteArray responseByteArray = new ResponseByteArray
            {
                file = pdfBytes,
                fileName = $"TemsiliHarcama_{repId}.pdf"
            };

            return Ok(responseByteArray);
        }
        #endregion
    }
}
