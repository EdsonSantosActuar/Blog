using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Blog.ViewModels.Posts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class PostController : ControllerBase
{
    [HttpGet("v1/posts")]
    public async Task<IActionResult> GetAscyn([FromServices] BlogDataContext context,
                                              [FromQuery] int page = 0,
                                              [FromQuery] int pageSize = 25)
    {
        var count = await context.Posts.CountAsync();

        var posts = await context.Posts
                           .AsNoTracking()
                           .Include(x => x.Category)
                           .Include(x => x.Author)
                           .Select(x => new ListPostsViewModel
                           {
                               Id = x.Id,
                               Title = x.Title,
                               Slug = x.Slug,
                               LastUpdateDate = x.LastUpdateDate,
                               Category = x.Category.Name,
                               Author = $"{x.Author.Name} ({x.Author.Email})"
                           })
                           .Skip(page * pageSize)
                           .Take(pageSize)
                           .OrderByDescending(x => x.LastUpdateDate)
                           .ToListAsync();

        return Ok(new ResultViewModel<dynamic>(new {total = count, page, pageSize, posts}));
    }
}