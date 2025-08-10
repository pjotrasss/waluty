using Microsoft.AspNetCore.Mvc;

namespace waluty.Controllers {
    [ApiController]
    [Route("/ping")]
    public class PingController : ControllerBase {
        [HttpGet]
        public string Get() => "pong";
    }
}