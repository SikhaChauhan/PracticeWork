using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;

namespace ControllerLayer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AddressBookController : ControllerBase
    {
        private readonly IAddressBookBL _addressBookService;

        public AddressBookController(IAddressBookBL addressBookService)
        {
            _addressBookService = addressBookService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _addressBookService.GetAllContactsAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactById(int id)
        {
            var contact = await _addressBookService.GetContactByIdAsync(id);
            if (contact == null) return NotFound("Contact not found.");
            return Ok(contact);
        }

        [HttpPost]
        public async Task<IActionResult> CreateContact([FromBody] AddressBookEntity contact)
        {
            try
            {
                var newContact = await _addressBookService.AddContactAsync(contact);
                return CreatedAtAction(nameof(GetContactById), new { id = newContact.Id }, newContact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateContact(int id, [FromBody] AddressBookEntity contact)
        {
            var updatedContact = await _addressBookService.UpdateContactAsync(id, contact);
            if (updatedContact == null) return NotFound("Contact not found.");
            return Ok(updatedContact);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact(int id)
        {
            var isDeleted = await _addressBookService.DeleteContactAsync(id);
            if (!isDeleted) return NotFound("Contact not found.");
            return Ok("Contact deleted successfully.");
        }
    }
}
