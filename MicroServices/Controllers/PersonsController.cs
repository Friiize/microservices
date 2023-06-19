using MicroServices.Data;
using MicroServices.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MicroServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly MicroServiceDbContext _context;

        //Injecting our Db context the be used by the controllers
        public PersonsController(MicroServiceDbContext context)
        {
            _context = context;
        }

        // GET: api/Persons
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> GetPersons(string? firstname = null, string? name = null)
        {
            var persons = from p in _context.Persons select p;
            //Check if null
            if (!persons.Any()) return NotFound("Users not found.");
            //Check matching values between firstname or name avoiding case sensitive and can still match any missing part of name/firstname
            if (!string.IsNullOrEmpty(firstname)) persons = persons.Where(p => p.FirstName.ToLower().Contains(firstname.ToLower()));
            if (!string.IsNullOrEmpty(name)) persons = persons.Where(p => p.Name.ToLower().Contains(name.ToLower()));
            

            return await persons.ToListAsync();
        }

        // GET: api/Persons/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Person>> GetPerson(Guid id)
        {
            if (_context.Persons == null) return NotFound("Database not found.");
            
            //Retrieve the person infos by id
            var person = await _context.Persons.FindAsync(id);
        
            //Check if null
            if (person == null) return NotFound("User not found.");
            return person;
            
        }

        // PUT: api/Persons/5
        [HttpPut]
        public async Task<IActionResult> PutPerson(Guid id, string? firstname = null, string? name = null)
        {
            if (_context.Persons == null) return NotFound("Database not found.");
            //Retrieve the person infos by id
            var person = await _context.Persons.FindAsync(id);

            //Check if null
            if (person == null) return NotFound("User not found.");
            //Check if values are null or not before updating the person values
            if (!string.IsNullOrEmpty(firstname))person.FirstName = firstname;
            if (!string.IsNullOrEmpty(name)) person.Name = name;

            //Handling the updating operations and potentials Exceptions
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id)) return NotFound("User not found.");
                throw;
            }
            //Return an empty response with status code of 204
            return NoContent();
        }

        // POST: api/Persons
        //Method that have 2 inputs for the person name and firstname with a generated Guid Id
        [HttpPost]
        public async Task<ActionResult<Person>> PostPerson(string firstname, string name)
        {
            //Check if null
            if (_context.Persons == null) return Problem("Entity set 'MicroServiceDbContext.Persons'  is null.");

            //Create a new Person Model with the values from the POST Attribute
            var person = new Person
            {
                Id = Guid.NewGuid(),
                FirstName = firstname,
                Name = name
            };
            
            //Add the Model to the DB context and wait until the operation is done before sending the code success
            _context.Persons.Add(person);
            await _context.SaveChangesAsync();

            //Send a "Created" Response to indicate it was successful
            return CreatedAtAction("GetPerson", new { id = person.Id }, person);
        }

        // DELETE: api/Persons/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePerson(Guid id)
        {
            //Check if null
            if (_context.Persons == null) return NotFound("Database not found.");
            
            //Find the person with the given id into the db and retrieve it if found
            var person = await _context.Persons.FindAsync(id);
            //Check if null
            if (person == null) return NotFound("User not found.");

            //Remove the Model to the DB context and wait until the operation is done before sending the code success
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            //Return an empty response with status code of 204
            return NoContent();
        }

        private bool PersonExists(Guid id)
        {
            return (_context.Persons?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
