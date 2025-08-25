using Microsoft.AspNetCore.Mvc;
using waluty.Services;

namespace waluty.Controllers
{
    //base route for controller
    [Route("/db")]
    public class DbCurrenciesController : Controller
    {
        private readonly DbCurrenciesService _dbCurrenciesService;

        public DbCurrenciesController(DbCurrenciesService dbCurrenciesService)
        {
            _dbCurrenciesService = dbCurrenciesService;
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> RatesByDate(int tableId)
        {
            var viewModel = await _dbCurrenciesService.prepareDbCurrenciesViewModel(tableId);
            return View(viewModel);
        }
    }
}