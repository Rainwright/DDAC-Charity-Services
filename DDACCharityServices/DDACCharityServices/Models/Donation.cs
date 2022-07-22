using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DDACCharityServices.Models {
	public class Donation {

		public int DonationID { get; set; }
		public int BackgroundID { get; set; }

		[Display(Name = "Account Requesting for Donation")]
		public string CustomerEmail { get; set; }

		[Display(Name = "Account Donating")]
		public string StaffEmail { get; set; }

		[Required(ErrorMessage = "Please specify an amount for this donation!")]
		[Range(1, Int32.MaxValue)]
		[Display(Name = "Donation Amount")]
		[DataType(DataType.Currency)]
		public int DonationAmount { get; set; }

		[DataType(DataType.Date)]
		[Display(Name = "Donation Date")]
		public DateTime DonationDate;

	}
}
