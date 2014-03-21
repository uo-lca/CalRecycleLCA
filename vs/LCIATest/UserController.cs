using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LCIATest
{
    public class UserController : ApiController
    {
         static IUserRepository repository = new UserRepository();

        public IEnumerable<Users> GetAllUsers()
        {
            var users = repository.GetAllUsers();
            return users;
        }

        public Users GetUserById(int id)
        {
            var user = repository.GetUserById(id);
            return user;
        }
        public string AddUser(Users user)
        {  var response = repository.AddUser(user);
            return response;
        }
        public void DeleteUser(int id)
        {
            repository.DeleteUser(id);
        }
    }
}