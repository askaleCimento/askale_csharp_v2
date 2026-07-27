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
        public async Task<ActionResult<AdminUserSaveDto>> save([FromForm] AdminUserSaveDto adminUserDto)
        {
            if (adminUserDto.id == null)
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser adminUser = _mapper.Map<AdminUser>(adminUserDto);

                adminUser.password = BCrypt.Net.BCrypt.EnhancedHashPassword(adminUserDto.password);
                if (adminUserDto.merni != String.Empty)
                {
                    BLLActions.Personel bllPersonel = new BLLActions.Personel(_configuration, _env);
                    List<EmployeeSap>? list = bllPersonel.GetAllFromSAP(null);
                    if (adminUser != null)
                    {
                        if (list != null)
                        {
                            EmployeeSap? employee = list.Where(u => u.MERNI == adminUserDto.merni).FirstOrDefault();
                            if (employee != null)
                            {
                                DateTime dt;
                                adminUser.bankl = employee.BANKL;
                                adminUser.btext = employee.BTEXT;
                                adminUser.adrfr = employee.ADRFR;
                                adminUser.plans = employee.PLANS;
                                adminUser.numch = employee.NUMCH;
                                adminUser.name1 = employee.NAME1;
                                adminUser.stell = employee.STELL;
                                adminUser.bankn = employee.BANKN;
                                DateTime.TryParse(employee.BDATE, out dt);
                                adminUser.bdate = dt;
                                adminUser.bldgr = employee.BLDGR;
                                adminUser.brpcl = employee.BRPLC;
                                adminUser.btrtl = employee.BTRTL;
                                adminUser.cinsy = employee.CINSY;
                                adminUser.eindt = employee.EINDT;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;

                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;

                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;


                                adminUser.bankl = employee.BANKL;

                                adminUser.bankl = employee.BANKL;

                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;

                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                                adminUser.bankl = employee.BANKL;
                            }
                        }
                    }

                }

                AdminUser? returnAdminUser = await bllAdminUsers.Add(adminUser!);
                AdminUserSaveDto adminUserDto1 = _mapper.Map<AdminUserSaveDto>(returnAdminUser);
                return Ok(adminUserDto1);
            }
            else
            {
                BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                AdminUser adminUser = _mapper.Map<AdminUser>(adminUserDto);
                if (!adminUser.password.StartsWith("$2"))
                {
                    adminUser.password = BCrypt.Net.BCrypt.EnhancedHashPassword(adminUserDto.password);
                }

                AdminUser returnAdminUser = await bllAdminUsers.Update(adminUser);
                AdminUserSaveDto adminUserDto1 = _mapper.Map<AdminUserSaveDto>(returnAdminUser);
                return Ok(adminUserDto1);
            }

        }

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
