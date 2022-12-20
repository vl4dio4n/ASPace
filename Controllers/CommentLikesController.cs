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
    public class CommentLikesController : Controller
    {
        private readonly ApplicationDbContext db;
        public CommentLikesController(ApplicationDbContext context)
        {
            db = context;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public ActionResult New(CommentLike commlike)
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

        [Authorize(Roles = "User,Admin")]
        [HttpPost]
        public ActionResult Delete(int CommentId, string UserId)
        {
            CommentLike ToDelete = db.CommentLikes.Find(CommentId, UserId);
            db.CommentLikes.Remove(ToDelete);
            db.Comments.Find(CommentId).LikeCount--;
            db.SaveChanges();
            return Redirect("/Posts/Show/" + db.Comments.Find(CommentId).PostId);
        }
    }
}