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
    public class CnpjUpdateRequestTest
    {
        private CnpjUpdateRequest request;

        [TestInitialize]
        public void InitialData()
        {
            request = new CnpjUpdateRequest() {
                Username = "test",
                Cnpjs = new List<string>() {
                    "00000000000000",
                    "11111111111111"
                }
            };
        }

        [TestMethod]
        public void Should_Not_Throw()
        {
            request.ValidateCnpjs();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Cnpjs_Is_Null()
        {
            request.Cnpjs = null;
            request.ValidateCnpjs();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Cnpjs_Is_Empty()
        {
            request.Cnpjs = new List<string>();
            request.ValidateCnpjs();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Cnpjs_Is_Short()
        {
            request.Cnpjs = new List<string>() { "00000000000" };
            request.ValidateCnpjs();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Cnpjs_Is_Long()
        {
            request.Cnpjs = new List<string>() { "000000000000000" };
            request.ValidateCnpjs();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Cnpjs_Has_Invalid_Character()
        {
            request.Cnpjs = new List<string>() { "000000A0000000" };
            request.ValidateCnpjs();
        }
    }
}
