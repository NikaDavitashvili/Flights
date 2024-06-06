using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Flights.Dtos;
using Flights.ReadModels;
using Flights.Domain.Entities;
using Flights.Data;
using System.Text;
using System.Security.Cryptography;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using FinalProject.Domain.Entities;

namespace FinalProject.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class PassengerController : ControllerBase
    {
        private readonly Entities _entities;

        public PassengerController(Entities entities)
        {
            _entities = entities;
        }


        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public IActionResult Register(NewPassengerDto dto)
        {
            var password = PasswordHash(dto.Password);
            var gender = dto.Gender ? "Female" : "Male";
            _entities.Passengers.Add(new Passenger(
                dto.Email,
                password,
                dto.UserName,
                dto.FirstName,
                dto.LastName,
                gender
                ));

            _entities.SaveChanges();

            return Ok();
        }

        [HttpPost("{email}&{password}")]
        public ActionResult<User> Login(string email, string password)
        {
            var passwordHash = PasswordHash(password);
            var passenger = _entities.Passengers.FirstOrDefault(p => p.Email == email && p.PasswordHash == passwordHash);

            if (passenger == null)
                return NotFound();

            var rm = new User(
                passenger.Email,
                passenger.PasswordHash,
                passenger.UserName
            );

            return Ok(rm);
        }

        private string PasswordHash(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
