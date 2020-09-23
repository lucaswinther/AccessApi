using System;
using System.Collections.Generic;
using Access.Auth.Service.Domain.Request;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Access.Auth.Service.Domain.UserManagement;

namespace Access.Auth.Service.Test
{
    [TestClass]
    public class RoleUpdateRequestTest
    {
        private RoleUpdateRequest request;

        [TestInitialize]
        public void InitialData()
        {
            request = new RoleUpdateRequest() {
                Username = "test",
                Roles = new List<string>() {
                    Roles.Admin,
                    Roles.Analyst,
                    Roles.User
                }
            };
        }

        [TestMethod]
        public void Should_Not_Throw()
        {
            request.ValidateRoles();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Roles_Is_Null()
        {
            request.Roles = null;
            request.ValidateRoles();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Roles_Is_Empty()
        {
            request.Roles = new List<string>();
            request.ValidateRoles();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Role_Doesnt_Exist()
        {
            request.Roles = new List<string>() {
                Roles.Admin,
                Roles.Analyst,
                Roles.User,
                "Fake"
            };

            request.ValidateRoles();
        }
    }
}
