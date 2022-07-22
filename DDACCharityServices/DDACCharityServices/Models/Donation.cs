using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DDACCharityServices.Models {
	public class Donation {

		public int DonationID { get; set; }

		[ForeignKey("Background")]
		public int BackgroundID;
		public virtual Background Background { get; set; }

		[Display(Name = "Account Donating")]
		public string CustomerEmail { get; set; }

		[Required(ErrorMessage = "Please specify an amount for this donation!")]
		[Range(1, Int32.MaxValue)]
		[Display(Name = "Donation Amount")]
		[DataType(DataType.Currency)]
		public int DonationAmount { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Donation Date")]
		public DateTime DonationDate { get; set; }

	}
}
