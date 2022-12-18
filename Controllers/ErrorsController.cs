using ASPace.Areas.Identity.Data;
using ASPace.Data;
using ASPace.Models;
using ASPace.CustomClasses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ArticlesApp.Controllers{
    [Route("Errors/{statuscode}")]
    public class ErrorsController : Controller{
        public IActionResult Index(int statuscode){
            ViewBag.ErrorCode = statuscode;
            if(TempData.ContainsKey("message")){
                ViewBag.ErrorMessage = TempData["message"];
            }
            
            switch(statuscode){
                case 404: 
                    ViewBag.ErrorTitle = "Not Found";
                    break;
                case 403:
                    ViewBag.ErrorTitle = "Forbidden";
                    break;
                default:
                    break;
            }

            return View();
        } 
    }
}