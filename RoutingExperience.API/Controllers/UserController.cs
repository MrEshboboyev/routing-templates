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
        public async Task<IActionResult> GetById([FromRoute] int userId)
        {
            return Ok(_context.Users.FirstOrDefault(u => u.UserId == userId));
        }
        
        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            return Ok(_context.Users.FirstOrDefault(u => u.Email == email));
        }
        
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPost("createWithForm")]
        public async Task<IActionResult> CreateWithForm([FromForm] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        // this action is not working as expected;
        // [FromQuery] using Get() methods
        [HttpPost("createWithQuery")]
        public async Task<IActionResult> CreateWithQuery([FromQuery] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPost("createWithRoute")]
        public async Task<IActionResult> CreateWithRoute([FromRoute] User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        
        [HttpPost("createWithHeader")]
        public async Task<IActionResult> CreateWithHeader([FromHeader] int userId,
            [FromHeader] string fullName,
            [FromHeader] string password,
            [FromHeader] string email)
        {
            var user = new User()
            {
                UserId = userId,
                FullName = fullName,
                Password = password,
                Email = email
            };

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
