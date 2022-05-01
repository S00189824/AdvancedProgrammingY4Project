using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SampleContact;
using SampleContact.Models;
using SampleContact.Controllers;
using SampleContact.Services;

namespace SampleContact.Controllers
{
    public class Contact : Controller
    {
        private IConfiguration _configurable;
        private readonly ILogger _logger;
        private IContactService _contactService;

        public Contact(IConfiguration configuration, ILogger logger, IContactService contactService)
        {
            _configurable = configuration;
            _logger = logger;
            _contactService = contactService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([Bind("Name,Email,Phone,Comments")] ContactFormModel model)
        {
            model.IP = Common.ResolveIPAddress(HttpContext);

            await _contactService.AddAsync(model);

            _logger.LogInformation($"Contact added to queue. {model.LogSerialized}");

            return RedirectToAction("Index");
        }
    }
}
