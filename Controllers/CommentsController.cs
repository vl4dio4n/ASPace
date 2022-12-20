using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ASPace.Data;
using ASPace.Models;
using ASPace.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ASPace.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public CommentsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // GET: Comments
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //[Authorize(Roles = "User,Moderator,Admin")]
        //public IActionResult Delete(int id)
        //{
        //    Comment? comm = db.Comments.Where(m => m.CommentId == id).Include("Post").First();
        //    if (comm == null)
        //    {
        //        TempData["message"] = "The comment doesn't exist!";
        //        return RedirectToAction("Index");
        //    }
        //    db.Comments.Remove(comm);
        //    db.SaveChanges();
        //    return Redirect("/Posts/Show/" + comm.PostId);
        //}

        

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;
            comm.UserId = _userManager.GetUserId(User);
            try
            {
                db.Comments.Add(comm);
                db.SaveChanges();
                return Redirect("/Posts/Show/" + comm.PostId);
            }

            catch (Exception e)
            {
                return Redirect("/Posts/Show/" + comm.PostId);
            }

        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id)
        {
            Comment comm = db.Comments.Where(m => m.CommentId == id)
                .Include("Post").Include("User").Include("CommentLikes").First();
            if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "You don't have enough permissions to modify this article!";
                return Redirect("/Posts/Show/" + comm.PostId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                
                Comment comm = db.Comments.Where(m => m.CommentId == id)
                                .Include("Post").Include("User").Include("CommentLikes").First();
                if (comm.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    
                        comm.Content = requestComment.Content;
                        comm.Date = requestComment.Date;
                        TempData["message"] = "The comment has been successfully changed!";
                        db.SaveChanges();
                        return Redirect("/Posts/Show/" + comm.PostId);
                }
                else
                {
                    TempData["message"] = "You don't have enough permissions to modify this comment!";
                    return RedirectToAction("Index");
                }
               
            }
            catch (Exception e)
            {
                return View(requestComment);
            }
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Posts/Show/" + comm.PostId);
        }
    }
}