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
using Microsoft.Extensions.Hosting;

namespace ASPace.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PostsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Index()
        {
            string CurrUserId = _userManager.GetUserId(User);
            List<int>? myGroups = (from gr in db.GroupMembers
                            where gr.UserId == CurrUserId
                            select gr.GroupId).ToList();

            List<string>? myFriends = (from fr in db.Friendships
                             where fr.FirstId == CurrUserId
                             select fr.SecondId).ToList();


            IQueryable<Post>? posts = db.Posts.Where(u => (u.UserId == CurrUserId ||
                                        (u.GroupId != null && myGroups.Contains((int)u.GroupId)) ||
                                        (u.GroupId == null && (myFriends.Contains(u.UserId)))
                                        ))
                                .OrderByDescending(a => a.Date)
                                .Include("User").Include("Group");

            // Search 
            string search = "";

            if (Convert.ToString(HttpContext.Request.Query["search"]) != null)
            {
                search = Convert.ToString(HttpContext.Request.Query["search"]).Trim().ToLower();
            }
            
            List<int> postsIds = db.Posts.Where(
                post => (post.Title.Contains(search) || post.Content.Contains(search))
                //&& (CurrUserId == post.UserId)
                ).Select(p => p.PostId).ToList();

            posts = posts.Where(post => postsIds.Contains(post.PostId)).OrderByDescending(a => a.Date).Include("User").Include("Group"); ;

            ViewBag.SearchString = search;
            ViewBag.Posts = posts;
            ViewBag.Count = posts.Count();
            return View();
        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Show(int id)
        {
            Post? post = db.Posts.Where(m => m.PostId == id).Include("User")
                .Include("PostLikes.User").Include("Comments.CommentLikes.User").Include("Group").First();
            if (post == null)
            {
                TempData["message"] = "The post doesn't exist!";
                return RedirectToAction("Index");
            }
            ViewBag.CurrentUser = new Tuple<string, bool>(_userManager.GetUserId(User), post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"));
            ViewBag.IsLikedByUser = db.PostLikes.Find(id, _userManager.GetUserId(User)) != null;
            return View(post);

        }

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New()
        {
            Post post = new Post();
            post.UserId = _userManager.GetUserId(User);
            var curr = _userManager.GetUserId(User);
            ViewBag.Curr = curr;

            return View(post);
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult New([FromForm] Post post)
        {
            post.UserId = _userManager.GetUserId(User);
            post.Date = DateTime.Now;
            try
            {
                if (ModelState.IsValid)
                {
                    db.Posts.Add(post);
                    db.SaveChanges();
                    TempData["message"] = "Your post has been added!";
                    /*if (post.GroupId != null)
                    {
                        return RedirectToAction("Show", "Groups", new { id = post.GroupId });
                        // return Redirect($"/Groups/Show/{post.GroupId}");
                    }
                    return RedirectToAction("Index")*/
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

        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id)
        {

            Post? post = db.Posts.Where(m => m.PostId == id).Include("User")
                .Include("PostLikes").Include("Comments").Include("Group").First();
            if (post == null)
            {
                TempData["message"] = "The post doesn't exist!";
                return RedirectToAction("Index");
            }
            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                return View(post);
            }
            else
            {
                TempData["message"] = "You don't have enough permissions to modify this post!";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Edit(int id, Post requestPost)
        {
            try
            {
                Post? post = db.Posts.Where(m => m.PostId == id).Include("User")
                    .Include("Comments").Include("PostLikes")
                    .First();
                if (post == null)
                {
                    TempData["message"] = "The post doesn't exist!";
                    return RedirectToAction("Index");
                }
                if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
                {
                    
                    post.Title = requestPost.Title;
                    post.Content = requestPost.Content;
                    post.Date = requestPost.Date;
                    db.SaveChanges();
                    TempData["message"] = "The post has been successfully changed! ||"+id +"||" +requestPost.
                        PostId;
                    return RedirectToAction("Show", new { id = post.PostId });

                }
                else
                {
                    TempData["message"] = "You don't have enough permissions to modify this post!";
                    return RedirectToAction("Index");
                }
                
            }
            catch (Exception e)
            {
                return View(requestPost);
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,Moderator,Admin")]
        public IActionResult Delete(int id)
        {
            Post? post = db.Posts.Where(m => m.PostId == id).Include("User").First();
            if (post == null)
            {
                TempData["message"] = "The post doesn't exist!";
                return RedirectToAction("Index");
            }
            if (post.UserId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            {
                if (post.UserId != _userManager.GetUserId(User) && (User.IsInRole("Admin")))
                {

                    string subject = "Your post has been deleted.";
                    string body = "Hello, " + post.User.UserName + " ! <br /> Unfortunately, your post " + post.Title + " was deleted by our Admin: " + _userManager.GetUserName(User) + "<br /> :(";

                    // send email
                }
                db.Posts.Remove(post);
                db.SaveChanges();
                TempData["message"] = "The post was deleted";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "You don't have enough permissions to modify this post!";
                return RedirectToAction("Index");
            }

        }
    }

}