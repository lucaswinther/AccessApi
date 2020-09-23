using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Access.Auth.Service.Domain.Request;

namespace Access.Auth.Service.Test
{
    [TestClass]
    public class AddClientRequestTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void When_client_name_is_null_should_throw_exception()
        {
          // Arrange >> Act
          var request = new AddClientRequest(
            null,
            new List<string>{"client_credentials"},
            new List<string>{"allowed_scope"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void When_grant_type_is_null_should_throw_exception()
        {
          // Arrange >> Act
          var request = new AddClientRequest(
            "test_client",
            null,
            new List<string>{"allowed_scope"});
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void When_allowed_scope_is_null_should_throw_exception()
        {
          // Arrange >> Act
          var request = new AddClientRequest(
            "test_client",
            new List<string>{"test_scope"},
            null);
        }

        [TestMethod]
        public void When_clientId_is_not_provided_a_random_id_is_generated()
        {
          // Arrange >> Act
          var request = new AddClientRequest(
            "test_client",
            new List<string>{"test_scope"},
            new List<string>{"client_credential"});

          // Assert
          Assert.IsNotNull(request.ClientId);
        }
        
        [TestMethod]
        public void When_clientSecret_is_not_provided_a_random_secreted_is_generated()
        {
          // Arrange >> Act
          var request = new AddClientRequest(
            "test_client",
            new List<string>{"test_scope"},
            new List<string>{"client_credential"},
            clientId: "test_client_id");

          // Assert
          Assert.IsNotNull(request.ClientSecret);
        }
    }
}