using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User.API.Database;
using User.API.DTO;
using User.API.Entity;
using User.API.Repository;

namespace User.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Users1Controller : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public Users1Controller(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/Users1
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers()
        {
            var users = await _userRepository.GetAllAsync();


            var result = users.Select(u => new UserDTO
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName
            });


                return Ok(result);
        }

        // GET: api/Users1/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            var result = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return Ok(result);
        }

        // PUT: api/Users1/5
        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, EditUserDTO dto)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
            {
                return BadRequest();
            }
            
            user.Username = dto.Username;
            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;


            try
            {
                await _userRepository.UpdateAsync(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                var existingUser = await _userRepository.GetByIdAsync(id);
                if (existingUser == user)
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

        // POST: api/Users1
        
        [HttpPost]
        public async Task<ActionResult<UserDTO>> PostUser(CreateUserDTO dto)
        {
            var user = new Entity.User
            {
                Username = dto.Username,
                Password = dto.Password,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };  

            await _userRepository.AddAsync(user);

            var result = new UserDTO
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user); 
        }

        // DELETE: api/Users1/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

           await _userRepository.DeleteAsync(id);

            return NoContent();
        }

       
    }
}
