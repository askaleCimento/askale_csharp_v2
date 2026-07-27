using AskalePortal.BLL;
using AskalePortal.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaticiFirmalarTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        public SaticiFirmalarTableController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }
        #region findByCompanyId
        [HttpPost("findByCompanyId")]
        public ActionResult<object> findByFirmaCompanyId([FromForm] int companyId)
        {
            BLLActions.SaticiFirmalarTable bllSaticiFirmalarTable = new BLLActions.SaticiFirmalarTable(_configuration, _env);
            List<SaticiFirmalarTable> liste = bllSaticiFirmalarTable.findByCompanyId(companyId) ?? [];
            return Ok(liste);
        }
        #endregion
        #region findByFirmaAdiCompany
        [HttpPost("findByFirmaAdiCompany")]

        public ActionResult<object> findByFirmaAdiCompanyList([FromForm] string firmaAdi, [FromForm] int companyId)
        {
            BLLActions.SaticiFirmalarTable bllSaticiFirmalarTable = new BLLActions.SaticiFirmalarTable(_configuration, _env);
            List<SaticiFirmalarTable> liste = bllSaticiFirmalarTable.findByFirmaAdiCompany(firmaAdi, companyId);

            return Ok(liste);
        }
        #endregion
        #region findByFirmaKodu
        [HttpPost("findByFirmaKodu")]
        public ActionResult<object> findByFirmaKodu([FromForm] string firmaKodu, [FromForm] int companyId)
        {
            BLLActions.SaticiFirmalarTable bllSaticiFirmalarTable = new BLLActions.SaticiFirmalarTable(_configuration, _env);
            SaticiFirmalarTable? table = bllSaticiFirmalarTable.findByFirmaKodu(firmaKodu, companyId);
            return Ok(table);
        }
        #endregion
    }
}
