using AskalePortal.BLL;
using AskalePortal.Data.ResponseModels;
using Microsoft.AspNetCore.Mvc;

namespace AskalePortal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public sealed class HRExpenseReportController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public HRExpenseReportController(IConfiguration configuration, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _env = env;
        }

        [HttpPost("pdf")]
        public ActionResult<ResponseByteArray> Pdf([FromForm] int tripId)
        {
            if (tripId <= 0)
            {
                return BadRequest("tripId sıfırdan büyük olmalıdır.");
            }

            try
            {
                BLLActions.HRExpenseReport report = new BLLActions.HRExpenseReport(_configuration, _env);
                byte[] pdf = report.CreatePdf(tripId);

                return Ok(new ResponseByteArray
                {
                    file = pdf,
                    fileName = $"HarcamaRaporu_{tripId}.pdf",
                    name = "application/pdf"
                });
            }
            catch (KeyNotFoundException exception)
            {
                return NotFound(exception.Message);
            }
        }
    }
}
