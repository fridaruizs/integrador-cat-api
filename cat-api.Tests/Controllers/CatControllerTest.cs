using integrador_cat_api.Controllers;
using integrador_cat_api.Models;
using integrador_cat_api.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace cat_api.Tests.Controllers
{
    public class CatControllerTest
    {
        private readonly CatController _catController;
        private readonly Mock<ICatService> _catServiceMock;

        public CatControllerTest()
        {
            _catServiceMock = new Mock<ICatService>();
            _catController = new CatController(_catServiceMock.Object);
        }

        [Fact]
        public void GetAll_ReturnsOkWithAllCats()
        {
            var cats = new List<Cat>
        {
            new Cat { Id = 1, Name = "Whiskers" },
            new Cat { Id = 2, Name = "Tom" }
        };
            _catServiceMock.Setup(service => service.GetAll()).Returns(cats);

            var result = _catController.GetAll();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.IsAssignableFrom<IEnumerable<Cat>>(okResult.Value);
            Assert.Equal(cats, okResult.Value);
        }

        [Fact]
        public void GetById_ExistingCatId_ReturnsOkWithCat()
        {
            var cat = new Cat { Id = 1, Name = "Whiskers" };
            _catServiceMock.Setup(service => service.GetById(1)).Returns(cat);

            var result = _catController.GetById(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCat = Assert.IsType<Cat>(okResult.Value);
            Assert.Equal(cat, returnedCat);
        }

        [Fact]
        public void GetById_NonExistingCatId_ReturnsNotFound()
        {
            _catServiceMock.Setup(service => service.GetById(2)).Returns((Cat)null);

            var result = _catController.GetById(2);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void Create_ValidCatName_ReturnsCreatedCat()
        {
            var newCat = new Cat { Id = 1, Name = "Whiskers" };
            _catServiceMock.Setup(service => service.Add("Whiskers")).Returns(newCat);

            var result = _catController.Create("Whiskers");

            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            Assert.Equal(newCat, createdResult.Value);
        }

        [Fact]
        public void Create_EmptyCatName_ReturnsBadRequest()
        {
            var result = _catController.Create("");

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Cat name cannot be empty.", badRequestResult.Value);
        }

    }
}
