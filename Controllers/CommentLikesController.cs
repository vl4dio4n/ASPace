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
using Microsoft.EntityFrameworkCore;

namespace ASPace.Controllers
{
    public class CommentLikesController : Controller
    {
        private readonly ApplicationDbContext db;
        public CommentLikesController(ApplicationDbContext context)
        {
            db = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New(CommentLike commlike)
        {
            try
            {
                db.CommentLikes.Add(commlike);
                db.Comments.Find(commlike.CommentId).LikeCount++;
                db.SaveChanges();
                return Redirect("/Posts/Show/" + commlike.Comment.PostId);
            }
            catch (Exception e)
            {
                return Redirect("/Posts/Show/" + commlike.Comment.PostId);
            }

        }

        [Authorize(Roles = "User,Moderator,Admin")]
        [HttpPost]
        public IActionResult Delete(int CommentId, string UserId)
        {
            CommentLike? ToDelete = db.CommentLikes.Where(m => (m.CommentId == CommentId) && (m.UserId == UserId)).Include("Comment").Include("User").First();
            db.CommentLikes.Remove(ToDelete);
            db.Comments.Find(CommentId).LikeCount--;
            db.SaveChanges();
            var com = db.Comments.Where(m => m.CommentId == CommentId).Include("Post").First();
            return Redirect("/Posts/Show/" + com.PostId);
        }
    }
}