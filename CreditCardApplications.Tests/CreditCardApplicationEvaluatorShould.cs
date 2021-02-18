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
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true).Verifiable();

            CreditCardApplicationDecision result = sut.Evaluate(mockApplication);

            //_mockFrequentFlyerValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once());
            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, result);
        }
    }
}
