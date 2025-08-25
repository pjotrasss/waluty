using Microsoft.AspNetCore.Mvc;
using waluty.Models;
using waluty.Services;

namespace waluty.Controllers
{
    //base route for controller
    [Route("/api")]
    public class ApiCurrenciesController : Controller
    {
        private readonly ApiCurrenciesService _apiCurrenciesService;

        public ApiCurrenciesController(ApiCurrenciesService apiCurrenciesService)
        {
            _apiCurrenciesService = apiCurrenciesService;
        }

        [HttpGet("fetch")]
        public async Task<IActionResult> FetchRatesByDate(DateTime? date)
        {
            var selectedDate = (date ?? DateTime.Today).Date;

            FetchCurrenciesView? viewModel = await _apiCurrenciesService.prepareFetchCurrenciesViewModel(selectedDate)
                ?? new FetchCurrenciesView { SelectedDate = selectedDate, Message = string.Empty };
            return View(viewModel);
        }
    }
}