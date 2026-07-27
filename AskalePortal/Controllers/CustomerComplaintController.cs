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
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerComplaintController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public CustomerComplaintController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<CustomerComplaintSaveDto?>> save([FromForm] CustomerComplaintSaveDto entity)
        {

            BLLActions.MusteriSikayetForm bllMusteriSikayetForm = new BLLActions.MusteriSikayetForm(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity?.FindFirst("userId")?.Value ?? "0");
            }
            CustomerComplaintSaveDto dto = await bllMusteriSikayetForm.save(entity, userId);
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
                        f.moduleId = (int)CommonConstants.MODULES.MUSTERI_SIKAYET_FORM;
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

        #region filterByPageable
        [HttpPost("filterByPageable")]
        public ActionResult<PageReturn<CustomerComplaintDto>> filterPageable([FromForm] FilterPageParam<CustomerComplaintListDtoParameter> filterPageParam)
        {
            BLLActions.MusteriSikayetForm bllMusteriSikayetForm = new BLLActions.MusteriSikayetForm(_configuration, _env, _mapper);
            PageReturn<CustomerComplaintDto> page = bllMusteriSikayetForm.listByPageable(filterPageParam);
            return Ok(page);

        }
        #endregion
        #region downloadById
        [HttpPost("downloadById")]
        public ActionResult<List<ResponseByteArray>> downloadById([FromForm] int id)
        {
            List<ResponseByteArray> list = new List<ResponseByteArray>();
            BLLActions.AttachedFiles bllAttachedFiles = new BLLActions.AttachedFiles(_configuration, _env);

            List<AttachedFile> attachedFiles = bllAttachedFiles.getByModuleIdAndTargetId((int)CommonConstants.MODULES.MUSTERI_SIKAYET_FORM, id);
            foreach (AttachedFile attachedFile in attachedFiles)
            {

                string file = attachedFile.title;
                if (file.Equals(null))
                {
                    return Ok(null);
                }


                string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                       _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "documents\\");

                ResponseByteArray response = FileConverter.convertByte(filePath, file, file);


                if (response.file != null)
                {
                    list.Add(response);
                }

            }
            return Ok(list);

        }
        #endregion
        #region listCustomerComplaintAction
        [HttpPost("listCustomerComplaintAction")]
        public ActionResult<List<CustomerComplaintActionDto>> listCustomerComplaintAction([FromForm] int customerComplaintId)
        {
            BLLActions.MusteriSikayetForm bllMusteriSikayetForm = new BLLActions.MusteriSikayetForm(_configuration, _env, _mapper);
            List<CustomerComplaintActionDto> liste = bllMusteriSikayetForm.listCustomerComplaintAction(customerComplaintId);
            return Ok(liste);
        }
        #endregion

    }
}


