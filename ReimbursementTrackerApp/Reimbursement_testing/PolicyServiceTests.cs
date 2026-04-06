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


        [Fact]
        public async Task ValidatePoliciesAsync_AmountWithinLimit_DoesNotThrow()
        {
         
            var rules = new List<PolicyRule> { MakeRule(10000m) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(5000m);

            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_AmountExceedsLimit_ThrowsException()
        {
    
            var rules = new List<PolicyRule> { MakeRule(10000m) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(15000m);

        
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.ValidatePoliciesAsync(request));
            Assert.Contains("Policy violation", ex.Message);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_AmountEqualsLimit_DoesNotThrow()
        {
   
            var rules = new List<PolicyRule> { MakeRule(10000m) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(10000m);


            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_NoRules_DoesNotThrow()
        {

            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync())
                .ReturnsAsync(new List<PolicyRule>());

            var request = MakeRequest(999999m);

        
            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_RuleWithNullMaxAmount_DoesNotThrow()
        {
      
            var rules = new List<PolicyRule> { MakeRule(null) };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(999999m);

          
            await _service.ValidatePoliciesAsync(request);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_MultipleRules_ThrowsOnFirstViolation()
        {
         
            var rules = new List<PolicyRule>
            {
                MakeRule(50000m),  
                MakeRule(5000m)    
            };
            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync()).ReturnsAsync(rules);

            var request = MakeRequest(10000m);

         
            var ex = await Assert.ThrowsAsync<Exception>(() =>
                _service.ValidatePoliciesAsync(request));
            Assert.Contains("Policy violation", ex.Message);
        }

        [Fact]
        public async Task ValidatePoliciesAsync_CallsGetAllActiveRules()
        {

            _policyRepoMock.Setup(r => r.GetAllActiveRulesAsync())
                .ReturnsAsync(new List<PolicyRule>());


            await _service.ValidatePoliciesAsync(MakeRequest(100m));

       
            _policyRepoMock.Verify(r => r.GetAllActiveRulesAsync(), Times.Once);
        }
    }
}
