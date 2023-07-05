using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Novels.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using Novels.Handlers;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http.Extensions;

namespace Novels.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly NovelStoreContext _context;
        private readonly JWTSettings _jwtSettings;
        private readonly PasswordEncryption _passwordencryption;

        public UsersController(NovelStoreContext context, IOptions<JWTSettings> jwtSettings, IOptions<PasswordEncryption> passwordencryption, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _jwtSettings = jwtSettings.Value;
            _passwordencryption = passwordencryption.Value;
            webHostEnvironment = hostEnvironment;
           
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.Include(u => u.UserTypeNavigation).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }
            // Load user type data 
            _context.Entry(user)
                    .Reference(u => u.UserTypeNavigation)
                    .Load();
            _context.Entry(user)
                    .Reference(u => u.Userdetail)
                    .Load();

            return user;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser( RegisterRequest req)
        {
            if (req.user.Password == null)
            {
                return BadRequest(new Dictionary<string, string>() { { "error", "برجاء توفير كلمة المرور" } });
            }
            byte[] hashed_password = Hash.GetHash(req.user.Password, _passwordencryption.value); 
            if (hashed_password == null)
            {
                return BadRequest(new Dictionary<string, string>() { { "error" ,"حدث خطأ بكلمة المرور" } } );
            }
            
            req.user.Password = Convert.ToBase64String(hashed_password);
            _context.Users.Add(req.user);
            await _context.SaveChangesAsync();
            Userdetail detail = new Userdetail();
            detail.Id = req.user.Id;
            detail.Address = req.address;
            _context.Userdetails.Add(detail);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUser), new { id = req.user.Id }, req.user);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserWithToken>> Login([FromBody] User user)
        {
            byte[] compare_password;
            string hased_password;
            try
            {
                compare_password = Hash.GetHash(user.Password, _passwordencryption.value);
                hased_password = Convert.ToBase64String(compare_password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return BadRequest();
            }
            user = await _context.Users.Include(u => u.UserTypeNavigation).Where(u => u.FullName == user.FullName
                                                   && u.Password == hased_password).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound(new Dictionary<string, string>() { { "error","خطا بالاسم او كلمة المرور"} });
            }
            UserWithToken userwithtoken = new UserWithToken(user);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = JWTSettings.GenerateToken(_jwtSettings.Secretkey,user);
            var refresh_token = JWTSettings.GenerateRefreshToken();
            user.RefreshToknes.Add(refresh_token);
            await _context.SaveChangesAsync();
            userwithtoken.Token = tokenHandler.WriteToken(token);
            userwithtoken.RefreshToken = refresh_token.Token;
            return userwithtoken;
        }

        [HttpPost("refresh_token")]
        public async Task<ActionResult<UserWithToken>> RefreshToken([FromBody] RefreshRequest refreshRequest)
        {
            User user = JWTSettings.GetUserFromAccessToken(refreshRequest.AccessToken, _jwtSettings.Secretkey, _context);
            if(user != null && JWTSettings.ValidateRefrestToken(user,refreshRequest.RefreshToken,_context))
            {
                UserWithToken userwithtoken =  new UserWithToken(user);
                var tokenHandler = new JwtSecurityTokenHandler();
                userwithtoken.Token = tokenHandler.WriteToken(JWTSettings.GenerateToken(_jwtSettings.Secretkey,user));
                return userwithtoken;
            }
            return null;
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        [HttpPost("upload")]
        public async Task<ActionResult<string>> uploadImage([FromForm] IFormFile file)
        {
            var req = HttpContext.Request.GetDisplayUrl();
            var index = req.IndexOf("api");
            string res = req.Substring(0, index);
            res = String.Concat(res, "StaticFiles/");
            string name =  UploadFile.Upload(file,webHostEnvironment);
            return String.Concat(res, name);
        }
    }
}
