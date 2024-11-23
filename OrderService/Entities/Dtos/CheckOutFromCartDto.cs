using Entities.Helpers;
using System.ComponentModel.DataAnnotations;

namespace Entities.Dtos
{
    public class CheckOutFromCartDto
    {
        [Required(ErrorMessage = "This field is required")]
        public Guid AddressId { get; set; }

        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^(Credit|Debit|UPI|COD)$", ErrorMessage = "This field can be either 'Credit' or 'Debit' or 'UPI' or 'COD' ")]
        public string PaymentMethod { get; set; } = string.Empty;

        [RequiredIf("PaymentMethod", "Credit", "Debit", "UPI")]
        public Guid? PaymentId { get; set; }
    }
}
