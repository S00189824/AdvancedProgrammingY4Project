using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleContact.Repository
{
    public class ContactQueueRepository : IContactQueueRepository
    {
        private readonly IAmazonSQS _sqsClient;
        private readonly ILogger _logger;

        public ContactQueueRepository(ILogger<ContactQueueRepository> logger, IAmazonSQS sqsClient)
        {
            _logger = logger;
            _sqsClient = sqsClient;
        }

        public async Task<bool> AddAsync(string contactId, string sendMailQueueUrl)
        {
            var sendRequest = new SendMessageRequest
            {
                QueueUrl = sendMailQueueUrl,
                MessageBody = $"{{'ContactId' : {JsonConvert.SerializeObject(contactId)} }}"
            };

            var response = await _sqsClient.SendMessageAsync(sendRequest);

            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
