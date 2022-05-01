using SampleContact.Models;
using System.Threading.Tasks;

namespace SampleContact.Repository
{
    public interface IContactDbRepository
    {
        Task<bool> AddAsync(ContactFormModel contactForm, string Id);
    }
}