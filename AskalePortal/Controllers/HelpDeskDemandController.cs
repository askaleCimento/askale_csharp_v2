using AskalePortal.BLL;
using AskalePortal.Constants;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelpDeskDemandController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public HelpDeskDemandController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }

        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> Save([FromForm] HelpDeskDemandSaveDto entity)
        {
            int userId = 0;

            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");
            }

            var bllHelpDeskDemands = new BLL.BLLActions.HelpDeskDemands(_configuration, _env);
            var bllAdminUsers = new BLL.BLLActions.AdminUsers(_configuration, _env, _mapper);
            var bllCompanies = new BLL.BLLActions.Companies(_configuration, _env, _mapper);
            var bllHelpDeskDemandRules = new BLL.BLLActions.HelpDeskDemandRules(_configuration, _env);

            AdminUser? user = bllAdminUsers.GetByID(userId);
            Company? company = bllCompanies.GetByID(user?.companyId ?? 0);

            if (entity.id == null)
            {
                var rules = bllHelpDeskDemandRules
                    .findIdByCompanyAndHelpDeskCategory(company?.vkorg, entity.helpDeskCategoryId.ToString());

                entity.assignedToHelpDeskRoleId =
                    (rules == null || rules.Count == 0)
                        ? null
                        : rules.First().helpDeskRoleId;

                entity.createdByCompanyId = user?.companyId;
                entity.createdByUserName = user?.username;
                entity.createdUserId = userId;
                entity.helpDeskStatusId = 1;
                entity.isClosed = false;
                entity.ticketNumber = null;
                entity.createdDate = DateTime.Now.ToString();
                entity.enabled = true;

                var mapped = _mapper.Map<HelpDeskDemand>(entity);

                var result = await bllHelpDeskDemands.Add(mapped);

                if (result?.assignedToHelpDeskRoleId != null)
                {
                    var helpDeskUser = bllAdminUsers.findHelpDeskRoleId(result.assignedToHelpDeskRoleId.Value);

                    var emailMessage = new EmailMessage
                    {
                        subject = "Destek Masası Talep hk.",
                        toAddress = helpDeskUser.email,
                        emailText = new BLLActions.EmailReaderFile()
                            .BuildEmailTemplate(
                                _configuration,
                                _env,
                                $"Sayın {helpDeskUser.name} Destek Masası Talebi",
                                $"{result.Id} ID'li talebiniz oluşmuştur"),
                        mailTuru = 1,
                        enabled = true,
                        isSent = false,
                        plannedDate = DateTime.Now
                    };

                    var bllEmailMessages = new BLL.BLLActions.EmailMessages(_configuration, _env);
                    await bllEmailMessages.Add(emailMessage);
                }

                return result;
            }

            else
            {
                var existing = bllHelpDeskDemands.GetByIDAll(entity.id ?? 0);

                if (existing == null)
                    return NotFound();

                if (existing.helpDeskCategoryId != entity.helpDeskCategoryId)
                {
                    var rules = bllHelpDeskDemandRules
                        .findIdByCompanyAndHelpDeskCategory(company?.vkorg, entity.helpDeskCategoryId.ToString());

                    entity.assignedToHelpDeskRoleId =
                        (rules == null || rules.Count == 0)
                            ? null
                            : rules.First().helpDeskRoleId;
                }

                if (entity.helpDeskStatusId == 3)
                {
                    entity.isClosed = true;
                }

                entity.updatedUserId = userId;
                entity.updateDate = DateTime.Now.ToString();

                _mapper.Map(entity, existing);

                var updated = await bllHelpDeskDemands.Update(existing);

                return updated;
            }
        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.HelpDeskDemands bllHelpDeskDemand = new BLL.BLLActions.HelpDeskDemands(_configuration, _env);
                bllHelpDeskDemand.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
            }

        }
        #endregion

        #region getById
        [HttpPost("getById")]

        public ActionResult<object> getById([FromForm] int id)
        {
            BLL.BLLActions.HelpDeskDemands bllHelpDeskDemand = new BLL.BLLActions.HelpDeskDemands(_configuration, _env);

            HelpDeskDemand? helpDeskDemand = bllHelpDeskDemand.GetByID(id);
            if (helpDeskDemand == null)
            {
                return NotFound();
            }
            return Ok(helpDeskDemand);


        }
        #endregion

        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.HelpDeskDemands bllHelpDeskDemand = new BLL.BLLActions.HelpDeskDemands(_configuration, _env);

            List<HelpDeskDemand>? listHelpDeskDemand = bllHelpDeskDemand.GetAll();
            return Ok(listHelpDeskDemand);

        }
        #endregion

        #region getAll
        [HttpPost("numberDemandsByStatusId")]

        public ActionResult<List<int>> numberDemandsByStatusId()
        {
            BLL.BLLActions.HelpDeskDemands bllHelpDeskDemand = new BLL.BLLActions.HelpDeskDemands(_configuration, _env);

            List<int> list = bllHelpDeskDemand.NumberDemandsByStatusId();
            return Ok(list);
        }
        #endregion
       
        #region mylist
        [HttpPost("mylist")]
        public ActionResult<object> mylist([FromForm] int userId)
        {


            BLL.BLLActions.HelpDeskDemands bllHelpDeskDemand = new BLL.BLLActions.HelpDeskDemands(_configuration, _env);
            BLL.BLLActions.AdminUsers bllAdminUsers = new BLL.BLLActions.AdminUsers(_configuration, _env, _mapper);
            string username = bllAdminUsers.GetByID(userId)?.username ?? "";
            List<HelpDeskDemand> list = bllHelpDeskDemand.mylist(userId, username);


            return Ok(list);
        }
        #endregion

        #region talepYonetimiDtoList
        [HttpPost("talepYonetimiDtoList")]
        public ActionResult<List<HelpDeskDemandDto>> talepYonetimiDtoList([FromForm] Data.RequestParams.FilterParam<HelpDeskDemandParamsDto> filterParam)
        {
            BLL.BLLActions.HelpDeskDemands bllHelpDeskDemand = new BLL.BLLActions.HelpDeskDemands(_configuration, _env);
            List<HelpDeskDemandDto> liste = bllHelpDeskDemand.talepYonetimiDtoList(filterParam);
            return Ok(liste);
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
                        f.moduleId = (int)CommonConstants.MODULES.HELPDESK_DEMANDS;
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
