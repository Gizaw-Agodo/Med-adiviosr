
using Microsoft.Extensions.Configuration;
using MedAdvisor.Api.Controllers;
using System.Web.Http.Results;
using MedAdvisor.Api.Dtos;
using FluentAssertions;
using FakeItEasy;
using Xunit;

namespace Test.MedAdvisor.Api.test.ControllersTest
{
    public class AuthControllerTest
    {
        private readonly IConfiguration _config;

        public AuthControllerTest()
        {
            _config = A.Fake<IConfiguration>();

        }


           [Fact]
            public void Signup_ReturnOK()
            {
                //arrange
                var authController = new AuthController(_config);
                var userDto = A.Fake<UserRegistrationDto>();
                
                // act 
                var result = authController.Register(userDto);

                //Assert
                result.Should().NotBeNull();

                result.Should().BeOfType(typeof(OkResult));

            }

        [Fact]
        public void Signup_ReturnOK()
        {
            //arrange
            var authController = new AuthController(_config);
            var userDto = A.Fake<UserRegistrationDto>();

            // act 
            var result = authController.Register(userDto);

            //Assert
            result.Should().NotBeNull();

            result.Should().BeOfType(typeof(OkResult));

        }



        [Fact]
        public void Login_ReturnOK()
        {
            //Arrange
            var authController = new AuthController(_config);
            var userDto = A.Fake<UserLoginDto>();

            //Act
            var result = authController.login(userDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));

        }


        [Fact]
        public void LoginWithWrongUsername_ReturnBadRequest()
        {
            //Arrange
            var authController = new AuthController(_config);
            var userDto = A.Fake<ExternalLoginDto>();

            //Act
            userDto.Username = "wrongUsername";
            var result = authController.login(userDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));

        }


        [Fact]
        public void LoginWithWrongPassword_ReturnBadRequest()
        {
            //Arrange
            var authController = new AuthController(_config);
            var userDto = A.Fake<UserLoginDto>();

            //Act
            userDto.Password = "wrongPassword";
            var result = authController.login(userDto);

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(BadRequestObjectResult));

        }

    }
}
