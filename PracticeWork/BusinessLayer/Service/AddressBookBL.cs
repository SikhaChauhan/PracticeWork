using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Interface;
using StackExchange.Redis;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BusinessLayer.Service
{
    public class AddressBookBL : IAddressBookBL
    {
        private readonly IAddressBookRL _addressBookRL;
        private readonly IDatabase _cache;
        private readonly IConnection _rabbitMqConnection;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(10);

        public AddressBookBL(IAddressBookRL addressBookRL, IConnectionMultiplexer redis, IConnection rabbitMqConnection)
        {
            _addressBookRL = addressBookRL ?? throw new ArgumentNullException(nameof(addressBookRL));
            _cache = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
            _rabbitMqConnection = rabbitMqConnection ?? throw new ArgumentNullException(nameof(rabbitMqConnection));
        }

        // Get all contacts (with caching)
        public async Task<IEnumerable<AddressBookEntity>> GetAllContactsAsync()
        {
            string cacheKey = "contacts:all";
            var cachedData = await _cache.StringGetAsync(cacheKey);

            if (!cachedData.IsNullOrEmpty)
                return DeserializeCache<IEnumerable<AddressBookEntity>>(cachedData) ?? new List<AddressBookEntity>();

            var contacts = await _addressBookRL.GetAllContactsAsync();
            await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(contacts), _cacheExpiration);

            return contacts;
        }

        // Get contact by ID (with caching)
        public async Task<AddressBookEntity?> GetContactByIdAsync(int id)
        {
            string cacheKey = $"contact:{id}";
            var cachedData = await _cache.StringGetAsync(cacheKey);

            if (!cachedData.IsNullOrEmpty)
                return DeserializeCache<AddressBookEntity>(cachedData);

            var contact = await _addressBookRL.GetContactByIdAsync(id);
            if (contact != null)
            {
                await _cache.StringSetAsync(cacheKey, JsonSerializer.Serialize(contact), _cacheExpiration);
            }

            return contact;
        }

        // Add new contact (invalidate cache and publish to RabbitMQ)
        public async Task<AddressBookEntity> AddContactAsync(AddressBookEntity contact)
        {
            if (contact == null) throw new ArgumentNullException(nameof(contact));

            var newContact = await _addressBookRL.AddContactAsync(contact);

            await _cache.KeyDeleteAsync("contacts:all");  // Invalidate cache
            PublishMessage("contact.added", newContact);

            return newContact;
        }

        // Update contact (update cache and publish event)
        public async Task<AddressBookEntity?> UpdateContactAsync(int id, AddressBookEntity updatedContact)
        {
            if (updatedContact == null) throw new ArgumentNullException(nameof(updatedContact));

            var contact = await _addressBookRL.UpdateContactAsync(id, updatedContact);
            if (contact != null)
            {
                await _cache.StringSetAsync($"contact:{id}", JsonSerializer.Serialize(contact), _cacheExpiration);
                await _cache.KeyDeleteAsync("contacts:all");
                PublishMessage("contact.updated", contact);
            }

            return contact;
        }

        // Delete contact (remove from cache and publish event)
        // Delete contact (remove from cache and publish event)
        public async Task<bool> DeleteContactAsync(int id)
        {
            var isDeleted = await _addressBookRL.DeleteContactAsync(id);
            if (isDeleted)
            {
                // Delete both cache keys directly without using a batch
                await _cache.KeyDeleteAsync($"contact:{id}");
                await _cache.KeyDeleteAsync("contacts:all");

                PublishMessage("contact.deleted", id);
            }

            return isDeleted;
        }




        // RabbitMQ Message Publisher
        private void PublishMessage<T>(string routingKey, T message)
        {
            if (_rabbitMqConnection?.IsOpen != true)
            {
                Console.WriteLine("RabbitMQ connection is closed.");
                return;
            }

            using var channel = _rabbitMqConnection.CreateModel();
            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
            channel.BasicPublish(exchange: "events", routingKey: routingKey, body: body);
        }

        // Helper: Deserialize cache safely
        private T? DeserializeCache<T>(RedisValue cachedData)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch
            {
                return default;
            }
        }
    }
}
