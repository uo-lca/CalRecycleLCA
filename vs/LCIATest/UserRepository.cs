using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace LCIATest
{
    public class UserRepository : IUserRepository
    {
         private List<Users> userList = new List<Users>();
        private int _id = 3;
        public UserRepository()
        {
 userList.Add(new Users { Id = 1, FirstName = "Madhur", LastName = "Kapoor", Company = "ABC", Email = "madhur@abc.com", PhoneNo = "65431546" });
userList.Add(new Users { Id = 2, FirstName = "Alan", LastName = "Wake", Company = "XYZ Corp", Email = "alan@xyz.com", PhoneNo = "64649879" });
        }
        public IEnumerable<Users> GetAllUsers()
        {
            return userList;
        }
        public string AddUser(Users user)
        {
            user.Id = _id++;
            userList.Add(user);
            return "User added";
        }
        public Users GetUserById(int id)
        {
            var user = userList.FirstOrDefault<Users>((p) => p.Id == id);
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }
            return user;
        }
        public void DeleteUser(int id)
        {
            var user = userList.FirstOrDefault<Users>((p) => p.Id == id);

            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            userList.Remove(user);
        }
    }
}