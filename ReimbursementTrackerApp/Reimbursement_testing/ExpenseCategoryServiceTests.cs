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

        

        [Fact]
        public async Task GetAllAsync_ReturnsAllCategories()
        {
            
            var categories = new List<ExpenseCategory>
            {
                MakeCategory(name: "Travel"),
                MakeCategory(name: "Food"),
                MakeCategory(name: "Accommodation")
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            
            var result = await _service.GetAllAsync();

            
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_EmptyList_ReturnsEmpty()
        {
            
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ExpenseCategory>());

            
            var result = await _service.GetAllAsync();

            
            Assert.Empty(result);
        }

        

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsCategory()
        {
            
            var cat = MakeCategory();
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);

            
            var result = await _service.GetByIdAsync(cat.ExpenseCategoryId);

            
            Assert.NotNull(result);
            Assert.Equal(cat.CategoryName, result!.CategoryName);
        }

        [Fact]
        public async Task GetByIdAsync_NotFound_ReturnsNull()
        {
            
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ExpenseCategory?)null);

            
            var result = await _service.GetByIdAsync(Guid.NewGuid());

            
            Assert.Null(result);
        }

        

        [Fact]
        public async Task CreateAsync_ValidName_ReturnsNewId()
        {
           
            _repoMock.Setup(r => r.AddAsync(It.IsAny<ExpenseCategory>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            var result = await _service.CreateAsync("Medical");

            
            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public async Task CreateAsync_SavesCorrectCategoryName()
        {
           
            ExpenseCategory? saved = null;
            _repoMock.Setup(r => r.AddAsync(It.IsAny<ExpenseCategory>()))
                .Callback<ExpenseCategory>(c => saved = c)
                .Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.CreateAsync("Stationery");

            
            Assert.NotNull(saved);
            Assert.Equal("Stationery", saved!.CategoryName);
        }

        [Fact]
        public async Task CreateAsync_CallsSaveChanges()
        {
            
            _repoMock.Setup(r => r.AddAsync(It.IsAny<ExpenseCategory>())).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.CreateAsync("Utilities");

            
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        

        [Fact]
        public async Task UpdateAsync_ExistingCategory_UpdatesName()
        {
            
            var cat = MakeCategory(name: "OldName");
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);
            _repoMock.Setup(r => r.UpdateAsync(cat)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.UpdateAsync(cat.ExpenseCategoryId, "NewName");

            
            Assert.Equal("NewName", cat.CategoryName);
            _repoMock.Verify(r => r.UpdateAsync(cat), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_NotFound_ThrowsException()
        {
            
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ExpenseCategory?)null);

           
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.UpdateAsync(Guid.NewGuid(), "NewName"));
            Assert.Equal("Category not found", ex.Message);
        }


        [Fact]
        public async Task DeleteAsync_ExistingCategory_ReturnsTrue()
        {
           
            var cat = MakeCategory();
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);
            _repoMock.Setup(r => r.DeleteAsync(cat)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            var result = await _service.DeleteAsync(cat.ExpenseCategoryId);

            
            Assert.True(result);
            _repoMock.Verify(r => r.DeleteAsync(cat), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_NotFound_ReturnsFalse()
        {
            
            _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((ExpenseCategory?)null);

            
            var result = await _service.DeleteAsync(Guid.NewGuid());

            
            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_CallsSaveChanges()
        {
           
            var cat = MakeCategory();
            _repoMock.Setup(r => r.GetByIdAsync(cat.ExpenseCategoryId)).ReturnsAsync(cat);
            _repoMock.Setup(r => r.DeleteAsync(cat)).Returns(Task.CompletedTask);
            _repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            
            await _service.DeleteAsync(cat.ExpenseCategoryId);

            
            _repoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
