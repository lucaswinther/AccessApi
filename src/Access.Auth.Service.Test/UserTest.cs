using Microsoft.VisualStudio.TestTools.UnitTesting;
using Access.Auth.Service.Domain.UserManagement;
using System;
using System.Collections.Generic;

namespace Access.Auth.Service.Test
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void When_Id_is_invalid_return_Exception()
        {
            var user = new User {
                Id = null
            };
            
            user.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void When_Nickname_is_invalid_return_Exception()
        {
            var user = new User {
                Id = "1",
                Nickname = " "
            };

            user.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void When_Username_is_invalid_return_Exception()
        {
            var user = new User {
                Id = "1",
                Nickname = "a",
                Username = " "
            };

            user.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void When_Password_is_invalid_return_Exception()
        {
            var user = new User {
                Id = "1",
                Nickname = "a",
                Username = "a",
                Password = null
            };

            user.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void When_PasswordLength_is_invalid_return_Exception()
        {
            var user = new User {
                Id = "1",
                Nickname = "a",
                Username = "a",
                Password = "ola"
            };

            user.Validate();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void When_Roles_is_invalid_return_Exception()
        {
            var user = new User {
                Id = "1",
                Nickname = "a",
                Username = "a",
                Password = "123456",
                Email = "asd@asd.com",
                Roles = new List<string> {

                }
            };
            
            user.Validate();            
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void When_Email_is_invalid_return_Exception()
        {
            var user = new User {
                Id = "1",
                Nickname = "a",
                Username = "a",
                Password = "123456",
                Roles = new List<string> {
                    "Admin"
                },
                Email = "#"
            };

            user.Validate();
        }
    }
}
