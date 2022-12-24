using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ASPace.Data;
using ASPace.Models;
using ASPace.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ASPace.Controllers
{
    public class PostLikesController : Controller
    {
        private readonly ApplicationDbContext db;
        public PostLikesController(ApplicationDbContext context)
        {
            db = context;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New(PostLike postlike)
        {
            try
            {
                db.PostLikes.Add(postlike);
                db.Posts.Find(postlike.PostId).LikeCount++;
                db.SaveChanges();
                return Redirect("/Posts/Show/" + postlike.PostId);
            }
            catch (Exception e)
            {
                return Redirect("/Posts/Show/" + postlike.PostId);
            }

        }

        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Delete(int PostId, string UserId)
        {
            PostLike? ToDelete = db.PostLikes.Where(m => m.PostId == PostId && m.UserId == UserId).Include("User").Include("Post").First();

            db.PostLikes.Remove(ToDelete);
            db.Posts.Find(PostId).LikeCount--;
            db.SaveChanges();
            return Redirect("/Posts/Show/" + PostId);
        }
    }
}