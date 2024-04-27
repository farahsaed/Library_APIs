using Library_APIs.Data;
using Library_APIs.DTO;
using Library_APIs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Library_APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly LibraryContext db;
        private readonly IWebHostEnvironment _environment;

        public CategoryController(LibraryContext db,IWebHostEnvironment environment)
        {
            this.db = db;
            this._environment = environment;
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var cats = await db.Categories.ToListAsync();

            var decryptedCats = new List<Category>();

            foreach (var cat in cats)
            {
                var decryptedCat = new Category
                {
                    Id = cat.Id,
                    Name = Encryption.Decrypt(cat.EncName),
                    Description = Encryption.Decrypt(cat.EncDescription)
                };

                decryptedCats.Add(decryptedCat);
            }

            return Ok(decryptedCats);
            //var cats = await db.Categories.ToListAsync();

            //if(cats == null)
            //    return NotFound("No Categories found");

            //return Ok(cats);
        }

        [HttpPost("CreateCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCatgeory(CategoryWithBooksDTO CatDTO)
        {
            var cat = new Category();
            if (CatDTO != null)
            {
                if (ModelState.IsValid)
                {
                    cat.Id = Guid.NewGuid();
                    cat.Name = CatDTO.Name;
                    cat.Description = CatDTO.Description;

                    cat.EncName = Encryption.Encrypt(cat.Name);
                    cat.EncDescription = Encryption.Encrypt(cat.Description);

                    db.Categories.Add(cat);
                    db.SaveChanges();
                    return Ok(cat);
                }

            }
            return BadRequest();
        }

        [HttpGet("GetCategory/{id}")]
        public async Task<IActionResult> GetCategory(Guid id)
        {
            var cat = await db.Categories.FirstOrDefaultAsync(x => x.Id == id);
            if (cat == null)
                return NotFound("Category not found");

            return Ok(cat);
        }

        [HttpPost("UpdateCategory/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCatgeory(Guid id, CategoryWithBooksDTO catDTO)
        {
            var cat = await db.Categories.FindAsync(id);
            if (cat == null)
                return NotFound("Category not found");

            if(ModelState.IsValid)
            {
                cat.Name = catDTO.Name;
                cat.Description = catDTO.Description;
                db.Categories.Update(cat);
                db.SaveChanges();
            }
            return Ok(cat);


        }

        [HttpDelete("DeleteCategory/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCatgeory(Guid id)
        {
            var cat = await db.Categories.FindAsync(id);
            if (cat == null)
                return NotFound("Category not found");

            db.Categories.Remove(cat);
            db.SaveChanges();
            return Ok("Deleted successfully");
        }
    }

 }
