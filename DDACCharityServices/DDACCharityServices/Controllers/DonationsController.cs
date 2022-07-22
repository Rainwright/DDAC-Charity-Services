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
			return View(await _context.Donation.ToListAsync());
		}

		// GET: Donations/Details/5
		public async Task<IActionResult> Details(int? id) {
			if (id == null) {
				return NotFound();
			}

			var donation = await _context.Donation
				.FirstOrDefaultAsync(m => m.DonationID == id);
			if (donation == null) {
				return NotFound();
			}

			return View(donation);
		}

		// GET: Donations/Create
		public IActionResult Create() {
			return View();
		}

		// POST: Donations/Create
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("ID,CustomerEmail,StaffEmail,BackgroundID,DonationAmount")] Donation donation) {
			if (ModelState.IsValid) {
				_context.Add(donation);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			return View(donation);
		}

		// GET: Donations/Edit/5
		public async Task<IActionResult> Edit(int? id) {
			if (id == null) {
				return NotFound();
			}

			var donation = await _context.Donation.FindAsync(id);
			if (donation == null) {
				return NotFound();
			}
			return View(donation);
		}

		// POST: Donations/Edit/5
		// To protect from overposting attacks, enable the specific properties you want to bind to, for 
		// more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("ID,CustomerEmail,StaffEmail,BackgroundID,DonationAmount")] Donation donation) {
			if (id != donation.DonationID) {
				return NotFound();
			}

			if (ModelState.IsValid) {
				try {
					_context.Update(donation);
					await _context.SaveChangesAsync();
				} catch (DbUpdateConcurrencyException) {
					if (!DonationExists(donation.DonationID)) {
						return NotFound();
					} else {
						throw;
					}
				}
				return RedirectToAction(nameof(Index));
			}
			return View(donation);
		}

		// GET: Donations/Delete/5
		public async Task<IActionResult> Delete(int? id) {
			if (id == null) {
				return NotFound();
			}

			var donation = await _context.Donation
				.FirstOrDefaultAsync(m => m.DonationID == id);
			if (donation == null) {
				return NotFound();
			}

			return View(donation);
		}

		// POST: Donations/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id) {
			var donation = await _context.Donation.FindAsync(id);
			_context.Donation.Remove(donation);
			await _context.SaveChangesAsync();
			return RedirectToAction(nameof(Index));
		}

		private bool DonationExists(int id) {
			return _context.Donation.Any(e => e.DonationID == id);
		}
	}
}
