using AskalePortal.BLL;
using AskalePortal.Constants;
using AskalePortal.Data.Models;
using AskalePortal.Data.ReportDataset;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using AskalePortal.Data.Functions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Reporting.NETCore;
using System.Collections;
using System.Data;
using System.Security.Claims;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AracTalepTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        public AracTalepTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }


        #region Save
        [HttpPost("save")]
        public async Task<ActionResult<object>> save([FromForm] AracTalepTableSaveDto? entity)
        {

            if (entity == null)
            {
                return BadRequest();
            }
            else
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);

                if (entity?.id != null)
                {
                    entity.enabled = true;
                    entity!.updateDate = DateTime.Now.ToString();
                    entity.updatedUserId = userId == 0 ? null : userId;
                    AracTalepTable aracTalepTable = _mapper.Map<AracTalepTable>(entity);

                    await bllAracTalepTable.Update(aracTalepTable);
                    return Ok(entity);
                }
                else
                {
                    entity.createdDate = DateTime.Now.ToString();
                    entity.createdUserId = userId == 0 ? null : userId; ;
                    entity.enabled = true;

                    entity.currentUserId = userId;
                    entity.onaySirasi = 0;
                    entity.currentStateId = 1;

                    AracTalepTable aracTalepTable = _mapper.Map<AracTalepTable>(entity);

                    AracTalepTable? savedAracTalepTable =await bllAracTalepTable.Add(aracTalepTable);

                    BLLActions.AdminUsers bllAdminUsers = new BLLActions.AdminUsers(_configuration, _env, _mapper);
                    AdminUser? createdUser = bllAdminUsers.GetByID(userId);

                    AracTalepTableDetail aracTalepTableDetail = new AracTalepTableDetail();
                    aracTalepTableDetail.enabled = (true);
                    aracTalepTableDetail.approved = (null);
                    aracTalepTableDetail.isReplied = (false);
                    aracTalepTableDetail.talepId = savedAracTalepTable.Id;
                    aracTalepTableDetail.userId = (userId);
                    aracTalepTableDetail.guid = Guid.NewGuid();
                    aracTalepTableDetail.createdDate = (DateTime.Now);

                    BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration, _env);
                    await bllAracTalepTableDetail.Add(aracTalepTableDetail);

                    BLLActions.EmailMessages bllEmailMessages = new BLLActions.EmailMessages(_configuration, _env);
                    EmailMessage emailMessage = new EmailMessage();
                    emailMessage.subject = ("Araç Talebi hk.");
                    emailMessage.toAddress = (createdUser?.email);

                    BLLActions.EmailReaderFile bllEmailReaderFile = new BLLActions.EmailReaderFile();
                    string mailMessage = bllEmailReaderFile.BuildEmailTemplate(_configuration, _env, "Sayın " + createdUser?.name +
                        " Araç Talebi hk.",
                                savedAracTalepTable.Id.ToString() + " ID'li talep onayınızı beklemektedir");
                    emailMessage.emailText = (mailMessage);
                    emailMessage.mailTuru = (1);
                    emailMessage.enabled = (true);
                    emailMessage.isSent = (false);
                    emailMessage.plannedDate = (DateTime.Now);
                    await bllEmailMessages.Add(emailMessage);

                    return Ok(_mapper.Map<AracTalepTableSaveDto>(savedAracTalepTable));

                }

            }

        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);
                bllAracTalepTable.Delete(id);
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
            BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);

            AracTalepTable? aracTalepTable = bllAracTalepTable.GetByID(id);
            if (aracTalepTable == null)
            {
                return NotFound();
            }
            return Ok(aracTalepTable);

        }
        #endregion

        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> myApprovalCount([FromForm] int userId)
        {
            BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);

            int count = bllAracTalepTable.approvalCount(userId);
            return Ok(count);
        }
        #endregion

        #region myList
        [HttpPost("myListdto")]
        public ActionResult<PageReturn<AracTalepTableDto>> myList([FromForm] FilterPageParam<AracTalepTableParamsDto> filterPageParam)
        {
            BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            PageReturn<AracTalepTableDto>? liste = bllAracTalepTable.mylistDto(filterPageParam, userId);
            return Ok(liste);
        }
        #endregion

        #region activeList
        [HttpPost("activeListdto")]
        public ActionResult<PageReturn<AracTalepTableDto>> activeList([FromForm] FilterPageParam<AracTalepTableParamsDto> filterPageParam)
        {
            BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLL.BLLActions.AdminUsers bllAdminUsers = new BLL.BLLActions.AdminUsers(_configuration, _env, _mapper);

            PageReturn<AracTalepTableDto>? liste = bllAracTalepTable.activeListdto(filterPageParam, bllAdminUsers.GetByID(userId)!.roleId);
            return Ok(liste);
        }
        #endregion

        #region completedList
        [HttpPost("completedListdto")]
        public ActionResult<PageReturn<AracTalepTableDto>> completedList([FromForm] FilterPageParam<AracTalepTableParamsDto> filterPageParam)
        {
            BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);
            int userId = 0;
            if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
            {
                userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

            }
            BLL.BLLActions.AdminUsers bllAdminUsers = new BLL.BLLActions.AdminUsers(_configuration, _env, _mapper);

            PageReturn<AracTalepTableDto>? liste = bllAracTalepTable.completedListdto(filterPageParam, bllAdminUsers.GetByID(userId)!.roleId);
            return Ok(liste);
        }
        #endregion

        #region reject
        [HttpPost("reject")]

        public async Task<ActionResult<int>> reject([FromForm] int talepId, [FromForm] int userId)
        {
            BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);
            int deger = await bllAracTalepTable.reject(talepId, userId);
            return Ok(deger);
        }
        #endregion

        #region confirm
        [HttpPost("confirm")]
        public async Task<ActionResult<int>> confirm([FromForm] int talepId, [FromForm] int userId)
        {
            BLL.BLLActions.AracTalepTable bllAracTalepTable = new BLL.BLLActions.AracTalepTable(_configuration, _env, _mapper);
            int deger = await bllAracTalepTable.confirm(talepId, userId);
            return Ok(deger);
        }
        #endregion


        #region AracTalepFormFinishedDetail
        [HttpPost("pdf")]
        public ActionResult<ResponseByteArray> AracTalepFormFinishedDetail([FromForm] int talepId)
        {

            BLLActions.AracTalepTable bllAracTalepTable = new BLLActions.AracTalepTable(_configuration, _env, _mapper);
            AracTalepTable? aracTalepTable = bllAracTalepTable.GetByID(talepId);

            string? filePath = Path.Combine("C:\\Users\\dilek.sariyerlioglu\\Source\\Repos\\askaleportalccore\\AskalePortal.BLL\\Raporlar");

            BLLActions.AracTalepTableDetail bllAracTalepTableDetail = new BLLActions.AracTalepTableDetail(_configuration, _env);
            List<AracTalepDataSource> liste = bllAracTalepTableDetail.getByReport(talepId);
            string fileFull = Path.Combine(filePath, "AracTalepTable.rdl");

            FileStream reportDefinition = new FileStream(fileFull, FileMode.Open, FileAccess.Read);



            ReportParameter talepIdParameter = new ReportParameter("talepId", talepId.ToString());
            ReportParameter acanKullaniciParameter = new ReportParameter("acanKullanici", aracTalepTable?.createdUser?.name ?? "");
            ReportParameter aciklamaParameter = new ReportParameter("aciklama", aracTalepTable?.aciklama);
            ReportParameter baslangicTarihiParameter = new ReportParameter("baslangicTarihi", aracTalepTable?.baslangicTarihi.ToString());
            ReportParameter teslimTarihiParameter = new ReportParameter("teslimTarihi", aracTalepTable?.teslimTarihi.ToString());
            ReportParameter gidilecekYerParameter = new ReportParameter("gidilecekYer", aracTalepTable?.destinationLocation?.destinationLocation ?? "");
            ReportParameter plakaParameter =
                     new ReportParameter("plaka", aracTalepTable?.plaka);
            List<ReportParameter> listReportParameter = new List<ReportParameter>();
            listReportParameter.Add(talepIdParameter);
            listReportParameter.Add(acanKullaniciParameter);
            listReportParameter.Add(aciklamaParameter);
            listReportParameter.Add(baslangicTarihiParameter);
            listReportParameter.Add(teslimTarihiParameter);
            listReportParameter.Add(gidilecekYerParameter);
            listReportParameter.Add(plakaParameter);






            // LocalReport nesnesi oluştur
            using (var localReport = new LocalReport())
            {
                // RDL dosyasını yükle
                localReport.ReportPath = fileFull;

                // Parametreleri ayarla
                localReport.SetParameters(listReportParameter);

                // Veri kaynağı ekle (örnek bir DataTable)
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("approved", typeof(string));
                dataTable.Columns.Add("username", typeof(string));
                dataTable.Columns.Add("shortDescription", typeof(string));
                foreach (var item in liste)
                {
                    dataTable.Rows.Add(item.approved, item.username, item.shortDescription);
                }


                localReport.DataSources.Add(new ReportDataSource("AracTalepTableDataset", dataTable)); // DataSet adı RDL'dekiyle eşleşmeli

                // Raporu PDF olarak render et
                string mimeType;
                string encoding;
                string fileNameExtension;
                string[] streams;
                Warning[] warnings;

                byte[] pdfBytes = localReport.Render(
                    "PDF",
                    null,
                    out mimeType,
                    out encoding,
                    out fileNameExtension,
                    out streams,
                    out warnings);



                ResponseByteArray responseByteArray = new ResponseByteArray();

                responseByteArray.file = pdfBytes;
                responseByteArray.fileName = "Rapor.pdf";
                return Ok(responseByteArray);
            }
        }
        #endregion
    }
}
