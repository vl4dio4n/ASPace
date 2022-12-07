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
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;
        public PostsController(ApplicationDbContext context)
        {
            db = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            string CurrUserId = "1";

            var posts = db.Posts.Where(u => (u.UserId == CurrUserId)).OrderByDescending(a => a.Date);

            ViewBag.SearchString = "1";
            ViewBag.Posts = posts;
            return View();
        }


        public IActionResult Show(int id)
        {
            Post post = db.Posts.Include("Comments").Where(post => post.PostId == id).First();
            ViewBag.CurrentUser = new Tuple<string, bool>("1", post.UserId == "1");
            ViewBag.IsLikedByUser = db.PostLikes.Find(id, "1") != null;

            return View(post);

        }

        public IActionResult New()
        {
            Post post = new Post();
            post.UserId = "1";
            var curr = "1";
            ViewBag.Curr = curr;

            return View(post);
        }

        [HttpPost]
        public IActionResult New(Post post)
         {
            post.UserId = "1";
            post.Date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    TempData["message"] = "Your post has been added!";
                    return RedirectToAction((string)TempData["action"], (string)TempData["controller"], new { id = TempData["id"] });
                }
                else
                {
                    return View(post);
                }
            }
            catch (Exception e)
            {
                return View(post);
            }
        }

        public ActionResult Edit(int id)
        {

            Post post = db.Posts.Find(id);
            return View(post);
           
        }

        [HttpPost]
        public ActionResult Edit(int id, Post requestPost)
        {
            Post post = db.Posts.Find(id);

            try
            {
                post.Title = requestPost.Title;
                post.Content = requestPost.Content;
                post.Date = DateTime.Now;
                db.SaveChanges();
                TempData["message"] = "The post has been successfully changed!";
                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return View(requestPost);
            }
        }


        [HttpPost]
        public IActionResult Delete(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            TempData["message"] = "The post was deleted";
            return RedirectToAction("Index");
        }
    }

}
