using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RoutingExperience.API.Data;
using RoutingExperience.API.Models;

namespace RoutingExperience.API.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_context.Users.ToList());
        }
        
        [HttpGet("{userId:int:min(1)}")]
        public async Task<IActionResult> GetById(int userId)
        {
            return Ok(_context.Users.FirstOrDefault(u => u.UserId == userId));
        }
        
        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            return Ok(_context.Users.FirstOrDefault(u => u.Email == email));
        }
        
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPut]
        public async Task<IActionResult> Update(int userId, User user)
        {
            if (userId != user.UserId)
            {
                return BadRequest("userId and User.UserId must be equal!");
            }

            var userFromDb = _context.Users.FirstOrDefault(u => u.UserId != user.UserId);
            if (userFromDb != null)
            {
                return NotFound();
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpDelete]
        public async Task<IActionResult> Delete(int userId)
        {
            var user = _context.Users.FirstOrDefault(x => x.UserId == userId);

            if (user is null)
            {
                return NotFound();
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
