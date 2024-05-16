using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Data;
using WebApplication1.Models;

namespace EntityFrameworkCore.MySQL.Controllers {
    [Route( "api/[controller]" )]
    [ApiController]
    public class ProductsController : ControllerBase {
        private readonly AppDbContext _appDbContext;
        public ProductsController( AppDbContext appDbContext ) {
            _appDbContext = appDbContext;
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct( Product product ) {
            _appDbContext.Products.Add( product );
            await _appDbContext.SaveChangesAsync();

            return Ok( product );
        }
    }
}