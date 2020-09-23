using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using NSubstitute;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Infra.Store;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Access.Auth.Service.Business.Validation;
using Access.Auth.Service.Infra.Authentication;
using System.Linq;

namespace Access.Auth.Service.Test
{
    [TestClass]
    public class ResourceOwnerPasswordValidatorTest
    {
        private IAuthenticationRepository authenticationRepository = Substitute.For<IAuthenticationRepository>();
        private ResourceOwnerPasswordValidationContext resourceOwnerPasswordValidationContext = Substitute.For<ResourceOwnerPasswordValidationContext>();
        private IUserStore userStore = Substitute.For<IUserStore>();

        private ResourceOwnerPasswordValidator passwordValidator;
        private User userWithoutClaims;
        private User userWithClaims;
        private User userWitHashedPassword;
        private GrantValidationResult grantValidationResultError;
        private GrantValidationResult additionalInfoResult;
        private GrantValidationResult grantValidationResultWhenExceptionIsThrown;

        [TestInitialize]
        public void InitialData()
        {
            this.passwordValidator = new ResourceOwnerPasswordValidator(userStore);

            this.userWithoutClaims = new User {
                Id = "1",
                Nickname = "gustavo.matos",
                Username = "gu",
                Roles = new List<string> {
                    "Admin",
                    "Analyst"
                },
                Email = "eae@cappta.com.br",
                Password = "123456"
            };

            this.userWitHashedPassword = new User {
                Id = "1",
                Nickname = "gustavo.matos",
                Username = "gu",
                Roles = new List<string> {
                    "Admin",
                    "Analyst"
                },
                Email = "eae@cappta.com.br",
                Password = BCrypt.Net.BCrypt.HashPassword("123456")
            };

            this.userWithClaims = new User {
                Id = "1",
                Nickname = "gustavo.matos",
                Username = "gu",
                Roles = new List<string> {
                    "Admin",
                    "Analyst"
                },
                Email = "eae@cappta.com.br",
                Password = BCrypt.Net.BCrypt.HashPassword("123456"),
                Claims = new Dictionary<string, IEnumerable<string>>() { { "TESTE", new List<string>() { "Hahahah" } } }
            };

            this.grantValidationResultError = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_user_credentials");
            this.grantValidationResultWhenExceptionIsThrown = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "user_validation_error");
            this.additionalInfoResult = new GrantValidationResult(
                    subject: userWithClaims.Id,
                    authenticationMethod: "custom",
                    claims: ResourceOwnerPasswordValidator.GetUserClaims(userWithClaims)
            );
        }

        [TestMethod]
        public void When_UserClaims_is_null_return_Claims_array_of_length_4_plus_UserRolesCount()
        {
            var claimsArray = ResourceOwnerPasswordValidator.GetUserClaims(this.userWithoutClaims);

            Assert.AreEqual(4 + this.userWithClaims.Roles.Count, claimsArray.Length);
        }

        [TestMethod]
        public void When_UserClaims_is_not_null_return_Claims_array_of_length_4_plus_UserRolesCount_plus_UserClaimsCount()
        {
            var listaProdutos = new List<string> {
                "Mensagem"
            };

            this.userWithClaims.Claims.Add("Produtos", listaProdutos);

            var claimsArray = ResourceOwnerPasswordValidator.GetUserClaims(this.userWithClaims);

            Assert.AreEqual(4 + this.userWithClaims.Roles.Count + this.userWithClaims.Claims.Count, claimsArray.Length);
        }

        [TestMethod]
        public async Task When_User_is_null_creates_GrantValidationResult_error()
        {
            this.userStore.FindUserByUsernameOrNicknameAsync(Arg.Any<string>()).Returns<User>(e => null);

            await this.passwordValidator.ValidateAsync(this.resourceOwnerPasswordValidationContext);

            Assert.AreEqual(this.grantValidationResultError.Error, resourceOwnerPasswordValidationContext.Result.Error);
            Assert.AreEqual(this.grantValidationResultError.ErrorDescription, resourceOwnerPasswordValidationContext.Result.ErrorDescription);
        }

        [TestMethod]
        public async Task When_ContextPassword_is_different_from_UserPassword_creates_GrantValidationResult_error()
        {
            this.userStore.FindUserByUsernameOrNicknameAsync(Arg.Any<string>()).Returns(this.userWitHashedPassword);

            this.resourceOwnerPasswordValidationContext.Password = "2";
            
            await this.passwordValidator.ValidateAsync(this.resourceOwnerPasswordValidationContext);

            Assert.AreEqual(this.grantValidationResultError.Error, this.resourceOwnerPasswordValidationContext.Result.Error);
            Assert.AreEqual(this.grantValidationResultError.ErrorDescription, this.resourceOwnerPasswordValidationContext.Result.ErrorDescription);
        }

        [TestMethod]
        public void When_User_and_ContextPassword_are_valid_creates_valid_GrantValidationResult()
        {
            var userClaims = ResourceOwnerPasswordValidator.GetUserClaims(userWithClaims);

            this.userWithClaims.Claims.Add("username", new List<string>() { this.userWithClaims.Username });
            this.userWithClaims.Claims.Add("sub", new List<string>() { this.userWithClaims.Id });
            this.userWithClaims.Claims.Add("role", this.userWithClaims.Roles);
            this.userWithClaims.Claims.Add("email", new List<string>() { this.userWithClaims.Email });

            foreach (var cc in this.userWithClaims.Claims)
            {
                var match = userClaims.Where(e => e.Type == cc.Key);
                Assert.IsTrue(match.Where(e => cc.Value.Contains(e.Value)).Any());
            }
        }

        [TestMethod]
        public async Task When_a_Exception_is_thrown_creates_GrantValidationResult_error()
        {
            this.userStore.FindUserByUsernameOrNicknameAsync(Arg.Any<string>()).Returns(this.userWithoutClaims);
            this.resourceOwnerPasswordValidationContext.UserName = "username";
            this.resourceOwnerPasswordValidationContext.Password = null;

            await this.passwordValidator.ValidateAsync(this.resourceOwnerPasswordValidationContext);

            Assert.AreEqual(this.grantValidationResultWhenExceptionIsThrown.Error, resourceOwnerPasswordValidationContext.Result.Error);
            Assert.AreEqual(this.grantValidationResultWhenExceptionIsThrown.ErrorDescription, resourceOwnerPasswordValidationContext.Result.ErrorDescription);
        }
    }
}