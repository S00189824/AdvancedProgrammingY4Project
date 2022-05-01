using SampleContact.Models;
using System.Threading.Tasks;

namespace SampleContact.Services
{
    public interface IContactService
    {
        Task<bool> AddAsync(ContactFormModel contactForm);
    }
}