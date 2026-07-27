using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.OutputParams;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using System.Collections.Generic;
using System.Security.Claims;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public CustomerController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region getCustomers
        [HttpPost("getCustomers")]
        public ActionResult<List<CustomerListDto>> getCustomers()
        {
            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);
            List<CustomerListDto> liste = bllCustomers.GetAllFromSAP()??[];
            return Ok(liste);
        }
        #endregion

        #region getCustomersSikayet
        [HttpPost("getCustomersSikayet")]
        public ActionResult<CustomerSikayetList[]> getCustomersSikayet([FromForm] string bukrs)
        {
            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);
            CustomerSikayetList[] liste = bllCustomers.getCustomerSikayet(bukrs);
            return Ok(liste);
        }
        #endregion

        #region getcustomer
        [HttpPost("getcustomer")]
        public ActionResult<CustomerCreditSap> getCustomer([FromForm] string kunnr)
        {
            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);

            return Ok(bllCustomers.getCustomerCredit(kunnr));
        }
        #endregion
        #region getCustomerDocument
        [HttpPost("getCustomerDocument")]

        public ActionResult<List<CustomerDocumentDto>> getCustomerDocument([FromForm] string kunnr)
        {
            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);

            return Ok(bllCustomers.getCustomerDocument(kunnr));
        }
        #endregion

        #region getFiyatOnayi
        [HttpPost("getFiyatOnayi")]
        public ActionResult<List<FiyatOnayiList>> getFiyatOnayi()
        {
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
            AdminUser user = bllAdminUsers.GetByID(userId)!;
            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);

            return Ok(bllCustomers.getMyFiyatList(user.sapUser));
        }
        #endregion

        #region setFiyatOnayi
        [HttpPost("setFiyatOnayi")]
        public ActionResult<string> setFiyatOnayi([FromForm] int wiid, [FromForm] int onay)
        {
            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);

            return Ok(bllCustomers.setFiyatOnayi(wiid, onay));
        }
        #endregion

    }
}
