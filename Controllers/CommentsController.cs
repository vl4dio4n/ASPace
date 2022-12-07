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

namespace ASPace.Controllers
{
    public class CommentsController : Controller
    {
        private readonly ApplicationDbContext db;
        public CommentsController(ApplicationDbContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            Comment comm = db.Comments.Find(id);
            db.Comments.Remove(comm);
            db.SaveChanges();
            return Redirect("/Posts/Show/" + comm.PostId);
        }

        [HttpPost]
        public ActionResult New(Comment comm)
        {
            comm.Date = DateTime.Now;
            // userId curent
            comm.UserId = "1";
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

        public ActionResult Edit(int id)
        {
            Comment comm = db.Comments.Where(c => c.CommentId == id).First();
            // user curent sau admin
            if (comm.UserId == "1")
            {
                return View(comm);
            }
            else
            {
                TempData["message"] = "You don't have permissions to modify this comment!";
                return Redirect("/Posts/Show/" + comm.PostId);
            }
        }

        [HttpPost]
        public ActionResult Edit(int id, Comment requestComment)
        {
            try
            {
                Comment comm = db.Comments.Where(c => c.CommentId == id).First();
                comm.Content = requestComment.Content;
                comm.Date = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "The comment has been changed successfully!";
                return Redirect("/Posts/Show/" + comm.PostId);
            }
            catch (Exception e)
            {
                return View("Edit", requestComment);
            }
        }
    }
}
