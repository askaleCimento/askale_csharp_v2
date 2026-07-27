using AskalePortal.Data.Contracts.Detached;
﻿using AskalePortal.BLL;
using AskalePortal.Constants;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IcYazismalarController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public IcYazismalarController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> approvalCount([FromForm] int userId)
        {
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env,_mapper);
            int count = bllIcYazismalarTable.approvalCount(userId);
            return Ok(count);
        }
        #endregion

        #region getById
        [HttpPost("getById")]
        public ActionResult<IcYazismalarTableSaveDto> getById([FromForm] int id)
        {
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env, _mapper);

            Data.Models.IcYazismalarTable? icYazismalarTable = bllIcYazismalarTable.GetByID(id);
            if (icYazismalarTable == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IcYazismalarTableSaveDto>(icYazismalarTable));


        }
        #endregion


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<IcYazismalarTableSaveDto?>> save([FromForm] IcYazismalarTableSaveDto? entity)
        {

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env, _mapper);
            if (entity != null)
            {
                IcYazismalarTableSaveDto? save = await bllIcYazismalarTable.save(entity!, userId);
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
        public ActionResult<PageReturn<IcYazismaTableDto>> list([FromForm] FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
        {
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env, _mapper);
            PageReturn<IcYazismaTableDto> page = bllIcYazismalarTable.list(filterPageParam);
            return Ok(page);
        }
        #endregion


        #region getDetail
        [HttpPost("getDetail")]
        public ActionResult<IcYazismaDetayDto> getDetail([FromForm] IcYazismaTableDto icYazismaDto)
        {
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            IcYazismaDetayDto detay = bllIcYazismalarTable.getDetail(icYazismaDto, userId);
            return Ok(detay);
        }
        #endregion

        #region mylist
        [HttpPost("mylist")]
        public ActionResult<PageReturn<IcYazismaTableDto>> mylist([FromForm] FilterPageParam<InternalCorrespondencePageableListBilgiDtoParameter> filterPageParam)
        {
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env, _mapper);
            PageReturn<IcYazismaTableDto> page = bllIcYazismalarTable.mylist(filterPageParam);
            return Ok(page);
        }
        #endregion

        #region approve
        [HttpPost("approve")]
        public async Task<ActionResult<int>> approved([FromForm] IcYazismaResponseMyList responseMyList)
        {
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllIcYazismalarTable.approve(responseMyList,userId);
            return Ok(returnInteger);
        }
        #endregion

        #region red
        [HttpPost("red")]
        public async Task<ActionResult<int>> red([FromForm] IcYazismalarTableDto request)
        {
            IcYazismalarTable icYazismaTable = request.ToEntity<IcYazismalarTable>();
            BLLActions.IcYazismalarTable bllIcYazismalarTable = new BLLActions.IcYazismalarTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            int returnInteger = await bllIcYazismalarTable.red(icYazismaTable, userId);
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
                        f.moduleId = (int)CommonConstants.MODULES.ICYAZISMA;
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

    }
}
