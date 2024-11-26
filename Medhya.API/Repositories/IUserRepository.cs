using Medhya.API.Model;

namespace Medhya.API.Repositories
{
    public interface IUserRepository
    {
        public Task<ResponeMessage> CreateUser(User user);
        public Task<int> UpdateUser(User user);


    }
}
