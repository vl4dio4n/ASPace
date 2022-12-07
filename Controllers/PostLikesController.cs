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


        [HttpPost]
        public IActionResult Delete(PostLike postLike)
        {
            try
            {
                PostLike ToDelete = db.PostLikes.Find(postLike.PostId, postLike.UserId);
                db.PostLikes.Remove(ToDelete);
                db.Posts.Find(postLike.PostId).LikeCount--;
                db.SaveChanges();
                return Redirect("/Posts/Show/" + postLike.PostId);
            }
            catch (Exception e)
            {
                return Redirect("/Posts/Show/" + postLike.PostId);
            }
        }
    }
}