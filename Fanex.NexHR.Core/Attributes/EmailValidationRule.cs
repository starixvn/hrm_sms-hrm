namespace Fanex.NexHR.Core.Attributes
{
    using System.Web.Mvc;

    public class EmailValidationRule : ModelClientValidationRule
    {
        public EmailValidationRule(string errorMessage)
        {
            ErrorMessage = errorMessage;
            ValidationType = "email";
        }
    }
}
