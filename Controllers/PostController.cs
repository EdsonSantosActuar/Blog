using System.Data.Common;
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
        try
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

            return Ok(new ResultViewModel<dynamic>(new { total = count, page, pageSize, posts }));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("03X01 - Um erro inesperado ocorreu!"));
        }
    }

    [HttpGet("v1/posts/{id:int}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] int id, [FromServices] BlogDataContext context)
    {
        try
        {
            var post = await context.Posts
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
                               .FirstOrDefaultAsync(x => x.Id == id);

            if (post is null)
                return NotFound(new ResultViewModel<Post>("Conteúdo não encontrado!"));

            return Ok(new ResultViewModel<ListPostsViewModel>(post));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("03X02 - Um erro inesperado ocorreu!"));
        }
    }

    [HttpGet("v1/posts/category/{category}")]
        public async Task<IActionResult> GetPostsByCategoryAscyn([FromServices] BlogDataContext context,
                                                                 [FromRoute] string category,
                                                                 [FromQuery] int page = 0,
                                                                 [FromQuery] int pageSize = 25)
    {
        try
        {
            var count = await context.Posts.CountAsync();

            var posts = await context.Posts
                               .AsNoTracking()
                               .Include(x => x.Author)
                               .Include(x => x.Category)
                               .Where(x => x.Category.Slug.ToLower() == category.ToLower())
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

            return Ok(new ResultViewModel<dynamic>(new { total = count, page, pageSize, posts }));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<string>("03X03 - Um erro inesperado ocorreu!"));
        }
    }
}