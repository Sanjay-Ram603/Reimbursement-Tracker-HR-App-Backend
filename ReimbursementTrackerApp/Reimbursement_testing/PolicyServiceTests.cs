using Moq;
using ReimbursementTrackerApp.Models.Enumerations;
using ReimbursementTrackerApp.Models.Policy;
using ReimbursementTrackerApp.Models.Reimbursement;
using ReimbursementTrackerApp.Repositories.Interfaces;
using ReimbursementTrackerApp.Services.Implementations;

namespace Reimbursement_testing
{
    public class PolicyServiceTests
    {
        private readonly Mock<IPolicyRepository> _policyRepoMock;
        private readonly PolicyService _service;

        public PolicyServiceTests()
        {
            _policyRepoMock = new Mock<IPolicyRepository>();
            _service = new PolicyService(_policyRepoMock.Object);
        }

        private static ReimbursementRequest MakeRequest(decimal amount) =>
            new ReimbursementRequest
            {
                ReimbursementRequestId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                ExpenseCategoryId = Guid.NewGuid(),
                Amount = amount,
                Description = "Test",
                Status = ReimbursementStatusType.Submitted,
                AttachmentPath = null,
                ExpenseDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

        private static PolicyRule MakeRule(decimal? maxAmount, bool isActive = true) =>
            new PolicyRule
            {
                PolicyRuleId = Guid.NewGuid(),
                RuleName = "Max Amount Rule",
                RuleType = PolicyRuleType.MaximumAmountLimit,
                MaximumAmount = maxAmount,
                IsActive = isActive
            };

        // ─── VALIDATE POLICIES ────────────────────────────────────────────────────

        [Fact]
        public async Task ValidatePoliciesAsync_AmountWithinLimit_DoesNotThrow()
        {
            // Arrange
            var rules = new List<PolicyRule> { MakeRule(10000m) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(5000m);

            // Act & Assert — no exception
            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_AmountExceedsLimit_ThrowsException()
        {
            // Arrange
            var rules = new List<PolicyRule> { MakeRule(10000m) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(15000m);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.ValidatePoliciesAsync(request));
            Assert.Contains("Policy violation", ex.Message);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_AmountEqualsLimit_DoesNotThrow()
        {
            // Arrange
            var rules = new List<PolicyRule> { MakeRule(10000m) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(10000m);

            // Act & Assert — exactly at limit, should not throw
            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_NoRules_DoesNotThrow()
        {
            // Arrange
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync())
                .ReturnsAsync(new List<PolicyRule>());

            var request = MakeRequest(999999m);

            // Act & Assert — no rules means no violations
            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_RuleWithNullMaxAmount_DoesNotThrow()
        {
            // Arrange
            var rules = new List<PolicyRule> { MakeRule(null) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(999999m);

            // Act & Assert — null max means no limit check
            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_MultipleRules_ThrowsOnFirstViolation()
        {
            // Arrange
            var rules = new List<PolicyRule>
            {
                MakeRule(50000m),  // passes
                MakeRule(5000m)    // fails
            };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(10000m);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.ValidatePoliciesAsync(request));
            Assert.Contains("Policy violation", ex.Message);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_CallsGetAllActiveRules()
        {
            // Arrange
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync())
                .ReturnsAsync(new List<PolicyRule>());

            // Act
            await _service.ValidatePoliciesAsync(MakeRequest(100m));

            // Assert
            _policyRepoMock.Verify(r => r.GetAllActiveRulesAsync(), Times.Once);
        }
    }
}
