using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using GestionDeTareas.Controllers;
using GestionDeTareas.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;

namespace Miproyecto.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<IJwtService> _jwtServiceMock = new();
        private readonly Mock<ILogger<AuthController>> _loggerMock = new();

        private TaskContext GetDbContextWithUser(bool includeUser = true)
        {
            var options = new DbContextOptionsBuilder<TaskContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var context = new TaskContext(options);

            if (includeUser)
            {
                context.Usuarios.Add(new Usuario
                {
                    NombreUsuario = "admin",
                    Email = "admin@demo.com",
                    PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("1234")),
                    Activo = true,
                    Rol = "Admin"
                });
                context.SaveChanges();
            }

            return context;
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsOk()
        {
            var context = GetDbContextWithUser();
            _jwtServiceMock.Setup(j => j.GenerarToken(It.IsAny<Usuario>())).Returns("fake-jwt");

            var controller = new AuthController(context, _jwtServiceMock.Object, _loggerMock.Object);
            var result = await controller.Login(new UserLoginRequest { NombreUsuario = "admin", Password = "1234" });

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var context = GetDbContextWithUser();
            var controller = new AuthController(context, _jwtServiceMock.Object, _loggerMock.Object);
            var result = await controller.Login(new UserLoginRequest { NombreUsuario = "admin", Password = "wrong" });

            Assert.IsType<UnauthorizedObjectResult>(result.Result);
        }

        [Fact]
        public async Task Register_ExistingUser_ReturnsConflict()
        {
            var context = GetDbContextWithUser();
            var controller = new AuthController(context, _jwtServiceMock.Object, _loggerMock.Object);

            var result = await controller.Register(new UserRegisterRequest
            {
                NombreUsuario = "admin",
                Email = "admin@demo.com",
                Password = "1234"
            });

            Assert.IsType<ConflictObjectResult>(result.Result);
        }
    }
}