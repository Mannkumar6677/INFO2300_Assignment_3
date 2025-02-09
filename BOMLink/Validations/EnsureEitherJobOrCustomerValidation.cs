//using System.ComponentModel.DataAnnotations;
//using BOMLink.Models;

//namespace BOMLink.Validations {
//    public class EnsureEitherJobOrCustomerValidation : ValidationAttribute {
//        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
//            var jobIdProperty = validationContext.ObjectType.GetProperty("JobId");
//            var customerIdProperty = validationContext.ObjectType.GetProperty("CustomerId");

//            if (jobIdProperty == null || customerIdProperty == null) {
//                return new ValidationResult("JobId or CustomerId property is missing.");
//            }

//            var jobIdValue = (int?)jobIdProperty.GetValue(validationContext.ObjectInstance);
//            var customerIdValue = (int?)customerIdProperty.GetValue(validationContext.ObjectInstance);

//            // Ensure that either Job or Customer is selected, but not both
//            if (jobIdValue == null && customerIdValue == null) {
//                return new ValidationResult("You must select either a Job or a Customer.");
//            }

//            return ValidationResult.Success;
//        }
//    }
//}
