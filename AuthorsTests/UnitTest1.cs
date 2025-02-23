using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Controllers;
using WebApplication2.Data;
using WebApplication2.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace AuthorsTests
{
    [TestFixture]
    public class AuthorsControllerTests
    {
        private Mock<ApplicationDbContext> _mockContext;
        private Mock<DbSet<Author>> _mockSet;
        private AuthorsController _controller;

       
            [SetUp]
            public void Setup()
            {

            var authors = new List<Author>
{
            new Author { Id = 1, FullName = "Author One", Deceased = false, ProfilePicture = "/uploads/author1.png", Biography = "Biography One", Books = new List<Book>() },
             new Author { Id = 2, FullName = "Author Two", Deceased = true, ProfilePicture = "/uploads/author2.png", Biography = "Biography Two", Books = new List<Book>() }
}            .AsQueryable();

            _mockSet = new Mock<DbSet<Author>>();

            _mockSet.As<IQueryable<Author>>().Setup(m => m.Provider).Returns(authors.Provider);
            _mockSet.As<IQueryable<Author>>().Setup(m => m.Expression).Returns(authors.Expression);
            _mockSet.As<IQueryable<Author>>().Setup(m => m.ElementType).Returns(authors.ElementType);
            _mockSet.As<IQueryable<Author>>().Setup(m => m.GetEnumerator()).Returns(authors.GetEnumerator());

            _mockSet.As<IAsyncEnumerable<Author>>()
        .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
        .Returns(new TestAsyncEnumerator<Author>(authors.GetEnumerator()));

            _mockContext = new Mock<ApplicationDbContext>();


            _mockContext.Setup(c => c.Authors).Returns(_mockSet.Object);


            _controller = new AuthorsController(_mockContext.Object);
        }

            [TearDown]
            public void TearDown()
            {
                _controller.Dispose();
            }

            [Test]
            public async Task Index_ReturnsViewWithAuthors()
            {
                var result = await _controller.Index() as ViewResult;

                Assert.IsNotNull(result);
                Assert.IsInstanceOf<IEnumerable<Author>>(result.Model);
            }

        [Test]
        public async Task Details_WithValidId_ReturnsViewWithAuthor()
        {
            // Arrange: Create a sample author
            var author = new Author
            {
                Id = 1,
                FullName = "Author One",
                Deceased = false,
                ProfilePicture = "/uploads/author1.png",
                Biography = "Biography One",
                Books = new List<Book>()
            };

            // Mock FindAsync to return the author when ID = 1
            _mockSet.Setup(m => m.FindAsync(1))
                    .ReturnsAsync(author);

            _mockContext.Setup(c => c.Authors).Returns(_mockSet.Object);

            // Act: Call the Details action
            var result = await _controller.Details(1) as ViewResult;

            // Assert: Check the result
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Model);
            Assert.IsInstanceOf<Author>(result.Model);
            Assert.AreEqual(author.Id, ((Author)result.Model).Id);
        }


        [Test]
            public async Task Details_WithInvalidId_ReturnsNotFound()
            {
                var result = await _controller.Details(99);

                Assert.IsInstanceOf<NotFoundResult>(result);
            }

        [Test]
        public async Task Create_WithValidModelAndImageFile_AddsAuthorAndRedirects()
        {
            var author = new Author { Id = 3, FullName = "New Author", Deceased = false, Biography = "New Bio", Books = new List<Book>() };

            var mockImageFile = new Mock<IFormFile>();
            var content = new MemoryStream(new byte[100]); // Simulating file content
            var fileName = "test.jpg";


            mockImageFile.Setup(f => f.Length).Returns(100);
            mockImageFile.Setup(f => f.FileName).Returns(fileName);
            mockImageFile.Setup(f => f.OpenReadStream()).Returns(content);
            mockImageFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns((Stream target, System.Threading.CancellationToken cancellationToken) => content.CopyToAsync(target, cancellationToken));

            var result = await _controller.Create(author, mockImageFile.Object) as RedirectToActionResult;

            _mockSet.Verify(m => m.Add(It.IsAny<Author>()), Times.Once);
            _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
            Assert.AreEqual("Index", result.ActionName);
        }

        [Test]
            public async Task Edit_WithValidId_UpdatesAuthorAndRedirects()
            {
                var existingAuthor = new Author { Id = 1, FullName = "Updated Author", Deceased = true, ProfilePicture = "/uploads/updated-author.png", Biography = "Updated Bio", Books = new List<Book>() };
                _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(existingAuthor);

                var result = await _controller.Edit(1, existingAuthor, null) as RedirectToActionResult;

                _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
                Assert.AreEqual("Index", result.ActionName);
            }

            [Test]
            public async Task DeleteConfirmed_WithValidId_RemovesAuthorAndRedirects()
            {
                var author = new Author { Id = 1, FullName = "Author One", Deceased = false, ProfilePicture = "/uploads/author1.png", Biography = "Biography One", Books = new List<Book>() };
                _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(author);

                var result = await _controller.DeleteConfirmed(1) as RedirectToActionResult;

                _mockSet.Verify(m => m.Remove(It.IsAny<Author>()), Times.Once);
                _mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
                Assert.AreEqual("Index", result.ActionName);
            }
        }

    }

