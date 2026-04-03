using Moq;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class ExpenseCategoryServiceTests
    {
        private readonly Mock<IExpenseCategoryRepository> _repoMock;
        private readonly ExpenseCategoryService _service;

        public ExpenseCategoryServiceTests()
        {
            _repoMock = new Mock<IExpenseCategoryRepository>();
            _service = new ExpenseCategoryService(_repoMock.Object);
        }

        private static ExpenseCategory MakeCategory(Guid? id = null, string name = "Travel") =>
            new ExpenseCategory
            {
                ExpenseCategoryId = id ?? Guid.NewGuid(),
                CategoryName = name,
                CreatedAt = DateTime.UtcNow
            };

        // ─── GET ALL ──────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            // Arrange
            var categories = new List<ExpenseCategory>
            {
                MakeCategory(name: "Travel"),
                MakeCategory(name: "Food"),
                MakeCategory(name: "Accommodation")
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_EmptyList_ReturnsEmpty()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ExpenseCategory>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        // ─── GET BY ID ────────────────────────────────────────────────────────────

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsCategory()
        {
            // Arrange
            var cat = MakeCategory();
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);

            // Act
            var result = await _service.GetByIdAsync(cat.ExpenseCategoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cat.CategoryName, result!.CategoryName);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ExpenseCategory?)null);

            // Act
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        // ─── CREATE ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task CreateAsync_ValidName_ReturnsNewId()
        {
            // Arrange
            _repoMock.Setup(r => r.AddAsync(It.IsAny<ExpenseCategory>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.CreateAsync("Medical");

            // Assert
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task CreateAsync_SavesCorrectCategoryName()
        {
            // Arrange
            ExpenseCategory? saved = null;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<ExpenseCategory>()))
                .Callback<ExpenseCategory>(c => saved = c)
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync("Stationery");

            // Assert
            Assert.NotNull(saved);
            Assert.Equal("Stationery", saved!.CategoryName);
        }

        [Fact]
        public async Task CreateAsync_CallsSaveChanges()
        {
            // Arrange
            _repoMock.Setup(r => r.AddAsync(It.IsAny<ExpenseCategory>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.CreateAsync("Utilities");

            // Assert
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        // ─── UPDATE ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task UpdateAsync_ExistingCategory_UpdatesName()
        {
            // Arrange
            var cat = MakeCategory(name: "OldName");
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);
            _repoMock.Setup(r => r.UpdateAsync(cat)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAsync(cat.ExpenseCategoryId, "NewName");

            // Assert
            Assert.Equal("NewName", cat.CategoryName);
            _repoMock.Verify(r => r.UpdateAsync(cat), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ThrowsException()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ExpenseCategory?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateAsync(Guid.NewGuid(), "NewName"));
            Assert.Equal("Category not found", ex.Message);
        }

        // ─── DELETE ───────────────────────────────────────────────────────────────

        [Fact]
        public async Task DeleteAsync_ExistingCategory_ReturnsTrue()
        {
            // Arrange
            var cat = MakeCategory();
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);
            _repoMock.Setup(r => r.DeleteAsync(cat)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAsync(cat.ExpenseCategoryId);

            // Assert
            Assert.True(result);
            _repoMock.Verify(r => r.DeleteAsync(cat), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ExpenseCategory?)null);

            // Act
            var result = await _service.DeleteAsync(Guid.NewGuid());

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_CallsSaveChanges()
        {
            // Arrange
            var cat = MakeCategory();
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);
            _repoMock.Setup(r => r.DeleteAsync(cat)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _service.DeleteAsync(cat.ExpenseCategoryId);

            // Assert
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
