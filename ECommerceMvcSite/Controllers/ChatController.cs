using ECommerceMvcSite.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ECommerceMvcSite.Controllers
{
   
        public class ChatController : Controller
        {
            private readonly OpenRouterService _chatService = new OpenRouterService();

            [HttpPost]
            public async Task<ActionResult> Ask(string prompt)
            {
                var response = await _chatService.GetChatbotResponse(prompt);
                return Json(new { response });
            }
        }

    
}