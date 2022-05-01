using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SampleContact.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SampleContact.Repository
{
    public class ContactDbRepository : IContactDbRepository
    {
        private readonly string _contactTableName;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly IAmazonDynamoDB _dynamoDBClient;

        public ContactDbRepository(IConfiguration configuration,
            ILogger<ContactDbRepository> logger,
            IAmazonDynamoDB amazonDynamoDB)
        {
            _configuration = configuration;
            _logger = logger;
            _dynamoDBClient = amazonDynamoDB;
            _contactTableName = configuration["ContactTableName"];
        }

        public async Task<bool> AddAsync(ContactFormModel contactForm, string Id)
        {
            var request = new PutItemRequest
            {
                TableName = _contactTableName,
                Item = new Dictionary<string, AttributeValue>()
                {
                    {"ID", new AttributeValue {S = Id} },
                    {"IP", new AttributeValue {S = contactForm.IP} },
                    {"Name", new AttributeValue {S = contactForm.Name} },
                    {"Email", new AttributeValue {S = contactForm.Email} },
                    {"Phone", new AttributeValue {S = contactForm.Phone} },
                    {"Comments", new AttributeValue {S = contactForm.Comments} },
                    {"DateTimeUTC", new AttributeValue {S = DateTime.UtcNow.ToString("o")} }
                }
            };

            var response = await _dynamoDBClient.PutItemAsync(request);

            if (response.HttpStatusCode != HttpStatusCode.OK)
            {
                _logger.LogWarning($"Contact failed to save. Response details: {response.ResponseMetadata}");
            }

            return response.HttpStatusCode == HttpStatusCode.OK;
        }
    }
}
