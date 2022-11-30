using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDACCharityServices.Data;
using DDACCharityServices.Models;

namespace DDACCharityServices.Controllers {
	public class DonationsController : Controller {
		private readonly DDACCharityServicesForBackgroundContext _context;

		public DonationsController(DDACCharityServicesForBackgroundContext context) {
			_context = context;
		}

		// GET: Donations
		public async Task<IActionResult> Index() {
			var donations = await _context.Donation.ToListAsync();
			foreach (Donation donation in donations) {
				donation.Background = await _context.Background.FindAsync(donation.BackgroundID);
			}
			return View(donations);
		}

		// GET: Donations/Details/5
		public async Task<IActionResult> Details(int? id) {
			if (id == null) return NotFound();

			var donation = await _context.Donation.FirstOrDefaultAsync(m => m.DonationID == id);
			if (donation == null) return NotFound();
			
			donation.Background = await _context.Background.FirstOrDefaultAsync(m => m.BackgroundID == donation.BackgroundID);
			return View(donation);
		}
	}
}
