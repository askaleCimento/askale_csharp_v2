using AskalePortal.BLL;
using AskalePortal.Data.Contracts.Detached;
using AskalePortal.Data.Functions;
using AskalePortal.Data.Models;
using AskalePortal.Data.RequestModel;
using AskalePortal.Data.RequestParams;
using AskalePortal.Data.ResponseModels;
using AskalePortal.Data.ResponseParams;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ReportingServices.ReportProcessing.ReportObjectModel;
using Newtonsoft.Json.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static AskalePortal.BLL.BLLActions;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ISGAksiyonTableController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public ISGAksiyonTableController(IConfiguration configuration, IWebHostEnvironment env, IMapper mapper)
        {
            _configuration = configuration;
            _env = env;
            _mapper = mapper;
        }
        #region approvalCount
        [HttpPost("approvalCount")]
        public ActionResult<int> approvalCount([FromForm] int userId)
        {

            BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLLActions.ISGAksiyonTable(_configuration, _env,_mapper);
            int count = bllISGAksiyonTable.approvalCount(userId);
            return Ok(count);
        }
        #endregion


        #region Save
        [HttpPost("save")]

        public async Task<ActionResult<ISGAksiyonTableSaveDto>> save([FromForm] ISGAksiyonTableSaveDto entity)
        {

            if (entity != null)
            {
                int userId = 0;
                if (HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
                {
                    userId = int.Parse(claimsIdentity?.FindFirst("userId")?.Value ?? "0");

                }
                BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);
                ISGAksiyonTableSaveDto saveDto = await bllISGAksiyonTable.save(entity, userId);
                return Ok(saveDto);
            }
            return Ok(null);
        }
        #endregion

        #region delete
        [HttpPost("delete")]

        public ActionResult<int> delete([FromForm] int id)
        {
            try
            {
                BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);
                bllISGAksiyonTable.Delete(id);
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

        public ActionResult<ISGAksiyonTableSaveDto> getById([FromForm] int id)
        {
            BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);

            ISGAksiyonTableSaveDto? isgAksiyonTable = _mapper.Map< ISGAksiyonTableSaveDto > (bllISGAksiyonTable.GetByID(id));
            if (isgAksiyonTable == null)
            {
                return NotFound();
            }
            return Ok(isgAksiyonTable);


        }
        #endregion


        #region getAll
        [HttpPost("getAll")]

        public ActionResult<object> getAll()
        {
            BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);

            List<Data.Models.ISGAksiyonTable>? listISGAksiyonTable = bllISGAksiyonTable.GetAll();
            return Ok(listISGAksiyonTable);

        }
        #endregion

        #region listByFilter
        [HttpPost("listByFilter")]
        public ActionResult<PageReturn<ISGAksiyonTableDto>> listByFilter([FromForm] FilterPageParam<IsgAksiyonTablePageableListParameter> filterPageParam)
        {
            BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);
            PageReturn<ISGAksiyonTableDto> page = bllISGAksiyonTable.listByPageable(filterPageParam);
            return Ok(page);
        }
        #endregion

        #region listFilter
        [HttpPost("listFilter")]
        public ActionResult<List<ISGAksiyonTableDto>> listFilter([FromForm] FilterParam<IsgAksiyonTablePageableListParameter> filterParam)
        {
            BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);

            List<ISGAksiyonTableDto> dto = bllISGAksiyonTable.listFilter(filterParam);
            return Ok(dto);
        }
        #endregion

        #region deleteFile
        [HttpPost("deleteFile")]
        public async Task<ActionResult<string>> deleteFile([FromForm] int id, [FromForm] string fileName)
        {
            BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);
            string deger = await bllISGAksiyonTable.deleteFile(id, fileName);
            return Ok(deger);
        }
        #endregion

        #region completed
        [HttpPost("completed")]
        public async Task<ActionResult<string>> completed([FromForm] int id)
        {
            BLL.BLLActions.ISGAksiyonTable bllISGAksiyonTable = new BLL.BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);
            string deger = await bllISGAksiyonTable.completed(id);
            return Ok(deger);
        }
        #endregion

        #region download
        [HttpPost("download")]
        public ActionResult<ResponseByteArray> download([FromForm] string file)
        {

            if (file.Equals(""))
            {
                return Ok(null);
            }
            string? filePath = Path.Combine(_env.IsDevelopment() ? _configuration["FilePath:local"]! : _env.IsProduction() ?
                   _configuration["FilePath:server"]! : _configuration["FilePath:test"]!, "documents\\ISG\\");

            ResponseByteArray responseByteArray = FileConverter.convertByte(filePath, file, file);

            return Ok(responseByteArray);

        }
        #endregion

        #region upload

        [HttpPost]
        [Route("upload")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [RequestFormLimits(
            ValueLengthLimit = int.MaxValue,
            MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> Upload(
            [FromForm] IFormFile[] file,
            [FromForm] int targetId)
        {
            if (targetId <= 0)
            {
                return BadRequest("targetId geçersiz veya boþ.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Yüklenecek dosya bulunamadý.");
            }

            // Projenizdeki ISG aksiyon BLL sýnýfýnýn ismine göre düzenleyin.
            BLLActions.ISGAksiyonTable bllISGAksiyon =
                new BLLActions.ISGAksiyonTable(_configuration, _env, _mapper);

            Data.Models.ISGAksiyonTable? isgAksiyonTable =
                bllISGAksiyon.GetByID(targetId);

            if (isgAksiyonTable == null)
            {
                return NotFound($"ISG aksiyon kaydý bulunamadý. Id: {targetId}");
            }

            string? basePath = _env.IsDevelopment()
                ? _configuration["FilePath:local"]
                : _env.IsProduction()
                    ? _configuration["FilePath:server"]
                    : _configuration["FilePath:test"];

            if (string.IsNullOrWhiteSpace(basePath))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Dosya kayýt yolu tanýmlanmamýþ.");
            }

            string directoryPath = Path.Combine(
                basePath,
                "documents",
                "ISG");

            Directory.CreateDirectory(directoryPath);

            List<string> uploadedFileNames = new();

            foreach (IFormFile formFile in file)
            {
                if (formFile == null || formFile.Length <= 0)
                {
                    continue;
                }

                // Kullanýcý tarafýndan gönderilen klasör bilgisini temizler.
                string originalFileName =
                    Path.GetFileName(formFile.FileName);

                string nameWithoutExtension =
                    Path.GetFileNameWithoutExtension(originalFileName);

                string extension =
                    Path.GetExtension(originalFileName);

                if (string.IsNullOrWhiteSpace(nameWithoutExtension))
                {
                    nameWithoutExtension = "file";
                }

                // Java kodundaki 25 karakter sýnýrý.
                if (nameWithoutExtension.Length > 25)
                {
                    nameWithoutExtension =
                        nameWithoutExtension.Substring(0, 25);
                }

                // Milisaniye eklenmesi ayný saniyede yüklenen dosyalarýn
                // birbirinin üzerine yazýlma ihtimalini azaltýr.
                string datePart =
                    DateTime.Now.ToString("yyyyMMddHHmmssfff");

                string newFileName =
                    $"{nameWithoutExtension}-{datePart}{extension}";

                string fileFullPath =
                    Path.Combine(directoryPath, newFileName);

                await using FileStream stream =
                    new FileStream(
                        fileFullPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None);

                await formFile.CopyToAsync(stream);

                uploadedFileNames.Add(newFileName);
            }

            if (uploadedFileNames.Count == 0)
            {
                return BadRequest("Geçerli bir dosya yüklenemedi.");
            }

            // Java kodundaki dosya1$dosya2$dosya3 yapýsý.
            string fileNames =
                string.Join("$", uploadedFileNames);

            isgAksiyonTable.fileOnceki = fileNames;

            // Projenizdeki metoda göre Save/Update çaðrýsýný uyarlayýn.
            await bllISGAksiyon.Update(isgAksiyonTable);

            return Ok(new
            {
                count = uploadedFileNames.Count,
                fileNames
            });
        }

        #endregion

        #region uploadcomplete

        [HttpPost]
        [Route("uploadcomplete")]
        [Consumes("multipart/form-data")]
        [Produces("application/json")]
        [RequestFormLimits(
            ValueLengthLimit = int.MaxValue,
            MultipartBodyLengthLimit = int.MaxValue)]
        public async Task<ActionResult> UploadComplete(
            [FromForm] IFormFile[] file,
            [FromForm] int targetId)
        {
            if (targetId <= 0)
            {
                return BadRequest("targetId geçersiz veya boþ.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Yüklenecek dosya bulunamadý.");
            }

            BLLActions.ISGAksiyonTable bllISGAksiyon =
                new BLLActions.ISGAksiyonTable(
                    _configuration,
                    _env, _mapper
                    );

            Data.Models.ISGAksiyonTable? isgAksiyonTable =
                bllISGAksiyon.GetByID(targetId);

            if (isgAksiyonTable == null)
            {
                return NotFound(
                    $"ISG aksiyon kaydý bulunamadý. Id: {targetId}");
            }

            string? basePath = _env.IsDevelopment()
                ? _configuration["FilePath:local"]
                : _env.IsProduction()
                    ? _configuration["FilePath:server"]
                    : _configuration["FilePath:test"];

            if (string.IsNullOrWhiteSpace(basePath))
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError,
                    "Dosya kayýt yolu tanýmlanmamýþ.");
            }

            string directoryPath = Path.Combine(
                basePath,
                "documents",
                "ISG");

            Directory.CreateDirectory(directoryPath);

            List<string> uploadedFileNames = new();

            foreach (IFormFile formFile in file)
            {
                if (formFile == null || formFile.Length <= 0)
                {
                    continue;
                }

                string originalFileName =
                    Path.GetFileName(formFile.FileName);

                string nameWithoutExtension =
                    Path.GetFileNameWithoutExtension(originalFileName);

                string extension =
                    Path.GetExtension(originalFileName);

                if (string.IsNullOrWhiteSpace(nameWithoutExtension))
                {
                    nameWithoutExtension = "file";
                }

                // Java kodundaki 25 karakter sýnýrý.
                if (nameWithoutExtension.Length > 25)
                {
                    nameWithoutExtension =
                        nameWithoutExtension.Substring(0, 25);
                }

                string datePart =
                    DateTime.Now.ToString("yyyyMMddHHmmssfff");

                string newFileName =
                    $"{nameWithoutExtension}-{datePart}{extension}";

                string fileFullPath =
                    Path.Combine(directoryPath, newFileName);

                await using FileStream stream =
                    new FileStream(
                        fileFullPath,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None);

                await formFile.CopyToAsync(stream);

                uploadedFileNames.Add(newFileName);
            }

            if (uploadedFileNames.Count == 0)
            {
                return BadRequest("Geçerli bir dosya yüklenemedi.");
            }

            // dosya1$dosya2$dosya3 formatýnda kaydeder.
            string fileNames =
                string.Join("$", uploadedFileNames);

            isgAksiyonTable.fileSonraki = fileNames;

            await bllISGAksiyon.Update(isgAksiyonTable);

            return Ok(new
            {
                count = uploadedFileNames.Count,
                fileNames
            });
        }

        #endregion




    }
}
