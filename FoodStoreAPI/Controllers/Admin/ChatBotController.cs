using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using FoodStoreAPI.DAO;
using System;
using Newtonsoft.Json;
using FoodStoreAPI.Models;
using static FoodStoreAPI.DTOs.ChatBotDTO;

namespace FoodStoreAPI.Controllers.Admin
{
    [Route("api/admin/[controller]")]
    [ApiController]
    public class ChatBotController : ControllerBase
    {
        private readonly ChatBotDAO _chatBotDAO;

        public ChatBotController(ChatBotDAO chatBotDAO)
        {
            _chatBotDAO = chatBotDAO;
        }

        [HttpPost("ask")]
        public async Task<IActionResult> Ask([FromBody] ChatBotRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.request))
                return BadRequest("Question is required.");
            try
            {
                var res = await _chatBotDAO.CallChatGPT(request.request);
                //var cleanJson = JsonConvert.SerializeObject(res.result); 
                return Ok(new
                {
                    summary = res.summary,
                    sql = res.sql,
                    result = res.result,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }

}