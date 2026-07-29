using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AskalePortal.Data.SAP.Models;
using AutoMapper;
using AskalePortal.API.Security;
using AskalePortal.Data.Functions;

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminUserController : ControllerBase
    {

        private readonly IConfiguration _configuration; private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;


        public AdminUserController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        [HttpPost("login")]
        public ActionResult<string> login([FromForm] string? username, [FromForm] string? password, [FromForm] string? ip)
        {

            Token token = TokenHandler.CreateToken(_configuration, _env, _mapper, username, password, ip);

            return Ok(token.AccessToken);
        }

        [HttpPost("getById")]
        public ActionResult<AdminUserSaveDto> getById([FromForm] int id)
        {

            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUserSaveDto adminUser = _mapper.Map<AdminUserSaveDto>(bllAdminUsers.GetByID(id));

            return Ok(adminUser);
        }

        [HttpPost("changepassword")]
        public async Task<ActionResult<int>> changepassword([FromForm] int userId, [FromForm] string newPassword)
        {

            BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? adminUser = bllAdminUser.GetByID(userId);
            if (adminUser != null)
            {
                string bcryptpassword = BCrypt.Net.BCrypt.HashPassword(newPassword);
                adminUser.password = bcryptpassword;
                try
                {
                    await bllAdminUser.Update(adminUser);
                    return 1;
                }
                catch (Exception) { return 0; }
            }

            return 0;

        }


        [HttpPost("filterPageableDto")]
        public ActionResult<PageReturn<UsersFilterDto>?> filterPageableDto([FromForm] FilterPageParam<UserFilterDtoRequest> filterPageParam)
        {

            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            PageReturn<UsersFilterDto>? liste = bllAdminUsers.FilterPageableDto(filterPageParam);
            return Ok(liste);
        }


        [HttpGet("downloadPicture/{userId}")]
        public ActionResult<ResponseByteArray> downloadPicture(int userId)
        {
            BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            ResponseByteArray responseByteArray = new();
            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "adminusers\\images\\");


            Data.Models.AdminUser? user = bllAdminUser.GetByID(userId);
            if (user != null)
            {

                IntegerAndResponseByteArrayDto dto = new();
                dto.userId = userId;
                responseByteArray = FileConverter.convertByte(filePath, user.imageUrl, user.name);
                responseByteArray.name = user.name;
            }

            return Ok(responseByteArray);

        }
        [HttpPost("filterPassivePageableDto")]
        public ActionResult<PageReturn<AdminUserDto>> listPassivePageableDto([FromForm] FilterPageParam<UserFilterDtoRequest> filterPageParam)
        {
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);

            PageReturn<AdminUserDto> page = bllAdminUsers.listPassivePageableDto(filterPageParam);
            return Ok(page);
        }

        [HttpPost("save")]
        public async Task<ActionResult<AdminUserSaveDto>> save([FromForm] AdminUserSaveDto entity)
        {
            if (entity.id == null)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser adminUser = _mapper.Map<AdminUser>(entity);

                adminUser.password = BCrypt.Net.BCrypt.EnhancedHashPassword(entity.password);

                if (!string.IsNullOrEmpty(entity.merni))
                {
                    BLLActions.Personel bllPersonel = new BLLActions.Personel(_configuration, _env);
                    List<EmployeeSap>? list = bllPersonel.GetAllFromSAP(null);
                    EmployeeSap? employee = list?.FirstOrDefault(u => u.MERNI == entity.merni);

                    if (employee != null)
                    {
                        // SAP personel alanlarini AdminUser entity alanlarina aktar.
                        adminUser.merni = employee.MERNI ?? entity.merni;
                        adminUser.mandt = employee.MANDT;
                        adminUser.pernr = employee.PERNR;
                        adminUser.perNo = employee.PERNR;
                        adminUser.ename = employee.ENAME;
                        adminUser.werks = employee.WERKS;
                        adminUser.name1 = employee.NAME1;
                        adminUser.btrtl = employee.BTRTL;
                        adminUser.btext = employee.BTEXT;
                        adminUser.persg = employee.PERSG;
                        adminUser.pgtxt = employee.PGTXT;
                        adminUser.persk = employee.PERSK;
                        adminUser.pktxt = employee.PKTXT;
                        adminUser.orgeh = employee.ORGEH;
                        adminUser.orgtx = employee.ORGTX;
                        adminUser.plans = employee.PLANS;
                        adminUser.plstx = employee.PLSTX;
                        adminUser.stell = employee.STELL;
                        adminUser.stltx = employee.STLTX;
                        adminUser.kostl = employee.KOSTL;
                        adminUser.cinsy = employee.CINSY;
                        adminUser.sstxt = employee.SSTXT;
                        adminUser.waers = employee.WAERS;
                        adminUser.schem = employee.SCHEM;
                        adminUser.bankl = employee.BANKL;
                        adminUser.bankn = employee.BANKN;
                        adminUser.iban = employee.IBAN;
                        adminUser.slstext = employee.SL_STEXT;
                        adminUser.kidem = employee.KIDEM;
                        adminUser.eindt = employee.EINDT;
                        adminUser.bldgr = employee.BLDGR;
                        adminUser.mrsta = employee.MRSTA;
                        adminUser.numch = employee.NUMCH;
                        adminUser.brpcl = employee.BRPLC;
                        adminUser.adrfr = employee.ADRFR;
                        adminUser.stat2 = employee.STAT2;
                        adminUser.statx = employee.STATX;

                        adminUser.bdate = DateTime.TryParse(employee.BDATE, out DateTime birthDate)
                            ? birthDate
                            : null;

                        adminUser.fredk = DateTime.TryParse(employee.FREDT, out DateTime fredDate)
                            ? fredDate
                            : null;

                        if (!string.IsNullOrWhiteSpace(employee.SYSUNAME))
                        {
                            adminUser.sapUser = employee.SYSUNAME;
                        }
                    }
                }

                AdminUser? returnAdminUser = await bllAdminUsers.Add(adminUser);
                AdminUserSaveDto adminUserDto1 = _mapper.Map<AdminUserSaveDto>(returnAdminUser);
                return Ok(adminUserDto1);
            }
            else
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser adminUser = _mapper.Map<AdminUser>(entity);

                if (!adminUser.password.StartsWith("$2"))
                {
                    adminUser.password = BCrypt.Net.BCrypt.EnhancedHashPassword(entity.password);
                }

                AdminUser returnAdminUser = await bllAdminUsers.Update(adminUser);
                AdminUserSaveDto adminUserDto1 = _mapper.Map<AdminUserSaveDto>(returnAdminUser);
                return Ok(adminUserDto1);
            }
        }

        #region delete
        [HttpPost("delete")]
        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                bllAdminUsers.Delete(id);
                return Ok(1);
            }
            catch
            {
                return Ok(0);
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

            if (!int.TryParse(Request.Form["targetId"].ToString(), out int targetId))
            {
                return BadRequest("targetId geçersiz veya boş.");
            }

            if (files.Count == 0)
            {
                return BadRequest("Yüklenecek dosya bulunamadı.");
            }

            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? adminUser = bllAdminUsers.GetByID(targetId);

            if (adminUser == null)
            {
                return NotFound($"AdminUser bulunamadı. Id: {targetId}");
            }

            string filePath = Path.Combine(
                _env.IsDevelopment()
                    ? _configuration["FilePath:local"]!
                    : _env.IsProduction()
                        ? _configuration["FilePath:server"]!
                        : _configuration["FilePath:test"]!,
                "adminusers\\images\\");

            Directory.CreateDirectory(filePath);

            long size = files.Sum(f => f.Length);
            string? lastUploadedFileName = null;

            foreach (var formFile in files)
            {
                if (formFile.Length <= 0)
                {
                    continue;
                }

                string originalFileName = Path.GetFileName(formFile.FileName);
                string fileName =
                    Path.GetFileNameWithoutExtension(originalFileName) +
                    "-" +
                    DateTimeOffset.Now.ToUnixTimeMilliseconds() +
                    Path.GetExtension(originalFileName);

                string fileFull = Path.Combine(filePath, fileName);

                using (var stream = System.IO.File.Create(fileFull))
                {
                    await formFile.CopyToAsync(stream);
                }

                lastUploadedFileName = fileName;
            }

            // Profil resmi AdminUser.imageUrl alanından okunuyor.
            // Bu nedenle yüklenen dosyanın adını doğrudan kullanıcı kaydına yazıyoruz.
            if (!string.IsNullOrEmpty(lastUploadedFileName))
            {
                adminUser.imageUrl = lastUploadedFileName;
                await bllAdminUsers.Update(adminUser);
            }

            return Ok(new { count = files.Count, size });
        }
        #endregion

        [HttpPost("getAll")]
        public ActionResult<List<AdminUserSaveDto>> getAll()
        {
            BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);

            List<AdminUserSaveDto> list = _mapper.Map<List<AdminUser>, List<AdminUserSaveDto>>(bllAdminUser.GetAll());
            return Ok(list);

        }
        [HttpPost("getAllNameAndId")]
        public ActionResult<List<IdandText>> getAllNameAndId()
        {
            BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);

            List<IdandText> list = bllAdminUser.GetIdandText();
            return Ok(list);

        }

        [HttpPost("getUserByNameEMailDto")]
        public ActionResult<UserByNameEMailDto> getUserByNameEMailDto([FromForm] int id)
        {
            BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);

            UserByNameEMailDto dto = bllAdminUser.getUserByNameEMailDto(id);
            return Ok(dto);
        }

        [HttpPost("getUserByNameEMailDtoAll")]
        public ActionResult<List<UserByNameEMailDto>> getUserByNameEMailDtoAll()
        {
            BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);

            List<UserByNameEMailDto> dto = bllAdminUser.getUserByNameEMailDtoAll();
            return Ok(dto);
        }

        [HttpPost("downloadPictureAll")]
        public ActionResult<List<IntegerAndResponseByteArrayDto>> downloadPictureAll([FromForm] List<int> listId)
        {
            List<IntegerAndResponseByteArrayDto> usersPictureDtos = new List<IntegerAndResponseByteArrayDto>();
            foreach (int userId in listId)
            {
                IntegerAndResponseByteArrayDto dto = new IntegerAndResponseByteArrayDto();
                dto.userId = userId;
                BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser? user = bllAdminUser.GetByID(userId);
                string? filename = user?.imageUrl;
                if (filename == null)
                {
                    return Ok(null);
                }
                string directoryName = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ? _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "adminusers\\images\\");

                ResponseByteArray response = FileConverter.convertByte(directoryName, filename, user?.name ?? "");
                dto.responseByteArray = response;
                usersPictureDtos.Add(dto);
            }

            return Ok(usersPictureDtos);
        }


        [HttpPost("listAllUser")]
        public ActionResult<List<AdminUserSaveDto>> listUser()
        {
            BLLActions.AdminUsers bllAdminUser = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            List<AdminUserSaveDto> list = _mapper.Map<List<AdminUser>, List<AdminUserSaveDto>>(bllAdminUser.listAllUser());


            return Ok(list);

        }

        [HttpPost("hrUserList")]
        public ActionResult<PageReturn<HrUserDto>> hrUserList([FromForm] FilterPageParam<HRUserListDtoParameter> filterPageParam)
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            PageReturn<HrUserDto> page = bllAdminUsers.hrUserList(filterPageParam, userId);
            return Ok(page);
        }
        [HttpPost("saveHRUsers")]
        public async Task<ActionResult<object>> saveHRUsers([FromForm] AdminUserSaveDto user, [FromForm] int userId)
        {
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser? saveUser = await bllAdminUsers.saveHRUser(user, userId);
            return Ok(saveUser);

        }

    }
}
