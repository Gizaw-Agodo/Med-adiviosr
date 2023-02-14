
using MedAdvisor.DataAccess.MySql.Repositories.Users;
using MedAdvisor.Api.Controllers;
using MedAdvisor.Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using FakeItEasy;
using AutoFixture;
using Moq;
using MedAdvisor.Services.Okta.Interfaces;

namespace Test.MedAdvisor.Api.test.ControllersTest
{
    public class AuthControllerTest
    {
       
        private readonly Mock<IUserServices> _userServiceMock;
        private readonly Mock<IAuthService> _authServiceMock;
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly IFixture _fixture;

        public AuthControllerTest()
        {
            _fixture = new Fixture();
            _userServiceMock = _fixture.Freeze<Mock<IUserServices>>();
            _authServiceMock = _fixture.Freeze<Mock<IAuthService>>();
            _userRepoMock = _fixture.Freeze<Mock<IUserRepository>>();
            
        }


        [Fact]
        public void Signup_ReturnOK()
        {
            //arrange
            var authController = new AuthController(_userRepoMock.Object, _userServiceMock.Object, _authServiceMock.Object);

            //var authController = new AuthController(_config);
            var userDto = A.Fake<UserRegistrationDto>();

            // act 
            var result = authController.Register(userDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
        }

        //[Fact]
        //public void Signup_ReturnOK()
        //{
        //    //arrange
        //    var authController = new AuthController(_config);
        //    var userDto = A.Fake<UserRegistrationDto>();

        //    // act 
        //    var result = authController.Register(userDto);

        //    //Assert
        //    result.Should().NotBeNull();

        //    result.Should().BeOfType(typeof(OkResult));

        //}



        //[Fact]
        //public void Login_ReturnOK()
        //{
        //    //Arrange
        //    var authController = new AuthController(_config);
        //    var userDto = A.Fake<UserLoginDto>();

        //    //Act
        //    var result = authController.Login(userDto);

        //    //Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeOfType(typeof(OkObjectResult));

        //}


        //[Fact]
        //public void LoginWithWrongUsername_ReturnBadRequest()
        //{
        //    //Arrange
        //    var authController = new AuthController(_config);
        //    var userDto = A.Fake<ExternalLoginDto>();

        //    //Act
        //    userDto.Username = "wrongUsername";
        //    var result = authController.login(userDto);

        //    //Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeOfType(typeof(BadRequestObjectResult));

        //}


        //[Fact]
        //public void LoginWithWrongPassword_ReturnBadRequest()
        //{
        //    //Arrange
        //    var authController = new AuthController(_config);
        //    var userDto = A.Fake<UserLoginDto>();

        //    //Act
        //    userDto.Password = "wrongPassword";
        //    var result = authController.login(userDto);

        //    //Assert
        //    result.Should().NotBeNull();
        //    result.Should().BeOfType(typeof(BadRequestObjectResult));

        //}

    }
}
