using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {
        private readonly ApplicationDBContext _db;

        public ProductController(ApplicationDBContext db)
        {
            _db = db;
        }


        [HttpGet("[action]")]
        [Authorize(Policy = "RequiredLoggedIn")]
        public IActionResult GetProducts()
        {
            return Ok(_db.Products.ToList());
        }


        [HttpPost("[action]")]
        [Authorize(Policy = "RequiredAdministratorRole")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel formData)
        {
            var newProduct = new ProductModel
            {
                Name = formData.Name,
                ImageUrl = formData.ImageUrl,
                Description = formData.Description,
                OutOfStock = formData.OutOfStock,
                Price = formData.Price
            };

            await _db.Products.AddAsync(newProduct);

            await _db.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequiredAdministratorRole")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductModel formData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var findProduct = _db.Products.FirstOrDefault(p => p.ProductId == id);

            if (findProduct == null)
            {
                return NotFound();
            }

            findProduct.Name = formData.Name;
            findProduct.Description = formData.Description;
            findProduct.ImageUrl = formData.ImageUrl;
            findProduct.OutOfStock = formData.OutOfStock;
            findProduct.Price = formData.Price;

            _db.Entry(findProduct).State = EntityState.Modified;

            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The Product with Id " + id + " is updated"));
        }

        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequiredAdministratorRole")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // find the product

            var findProduct = await _db.Products.FindAsync(id);

            if (findProduct == null)
            {
                return NotFound();
            }

            _db.Products.Remove(findProduct);

            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The Product with id " + id + " is Deleted."));
        }

    }
}