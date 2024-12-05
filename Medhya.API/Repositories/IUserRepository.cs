using Medhya.API.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Medhya.API.Model.User;

namespace Medhya.API.Repositories
{
    public interface IUserRepository
    {
        Task<ResponeMessage> CreateUser(User user); // Method to create a new user
        Task<ResponeMessage> InsertUserAddress(int userId, IEnumerable<UserAddress> addressDetails); // Add addresses for a user
        Task<ResponeMessage> UpdateUserAddress(int addressId, int userId, IEnumerable<UserAddress> addressDetails); // Update user addresses
        Task<ResponeMessage> UpdateUser(User user); // Update user details (currently not implemented)
    }
}
