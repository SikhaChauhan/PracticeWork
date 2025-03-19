using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;

namespace ControllerLayer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookBL;

        public AddressBookController(IAddressBookBL addressBookBL)
        {
            _addressBookBL = addressBookBL;
        }

        // GET: api/addressbook
        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _addressBookBL.GetAllContactsAsync();
            return Ok(contacts);
        }

        // GET: api/addressbook/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _addressBookBL.GetContactByIdAsync(id);
            if (contact == null) return NotFound(new { message = "Contact not found" });

            return Ok(contact);
        }

        // POST: api/addressbook
        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] AddressBookDTO contactDto)
        {
            var newContact = await _addressBookBL.CreateContactAsync(contactDto);
            return CreatedAtAction(nameof(GetContactById), new { id = newContact.Id }, newContact);
        }

        // PUT: api/addressbook/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] AddressBookDTO contactDto)
        {
            var updatedContact = await _addressBookBL.UpdateContactAsync(id, contactDto);
            if (updatedContact == null) return NotFound(new { message = "Contact not found" });

            return Ok(updatedContact);
        }

        // DELETE: api/addressbook/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var isDeleted = await _addressBookBL.DeleteContactAsync(id);
            if (!isDeleted) return NotFound(new { message = "Contact not found" });

            return NoContent();
        }
    }
}
