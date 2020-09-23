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
    public class ProductUpdateRequestTest
    {
        private ProductUpdateRequest request;

        [TestInitialize]
        public void InitialData()
        {
            request = new ProductUpdateRequest() {
                Username = "test",
                Products = new List<string>() {
                    "Reconciliation",
                    "Split"
                }
            };
        }

        [TestMethod]
        public void Should_Not_Throw()
        {
            request.ValidateProducts();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Products_Is_Null()
        {
            request.Products = null;
            request.ValidateProducts();
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public void Should_Throw_UserManagementException_When_Products_Is_Empty()
        {
            request.Products = new List<string>();
            request.ValidateProducts();
        }
    }
}
