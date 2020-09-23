using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Access.Auth.Service.Domain.Configuration;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Business.Validation;
using Access.Auth.Service.Infra.Store;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using NSubstitute;
using System.Security.Claims;
using System.Linq;

namespace Access.Auth.Service.Test
{
    [TestClass]
    public class ClientCredentialsValidatorTest
    {
        private IAuthConfiguration authConfiguration = Substitute.For<IAuthConfiguration>();
        private CustomTokenRequestValidationContext context = Substitute.For<CustomTokenRequestValidationContext>();

        private ClientCredentialsRequestValidator clientValidator;
        private Client client;
        private ValidatedTokenRequest validatedTokenRequestWithNullRaw;
        private ValidatedTokenRequest validatedTokenRequestWithRaw;
        private TokenRequestValidationResult tokenRequestValidationResultWithNullRaw;
        private TokenRequestValidationResult tokenRequestValidationResultRaw;
        private TokenRequestValidationResult invalidEmailResult;
        private TokenRequestValidationResult clientValidationErrorResult;
        private Claim claim;

        [TestInitialize]
        public void InitialData()
        {
            this.clientValidator = new ClientCredentialsRequestValidator(this.authConfiguration);
            this.client = new Client {
                ClientId = "1"
            };
            this.validatedTokenRequestWithNullRaw = new ValidatedTokenRequest {
                Client = this.client,
                Raw = null
            };
            this.validatedTokenRequestWithRaw = new ValidatedTokenRequest {
                Client = this.client,
                Raw = new NameValueCollection()
            };
            this.invalidEmailResult = new TokenRequestValidationResult(null, "invalid_email");
            this.clientValidationErrorResult = new TokenRequestValidationResult(null, "client_validation_error");
            this.tokenRequestValidationResultWithNullRaw = new TokenRequestValidationResult(this.validatedTokenRequestWithNullRaw);
            this.tokenRequestValidationResultRaw = new TokenRequestValidationResult(this.validatedTokenRequestWithRaw);
            this.claim = new Claim("email", "gu@matos.com");
        }

        [TestMethod]
        public void When_email_field_in_the_TokenRequest_is_null_results_in_a_invalid_email_error()
        {
            this.authConfiguration.ExternalValidationClientId.Returns("1");
            this.context.Result = this.tokenRequestValidationResultWithNullRaw;

            this.clientValidator.ValidateAsync(this.context);

            Assert.AreEqual(this.invalidEmailResult.Error, this.context.Result.Error);
        }

        [TestMethod]
        public void When_email_field_in_the_TokenRequest_is_valid_adds_new_claim()
        {
            this.validatedTokenRequestWithRaw.Raw.Add("extra_email", "gu@matos.com");
            this.authConfiguration.ExternalValidationClientId.Returns("1");
            this.context.Result = this.tokenRequestValidationResultRaw;

            this.clientValidator.ValidateAsync(this.context);

            var contextClaims = this.context.Result.ValidatedRequest.ClientClaims.AsEnumerable()
                .Where(c => c.Type == "email" && c.Value == "gu@matos.com")
                .FirstOrDefault();

            Assert.AreEqual(this.claim.Value, contextClaims.Value);
        }
    }
}
