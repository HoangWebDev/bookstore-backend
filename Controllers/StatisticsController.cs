using BookStore.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : Controller
    {

        private readonly IStatisticRepository _statisticRepository;

        public StatisticsController(IStatisticRepository statisticRepository)
        {
            _statisticRepository = statisticRepository;
        }

        //Thống kê số lượng user
        [Authorize(Roles = "Admin")]
        [HttpGet("count_user")]
        public async Task<IActionResult> GetStatisticUser()
        {
            var statsUser = await _statisticRepository.GetStatistic("Users");
            return Ok(statsUser);
        }

        //Thống kê số lượng sách
        [Authorize(Roles = "Admin")]
        [HttpGet("count_book")]
        public async Task<IActionResult> GetStatisticBook()
        {
            var statsBook = await _statisticRepository.GetStatistic("Books");
            return Ok(statsBook);
        }

        //Thống kê số lượng tất cả đối tượng
        [Authorize(Roles = "Admin")]
        [HttpGet("count_all")]
        public async Task<IActionResult> GetStatisticsAll()
        {
            var list = new List<string>{ "Books", "Authors", "Categories", "Publishers" };         

            var statsAll = await _statisticRepository.GetStatistics(list);
            return Ok(statsAll);
        }
    }
}
