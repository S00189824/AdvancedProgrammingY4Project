using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SampleContact.Models;
using SampleContact.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleContact.Services
{
    public class ContactService : IContactService
    {
        private readonly IContactDbRepository _contactDbRepository;
        private readonly IContactQueueRepository _contactQueueRepository;
        private readonly ILogger _logger;
        private IConfiguration _configuration;


        public ContactService(IConfiguration configuration,
            ILogger<ContactService> logger,
            IContactDbRepository contactDbRepository,
            IContactQueueRepository contactQueueRepository)
        {
            _logger = logger;
            _contactQueueRepository = contactQueueRepository;
            _contactDbRepository = contactDbRepository;
            _configuration = configuration;
        }

        public async Task<bool> AddAsync(ContactFormModel contactForm)
        {
            var id = Guid.NewGuid().ToString();

            _ = _contactDbRepository.AddAsync(contactForm, id);
            await _contactQueueRepository.AddAsync(id, _configuration["SendMailQueueUrl"]);

            return true;
        }
    }
}
