using AskalePortal.BLL;
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly IWebHostEnvironment _env; private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        public CompanyController(IWebHostEnvironment env, IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        [HttpPost("getAll")]
        public ActionResult<List<CompanyDto>> getAll()
        {
            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);

            List<CompanyDto> list = _mapper.Map<List<Company>, List<CompanyDto>>(bllCompanies.GetAll());
            return Ok(list);

        }
        [HttpPost("getAllNameAndId")]
        public ActionResult<List<IdandText>> getAllNameAndId()
        {
            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);

            List<IdandText> list = bllCompanies.GetIdandText();
            return Ok(list);

        }
        #region getById

        [HttpPost("getById")]

        public ActionResult<CompanyDto> getById([FromForm] int id)
        {
            BLLActions.Companies bllCompany = new BLLActions.Companies(_configuration, _env, _mapper);

            Company? company = bllCompany.GetByID(id);
            if (company == null)
            {
                return NotFound();
            }
            return Ok(company);


        }
        #endregion

        #region getByRoleId

        [HttpPost("getByRoleId")]
        public ActionResult<object> getByRoleId()
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity identity)
            {
                userId = int.Parse(identity?.FindFirst("userId")?.Value ?? "0");
            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);

            int roleId = bllAdminUsers.GetByID(userId)?.roleId ?? 0;

            BLLActions.Companies bllCompany = new BLLActions.Companies(_configuration, _env, _mapper);

            List<Company> listCompanies = bllCompany.getByRoleId(roleId);

            return Ok(listCompanies);
        }
        #endregion

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<CompanySaveDto>> save([FromForm] CompanySaveDto? entity)
        {
            if (entity == null)
            {
                return BadRequest();
            }

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");
            }

            BLLActions.Companies bllCompanies =
                new BLLActions.Companies(_configuration, _env, _mapper);

            if (entity.id != null)
            {
                entity.updateDate = DateTime.Now;
                entity.updatedUserId = userId == 0 ? null : userId;

                Company updatedCompany = await bllCompanies.Update(
                    _mapper.Map<Data.Models.Company>(entity));

                CompanySaveDto returnDto =
                    _mapper.Map<CompanySaveDto>(updatedCompany);

                return Ok(returnDto);
            }

            entity.createdDate = DateTime.Now;
            entity.createdUserId = userId == 0 ? null : userId;
            entity.enabled = true;

            Company? addedCompany = await bllCompanies.Add(
                _mapper.Map<Data.Models.Company>(entity));

            if (addedCompany == null)
            {
                return BadRequest();
            }

            CompanySaveDto addedCompanyDto =
                _mapper.Map<CompanySaveDto>(addedCompany);

            return Ok(addedCompanyDto);
        }
        #endregion

        [HttpPost("getAllFilter")]
        public ActionResult<List<CompanySaveDto>> getAllFilter([FromForm] FilterParam<CompanyFilterDto> filterParam)
        {
            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);

            List<CompanySaveDto> list = bllCompanies.getAllFilter(filterParam);
            return Ok(list);

        }

        #region upload
        [HttpPost]
        [Route("upload")]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> upload()
        {
            IFormFileCollection files = Request.Form.Files;

            if (!int.TryParse(Request.Form["targetId"].ToString(), out int targetId))
            {
                return BadRequest("targetId geçersiz veya boş.");
            }

            if (files.Count == 0)
            {
                return BadRequest("Yüklenecek dosya bulunamadı.");
            }

            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
            Company? company = bllCompanies.GetByID(targetId);

            if (company == null)
            {
                return NotFound($"Company bulunamadı. Id: {targetId}");
            }

            string basePath = _env.IsDevelopment()
                ? _configuration["FilePath:local"]!
                : _env.IsProduction()
                    ? _configuration["FilePath:server"]!
                    : _configuration["FilePath:test"]!;

            string directoryPath = Path.Combine(basePath, "documents", "company");
            Directory.CreateDirectory(directoryPath);

            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");
            }

            BLLActions.AttachedFiles bllAttachedFiles =
                new BLLActions.AttachedFiles(_configuration, _env);

            long size = files.Sum(f => f.Length);
            int uploadedCount = 0;

            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                {
                    continue;
                }

                string originalFileName = Path.GetFileName(formFile.FileName);
                string fileName =
                    Path.GetFileNameWithoutExtension(originalFileName) + "-" +
                    DateTimeOffset.Now.ToUnixTimeMilliseconds() +
                    Path.GetExtension(originalFileName);

                string fileFull = Path.Combine(directoryPath, fileName);

                using (var stream = System.IO.File.Create(fileFull))
                {
                    await formFile.CopyToAsync(stream);
                }

                AttachedFile attachedFile = new AttachedFile
                {
                    moduleId = (int)CommonConstants.MODULES.SIRKETLER,
                    targetId = targetId,
                    createdUserId = userId,
                    title = originalFileName,
                    filePath = fileName,
                    createdDate = DateTime.Now,
                    enabled = true
                };

                await bllAttachedFiles.Add(attachedFile);
                uploadedCount++;
            }

            return Ok(new { count = uploadedCount, size });
        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
                bllCompanies.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion
        #region getFromSirketler
        [HttpPost("getFromSirketler")]

        public ActionResult<List<CompanySaveDto>> getFromSirketler()
        {
            BLLActions.Companies bllCompanies = new BLLActions.Companies(_configuration, _env, _mapper);
            List<CompanySaveDto>? listCompanies = bllCompanies.GetAllFromSAP();

            return Ok(listCompanies);
        }
        #endregion
    }
}
