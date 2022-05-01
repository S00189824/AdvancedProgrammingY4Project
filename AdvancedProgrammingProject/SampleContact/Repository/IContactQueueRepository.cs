using System.Threading.Tasks;

namespace SampleContact.Repository
{
    public interface IContactQueueRepository
    {
        Task<bool> AddAsync(string contactId, string sendMailQueueUrl);
    }
}