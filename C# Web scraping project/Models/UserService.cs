using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharp_Web_scraping_project.Models;

namespace C__Web_scraping_project.Models
{
    public class UserService
    {
        public bool RegisterUser(string username, string password)
        {
            if (username.Length < 5 || password.Length < 8)
            {
                Console.WriteLine("Username must be at least 5 characters and password at least 8 characters long.");
                return false;
            }
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            try
            {
                using (var db = new UserDbContext())
                {
                    db.tblUsers.Add(new User { Username = username, PasswordHash = passwordHash });
                    db.SaveChanges();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool AuthenticateUser(string username, string password)
        {
            try
            {
                using (var db = new UserDbContext())
                {
                    User user = db.tblUsers.FirstOrDefault(user => user.Username == username);

                    if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                    {
                        return true;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }
    }
}
