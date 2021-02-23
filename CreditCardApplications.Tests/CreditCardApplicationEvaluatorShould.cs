using System;
using Xunit;
using Moq;

namespace CreditCardApplications.Tests
{
    public class CreditCardApplicationEvaluatorShould
    {
        public readonly Mock<IFrequentFlyerNumberValidator> _mockFrequentFlyerValidator;

        public CreditCardApplicationEvaluatorShould()
        {
            _mockFrequentFlyerValidator = new Mock<IFrequentFlyerNumberValidator>();
        }

        [Fact]
        public void AcceptHighIncomeApplications()
        {
            //Arrange
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication() { GrossAnnualIncome = 100_000 };

            //Act
            CreditCardApplicationDecision result = sut.Evaluate(mockApplication);

            //Assert
            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, result);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true);
            CreditCardApplication mockApplication = new CreditCardApplication() { Age = 19 };

            CreditCardApplicationDecision result = sut.Evaluate(mockApplication);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, result);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication() 
            { 
                GrossAnnualIncome = 19_999,
                Age = 42,
                FrequentFlyerNumber = "abc" 
            };
            //_mockFrequentFlyerValidator.Setup(x => x.IsValid("x")).Returns(true);
            //_mockFrequentFlyerValidator.Setup(x => x.IsValid(It.Is<string>(number => number.StartsWith("a")))).Returns(true);
            //_mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsInRange<string>("a", "z", Moq.Range.Inclusive))).Returns(true);
            //_mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsIn<string>("a", "b", "c"))).Returns(true);
            //_mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsRegex("[a-z]"))).Returns(true);
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true).Verifiable();

            CreditCardApplicationDecision result = sut.Evaluate(mockApplication);

            //_mockFrequentFlyerValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once());
            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, result);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            //Arrange
            Mock<IFrequentFlyerNumberValidator> mockValidator = new Mock<IFrequentFlyerNumberValidator>(MockBehavior.Strict);
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(mockValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication();
            mockValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false).Verifiable();

            //Act
            CreditCardApplicationDecision mockDecision = sut.Evaluate(mockApplication);

            //Arrange
            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, mockDecision);
        }

        [Fact]
        public void DeclineLowIncomeApplicationsOutDemo()
        {
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            bool isValid = true;
            CreditCardApplication mockApplication = new CreditCardApplication()
            {
                GrossAnnualIncome = 19_999,
                Age = 42
            };
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid)).Verifiable();

            CreditCardApplicationDecision mockDecision = sut.EvaluateUsingOut(mockApplication);

            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, mockDecision);
        }
    }
}
