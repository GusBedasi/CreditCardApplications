using System;
using Xunit;
using Moq;
using System.Collections.Generic;

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
            //Setup to specifie a mock property to return a specific value, in this case a literal string or it can be done with a function as well
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK").Verifiable();
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication() { GrossAnnualIncome = 100_000 };

            //Act
            CreditCardApplicationDecision result = sut.Evaluate(mockApplication);

            //Assert
            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.AutoAccepted, result);
        }

        [Fact]
        public void ReferYoungApplications()
        {
            //When a setup is settled the DefaultValue of the properties is mock
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK").Verifiable();
            //_mockFrequentFlyerValidator.DefaultValue = DefaultValue.Mock;
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true).Verifiable();
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication() { Age = 19 };

            CreditCardApplicationDecision result = sut.Evaluate(mockApplication);

            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, result);
        }

        [Fact]
        public void DeclineLowIncomeApplications()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK").Verifiable();
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true).Verifiable();
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

            CreditCardApplicationDecision result = sut.Evaluate(mockApplication);

            //_mockFrequentFlyerValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once());
            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, result);
        }

        [Fact]
        public void ReferInvalidFrequentFlyerApplications()
        {
            //Arrange
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK").Verifiable();
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(false).Verifiable();
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication();

            //Act
            CreditCardApplicationDecision mockDecision = sut.Evaluate(mockApplication);

            //Arrange
            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, mockDecision);
        }

        //[Fact]
        // public void DeclineLowIncomeApplicationsOutDemo()
        //{
        //    CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
        //    bool isValid = true;
        //    CreditCardApplication mockApplication = new CreditCardApplication()
        //    {
        //        GrossAnnualIncome = 19_999,
        //        Age = 42
        //    };
        //    _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>(), out isValid)).Verifiable();

        //    CreditCardApplicationDecision mockDecision = sut.EvaluateUsingOut(mockApplication);

        //    Mock.VerifyAll();
        //    Assert.Equal(CreditCardApplicationDecision.AutoDeclined, mockDecision);
        //}

        [Fact]
        public void ReferWhenLicenseKeyExpired() 
        {
            //var mockLicenseData = new Mock<ILicenseData>();
            //mockLicenseData.Setup(x => x.LicenseKey).Returns("EXPIRED").Verifiable();
            //var mockServiceInformation = new Mock<IServiceInformation>();
            //mockServiceInformation.Setup(x => x.License).Returns(mockLicenseData.Object).Verifiable(); ;
            //var mockValidator = new Mock<IFrequentFlyerNumberValidator>();
            //mockValidator.Setup(x => x.ServiceInformation).Returns(mockServiceInformation.Object).Verifiable(); ;
            //_mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Returns(true).Verifiable();

            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("EXPIRED").Verifiable();
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(mockApplication);

            Mock.VerifyAll();
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);
        }

        [Fact]
        public void UseDetailedLookupForOlderApplications()
        {
            // Keep enable track of changing to mock properties (SetupAllProperties and SetupProperty)
            //_mockFrequentFlyerValidator.SetupAllProperties();
            _mockFrequentFlyerValidator.SetupProperty(x => x.ValidationNode);
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK").Verifiable();
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication mockApplication = new CreditCardApplication { Age = 30 };

            sut.Evaluate(mockApplication);

            Mock.VerifyAll();
            Assert.Equal(ValidationNode.Detailed, _mockFrequentFlyerValidator.Object.ValidationNode);
        }

        //Testes abaixo são Behavior based testing
        [Fact]
        public void ValidateFrequentFlyerNumberForLowIncomeApplications()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("Ok");
            
            CreditCardApplicationEvaluator sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            CreditCardApplication application = new CreditCardApplication { FrequentFlyerNumber = "q" };

            sut.Evaluate(application);

            _mockFrequentFlyerValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void NotValidateFrequentFlyerNumberForHighIncomeApplications()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            var application = new CreditCardApplication { GrossAnnualIncome = 100_000 };

            sut.Evaluate(application);

            _mockFrequentFlyerValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CheckLicenseKeyToLowIncomeApplications()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            var application = new CreditCardApplication { GrossAnnualIncome = 99_000 };

            sut.Evaluate(application);

            _mockFrequentFlyerValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
        }

        [Fact]
        public void SetDetailedLookupForOlderApplications()
        {
            _mockFrequentFlyerValidator.SetupProperty(x => x.ValidationNode);
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            var sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            var application = new CreditCardApplication { Age = 30 };

            sut.Evaluate(application);

            _mockFrequentFlyerValidator.VerifySet(x => x.ValidationNode = It.IsAny<ValidationNode>(), Times.Once);
            //_mockFrequentFlyerValidator.VerifyGet(x => x.ServiceInformation.License.LicenseKey, Times.Once);
            //_mockFrequentFlyerValidator.Verify(x => x.IsValid(null), Times.Once);
           // _mockFrequentFlyerValidator.VerifyNoOtherCalls();
        }

        [Fact]
        public void ReferWhenFrequentFlyerValidatonError()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            //_mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws(new Exception("Custome message")); 
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>())).Throws<Exception>();

            var sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            var application = new CreditCardApplication { Age = 42 };

            CreditCardApplicationDecision decision = sut.Evaluate(application);

            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, decision);

        }

        [Fact]
        public void IncrementLookupCount()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(It.IsAny<string>()))
                .Returns(true)
                .Raises(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);
            var sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            var application = new CreditCardApplication { FrequentFlyerNumber = "x", Age = 25 };

            sut.Evaluate(application);

            //_mockFrequentFlyerValidator.Raise(x => x.ValidatorLookupPerformed += null, EventArgs.Empty);

            Assert.Equal(1, sut.ValidatorLookupCount);
        }

        [Fact]
        public void ReferFrequentFlyerInvalidApplications_ReturnValuesSequences()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");
            _mockFrequentFlyerValidator.SetupSequence(x => x.IsValid(It.IsAny<string>()))
                .Returns(false)
                .Returns(true);

            var sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            var application = new CreditCardApplication { Age = 25 };

            CreditCardApplicationDecision firstDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.ReferredToHuman, firstDecision);
            CreditCardApplicationDecision secondDecision = sut.Evaluate(application);
            Assert.Equal(CreditCardApplicationDecision.AutoDeclined, secondDecision);

            _mockFrequentFlyerValidator.Verify(x => x.IsValid(It.IsAny<string>()), Times.Exactly(2));
        }

        [Fact]
        public void ReferFrequentFlyerInvalidApplications_MultipleCallsSequence()
        {
            _mockFrequentFlyerValidator.Setup(x => x.ServiceInformation.License.LicenseKey).Returns("OK");

            var frequentFlyerNumbersPassed = new List<string>();
            _mockFrequentFlyerValidator.Setup(x => x.IsValid(Capture.In(frequentFlyerNumbersPassed)));

            var sut = new CreditCardApplicationEvaluator(_mockFrequentFlyerValidator.Object);
            var application1 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "aa" };
            var application2 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "bb" };
            var application3 = new CreditCardApplication { Age = 25, FrequentFlyerNumber = "cc" };

            sut.Evaluate(application1);
            sut.Evaluate(application2);
            sut.Evaluate(application3);

            //Assert that IsValid was called three times  with "aa", "bb", "cc"
            Assert.Equal(new List<string> { "aa", "bb", "cc" }, frequentFlyerNumbersPassed);
        }
    }
}
