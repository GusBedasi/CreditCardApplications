using System;

namespace CreditCardApplications
{

    public interface ILicenseData
    {
        string LicenseKey { get; }
    }

    public interface IServiceInformation
    {
        ILicenseData License { get; }
    }
    public interface IFrequentFlyerNumberValidator
    {
        bool IsValid(string frequentFlyerNumber);
        void IsValid(string frequentFlyerNumber, out bool isValid);
        IServiceInformation ServiceInformation { get; }
        ValidationNode ValidationNode { get; set; }
        event EventHandler ValidatorLookupPerformed;
    }
}