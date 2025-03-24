using BusinessLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using ModelLayer.DTO;
using ModelLayer.Model;
using Moq;
using NUnit.Framework;
using PracticeWork.Controllers;

namespace Testing
{
    [TestFixture]
    public class AddressBookControllerTests
    {
        private Mock<IAddressBookBL> _mockAddressBookBL;
        private AddressBookController _controller;

        [SetUp]
        public void Setup()
        {
            _mockAddressBookBL = new Mock<IAddressBookBL>();
            _controller = new AddressBookController(_mockAddressBookBL.Object);
        }

        [Test]
        public async Task GetAllContacts_ReturnsOk_WithListOfContacts()
        {
            // Arrange
            var contacts = new List<AddressBookEntity>
            {
                new AddressBookEntity { Id = 1, Name = "John Doe", Email = "john@example.com" },
                new AddressBookEntity { Id = 2, Name = "Jane Doe", Email = "jane@example.com" }
            };

            _mockAddressBookBL.Setup(bl => bl.GetAllContactsAsync()).ReturnsAsync(contacts);

            // Act
            var result = await _controller.GetAllContacts();

            // Assert
            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult?.Value, Is.EqualTo(contacts));
        }

        [Test]
        public async Task GetContactById_ExistingId_ReturnsOk()
        {
            var contact = new AddressBookEntity { Id = 1, Name = "John Doe", Email = "john@example.com" };
            _mockAddressBookBL.Setup(bl => bl.GetContactByIdAsync(1)).ReturnsAsync(contact);

            var result = await _controller.GetContactById(1);

            Assert.That(result.Result, Is.InstanceOf<OkObjectResult>());
            var okResult = result.Result as OkObjectResult;
            Assert.That(okResult, Is.Not.Null);
            Assert.That(okResult?.Value, Is.EqualTo(contact));
        }

        [Test]
        public async Task GetContactById_NonExistingId_ReturnsNotFound()
        {
            _mockAddressBookBL.Setup(bl => bl.GetContactByIdAsync(99)).ReturnsAsync((AddressBookEntity)null);

            var result = await _controller.GetContactById(99);

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task UpdateContact_ExistingId_ReturnsNoContent()
        {
            var updatedContact = new AddressBookEntity { Id = 1, Name = "John Updated", Email = "john@example.com" };
            _mockAddressBookBL.Setup(bl => bl.UpdateContactAsync(1, updatedContact)).ReturnsAsync(updatedContact);

            var result = await _controller.UpdateContact(1, updatedContact);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task UpdateContact_NonExistingId_ReturnsNotFound()
        {
            _mockAddressBookBL.Setup(bl => bl.UpdateContactAsync(99, It.IsAny<AddressBookEntity>())).ReturnsAsync((AddressBookEntity)null);

            var result = await _controller.UpdateContact(99, new AddressBookEntity());

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task DeleteContact_ExistingId_ReturnsNoContent()
        {
            _mockAddressBookBL.Setup(bl => bl.DeleteContactAsync(1)).ReturnsAsync(true);

            var result = await _controller.DeleteContact(1);

            Assert.That(result, Is.InstanceOf<NoContentResult>());
        }

        [Test]
        public async Task DeleteContact_NonExistingId_ReturnsNotFound()
        {
            _mockAddressBookBL.Setup(bl => bl.DeleteContactAsync(99)).ReturnsAsync(false);

            var result = await _controller.DeleteContact(99);

            Assert.That(result, Is.InstanceOf<NotFoundResult>());
        }
    }
}
