using Microsoft.VisualStudio.TestTools.UnitTesting;
using Access.Auth.Service.Domain.UserManagement;
using Access.Auth.Service.Domain.Request;
using Access.Auth.Service.Business.UserManagement;
using Access.Auth.Service.Infra.Authentication;
using Access.Auth.Service.Infra.Store;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BCrypt.Net;

namespace Access.Auth.Service.Test
{
    [TestClass]
    public class UserManagementHandlerTest
    {     
        private IAuthenticationRepository authenticationRepository = Substitute.For<IAuthenticationRepository>();  
        private IUserStore mockedUserStore =  Substitute.For<IUserStore>();  
        
        private IUserManagementHandler handlerWithMockedUserStore;
        private IUserManagementHandler userManagementHandler;
        private IUserStore userStore;
        private User user;
        private PasswordChangeRequest passwordChangeRequest;
        private NicknameChangeRequest nicknameChangeRequest;
        private OldPortalPasswordChangeRequest oldPortalPasswordChangeRequest;
        private OldPortalNicknameChangeRequest oldPortalNicknameChangeRequest;

        [TestInitialize]
        public void InitialData() 
        {
            this.userStore = new UserStore(this.authenticationRepository);
            this.userManagementHandler = new UserManagementHandler(this.userStore);

            this.handlerWithMockedUserStore = new UserManagementHandler(this.mockedUserStore);
            
            this.user = new User {
                Id = "1",
                Nickname = "gustavo.matos",
                Username = "gu",
                Roles = new List<string> {
                    "Admin"
                },
                Email = "eae@cappta.com.br",
                Password = "123456"
            };

            this.passwordChangeRequest = new PasswordChangeRequest {
                OldPassword = "123456",
                NewPassword = "456789",
                NewPasswordConfirmation = "456789"
            };

            this.nicknameChangeRequest = new NicknameChangeRequest {
                Password = "123456",
                NewNickname = "jureg"
            };

            this.oldPortalPasswordChangeRequest = new OldPortalPasswordChangeRequest {
                Username = "gustavo.matos",
                NewPassword = "456789"
            };

            this.oldPortalNicknameChangeRequest = new OldPortalNicknameChangeRequest {
                Username = "gustavo.matos",
                NewNickname = "jureg"
            };
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_User_Id_in_ChangePasswordAsync_is_invalid_return_Exception()
        {
            await this.userManagementHandler.ChangePasswordAsync(this.passwordChangeRequest, null);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_User_Id_in_ChangePasswordAsync_is_not_found_return_Exception()
        {
            this.user.Id = "256";

            await this.userManagementHandler.AddAsync(this.user);

            await this.userManagementHandler.ChangePasswordAsync(this.passwordChangeRequest, this.user.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_New_Password_is_different_from_the_Confirmation_return_Exception()
        {
            this.passwordChangeRequest.NewPasswordConfirmation = "789456";
            
            await this.userManagementHandler.ChangePasswordAsync(this.passwordChangeRequest, this.user.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_UserPassword_in_ChangePasswordAsync_after_being_hashed_is_different_from_old_password_return_Exception()
        {
            this.user.Password = "741258";

            await this.userManagementHandler.AddAsync(this.user);

            this.authenticationRepository.Single(Arg.Any<Expression<Func<User, bool>>>()).Returns(this.user);

            await this.userManagementHandler.ChangePasswordAsync(this.passwordChangeRequest, this.user.Id);
        }

        [TestMethod]
        public async Task When_the_request_is_valid_change_the_User_Password()
        {
            await this.userManagementHandler.AddAsync(this.user);

            this.authenticationRepository.Single(Arg.Any<Expression<Func<User, bool>>>()).Returns(this.user);

            await this.userManagementHandler.ChangePasswordAsync(this.passwordChangeRequest, this.user.Id);
       
            var resultUser = this.authenticationRepository.Single<User>(u => u.Id == user.Id);

            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(this.passwordChangeRequest.NewPassword, resultUser.Password));
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_User_Id_in_ChangeNicknameAsync_is_invalid_return_Exception()
        {
            await this.userManagementHandler.ChangeNicknameAsync(this.nicknameChangeRequest, null);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_User_Id_in_ChangeNicknameAsync_is_not_found_return_Exception()
        {
            this.user.Id = "256";

            await this.userManagementHandler.AddAsync(this.user);

            await this.userManagementHandler.ChangeNicknameAsync(this.nicknameChangeRequest, this.user.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_the_requested_Nickname_already_exists_return_Exception()
        {
            await this.userManagementHandler.AddAsync(this.user);

            this.authenticationRepository.Single(Arg.Any<Expression<Func<User, bool>>>()).Returns(new User());
            
            await this.userManagementHandler.ChangeNicknameAsync(this.nicknameChangeRequest, this.user.Id);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_UserPassword_in_ChangeNicknameAsync_after_being_hashed_is_different_from_old_password_return_Exception()
        {
            this.user.Password = "741258";

            await this.handlerWithMockedUserStore.AddAsync(this.user);

            this.mockedUserStore.FindUserByUsernameOrNicknameAsync(Arg.Any<string>()).Returns<User>(e => null);
            this.mockedUserStore.FindUserByIdAsync(Arg.Any<string>()).Returns(this.user);

            await this.handlerWithMockedUserStore.ChangeNicknameAsync(this.nicknameChangeRequest, this.user.Id);
        }

        [TestMethod]
        public async Task When_the_request_is_valid_change_the_User_Nickname()
        {
            await this.handlerWithMockedUserStore.AddAsync(this.user);

            this.mockedUserStore.FindUserByUsernameOrNicknameAsync(Arg.Any<string>()).Returns<User>(e => null);
            this.mockedUserStore.FindUserByIdAsync(Arg.Any<string>()).Returns(this.user);

            await handlerWithMockedUserStore.ChangeNicknameAsync(this.nicknameChangeRequest, this.user.Id);

            var resultUser = this.mockedUserStore.FindUserByIdAsync(this.user.Id).Result;

            Assert.AreEqual(this.nicknameChangeRequest.NewNickname, resultUser.Nickname);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_User_in_OldPortalChangePasswordAsync_is_null_return_Exception()
        {
            await this.handlerWithMockedUserStore.AddAsync(this.user);

            this.mockedUserStore.FindUserByUsernameAsync(Arg.Any<string>()).Returns<User>(e => null);

            await this.handlerWithMockedUserStore.OldPortalChangePasswordAsync(this.oldPortalPasswordChangeRequest);
        }

        [TestMethod]
        public async Task When_the_request_is_valid_change_the_User_Password_in_the_Old_Portal()
        {
            await this.userManagementHandler.AddAsync(this.user);

            this.authenticationRepository.Single(Arg.Any<Expression<Func<User, bool>>>()).Returns(this.user);

            await this.userManagementHandler.OldPortalChangePasswordAsync(this.oldPortalPasswordChangeRequest);

            var resultUser = this.authenticationRepository.Single<User>(u => u.Id == user.Id);

            Assert.IsTrue(BCrypt.Net.BCrypt.Verify(this.oldPortalPasswordChangeRequest.NewPassword, resultUser.Password));
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_ByNickname_in_OldPortalChangeNicknameAsync_is_not_null_return_Exception()
        {
            await this.handlerWithMockedUserStore.AddAsync(this.user);

            this.mockedUserStore.FindUserByNicknameAsync(Arg.Any<string>()).Returns(this.user);

            await this.handlerWithMockedUserStore.OldPortalChangeNicknameAsync(this.oldPortalNicknameChangeRequest);
        }

        [TestMethod]
        [ExpectedException(typeof(UserManagementException))]
        public async Task When_User_in_OldPortalChangeNicknameAsync_is_null_return_Exception()
        {
            await this.handlerWithMockedUserStore.AddAsync(this.user);

            this.mockedUserStore.FindUserByNicknameAsync(Arg.Any<string>()).Returns<User>(e => null);
            this.mockedUserStore.FindUserByUsernameAsync(Arg.Any<string>()).Returns<User>(e => null);

            await this.handlerWithMockedUserStore.OldPortalChangeNicknameAsync(this.oldPortalNicknameChangeRequest);
        }

        [TestMethod]
        public async Task When_the_request_is_valid_change_the_User_Nickname_in_the_Old_Portal()
        {
            await this.handlerWithMockedUserStore.AddAsync(this.user);

            this.mockedUserStore.FindUserByNicknameAsync(Arg.Any<string>()).Returns<User>(e => null);
            this.mockedUserStore.FindUserByUsernameAsync(Arg.Any<string>()).Returns(this.user);
            this.mockedUserStore.FindUserByIdAsync(Arg.Any<string>()).Returns(this.user);

            await this.handlerWithMockedUserStore.OldPortalChangeNicknameAsync(this.oldPortalNicknameChangeRequest);

            var resultUser = this.mockedUserStore.FindUserByIdAsync(this.user.Id).Result;

            Assert.AreEqual(this.oldPortalNicknameChangeRequest.NewNickname, resultUser.Nickname);
        }
    }
}