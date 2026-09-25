using AskalePortal.BLL;
using AskalePortal.Data.Models;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.SAP.Models;
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
        public ActionResult<CustomerCreditList> getCustomer([FromForm] string kunnr)
        {
            BLLActions.Customers bllCustomers = new BLLActions.Customers(_configuration, _env);
            CustomerCreditList? data = bllCustomers.getCustomerCredit(kunnr);
            if (data == null)
            {
                return NotFound(new { message = "Müşteri kredi bilgisi alınamadı." });
            }

            // Explicit keys survive DetachedEntityResultFilter and Newtonsoft.Json.
            return Ok(new Dictionary<string, object?>
            {
                ["kunnr"] = data.KUNNR,
                ["name1"] = data.NAME1,
                ["dmbtr"] = data.DMBTR,
                ["dmbtr120"] = data.DMBTR_120,
                ["dmbtra"] = data.DMBTR_A,
                ["dmbtrg"] = data.DMBTR_G,
                ["riskborc"] = data.RISK_BORC,
                ["dmbtrt"] = data.DMBTR_T,
                ["acikcekm"] = data.ACIK_CEK_M,
                ["acikcekk"] = data.ACIK_CEK_K,
                ["aciksd"] = data.ACIK_SD,
                ["topborc"] = data.TOP_BORC,
                ["aciksenetm"] = data.ACIK_SENET_M,
                ["aciksenetk"] = data.ACIK_SENET_K,
                ["kredikul"] = data.KREDI_KUL,
                ["klimk"] = data.KLIMK,
                ["dmbtrvade1"] = data.DMBTR_VADE1,
                ["dmbtrvade2"] = data.DMBTR_VADE2,
                ["dmbtrvade3"] = data.DMBTR_VADE3,
                ["dmbtrvade4"] = data.DMBTR_VADE4,
                ["dmbtrvade5"] = data.DMBTR_VADE5,
                ["dmbtr2vade1"] = data.DMBTR2_VADE1,
                ["dmbtr2vade2"] = data.DMBTR2_VADE2,
                ["dmbtr2vade3"] = data.DMBTR2_VADE3,
                ["dmbtr2vade4"] = data.DMBTR2_VADE4,
                ["dmbtr2vade5"] = data.DMBTR2_VADE5,
                ["dmbtrt901"] = data.DMBTR_T_901,
                ["dmbtrt902"] = data.DMBTR_T_902,
                ["snlmt"] = data.SNLMT,
                ["kllmt"] = data.KLLMT,
            });
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
