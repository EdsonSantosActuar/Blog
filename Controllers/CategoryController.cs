using Blog.Data;
using Blog.Models;
using Blog.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    [HttpGet("v1/categories")]
    public async Task<IActionResult> GetAsync([FromServices] BlogDataContext context)
    {
        try
        {
            var categories = await context.Categories.ToListAsync();
            return Ok(new ResultViewModel<List<Category>>(categories));
        }
        catch
        {
            return StatusCode(500, new ResultViewModel<List<Category>>("05X01 - Um erro inesperado ocorreu!"));
        }

    }

    [HttpGet("v1/categories/{id:int}")]
    public async Task<IActionResult> GetByIdAsync(int id, [FromServices] BlogDataContext context)
    {
        var category = await context.Categories
            .FirstOrDefaultAsync(x => x.Id == id);

        if (category is null)
            return NotFound();

        return Ok(category);
    }

    [HttpPost("v1/categories")]
    public async Task<IActionResult> PostAsync([FromBody] EditorCategoryViewModel model, [FromServices] BlogDataContext context)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var category = new Category
            {
                Id = 0,
                Name = model.Name,
                Slug = model.Slug.ToLower()
            };
            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return Created($"v1/categories/{category.Id}", category);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "05XE9 - Não foi possível incluir a categoria.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05X10 - Um erro inesperado ocorreu!");
        }
    }

    [HttpPut("v1/categories/{id:int}")]
    public async Task<IActionResult> PutAsync(int id,
                                                    [FromBody] EditorCategoryViewModel model,
                                                    [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
                return NotFound();

            category.Name = model.Name;
            category.Slug = model.Slug.ToLower();

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return Ok(category);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "05XE8 - Não foi possível incluir a categoria.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05X11 - Um erro inesperado ocorreu!");
        }


    }

    [HttpDelete("v1/categories/{id:int}")]
    public async Task<IActionResult> DeleteAsync(int id, [FromServices] BlogDataContext context)
    {
        try
        {
            var category = await context.Categories
                .FirstOrDefaultAsync(x => x.Id == id);

            if (category is null)
                return NotFound();

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return Ok(category);
        }
        catch (DbUpdateException ex)
        {
            return StatusCode(500, "05XE7 - Não foi possível incluir a categoria.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "05X12 - Um erro inesperado ocorreu!");
        }
    }
}