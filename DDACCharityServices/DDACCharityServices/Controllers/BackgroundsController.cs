using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DDACCharityServices.Data;
using DDACCharityServices.Models;
using DDACCharityServices.Controllers.Enums;

namespace DDACCharityServices.Views.Backgrounds
{
    public class BackgroundsController : Controller
    {
        private readonly DDACCharityServicesForBackgroundContext _context;

        public BackgroundsController(DDACCharityServicesForBackgroundContext context)
        {
            _context = context;
        }

        // GET: Backgrounds
        public async Task<IActionResult> Index(string searchstring, string searchkeyword)
        { 
            return View(await _context.Background.ToListAsync());
        }

        // GET: Backgrounds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var background = await _context.Background
                .FirstOrDefaultAsync(m => m.BackgroundID == id);
            if (background == null)
            {
                return NotFound();
            }

            return View(background);
        }

        // GET: Backgrounds/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Backgrounds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BackgroundID,BackgroundName,BackgroundDescription,BackgroundAmount,CustomUserModelEmail,BackgroundStatus")] Background background)
        {
            if (ModelState.IsValid)
            {
                background.CustomUserModelEmail = User.Identity.Name;
                background.BackgroundStatus = Statuses.Created.ToString();
                _context.Add(background);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(background);
        }

        // GET: Backgrounds/Edit/5
        public async Task<IActionResult> Edit(int? id) {
            if (id == null) {
                return NotFound();
            }

            var background = await _context.Background.FindAsync(id);
            if (background == null) {
                return NotFound();
            }
            return View(background);
        }

        // POST: Backgrounds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BackgroundID,BackgroundName,BackgroundDescription,BackgroundAmount,CustomUserModelEmail,BackgroundStatus")] Background background)
        {
            if (id != background.BackgroundID)
            {
                return NotFound();
            }

            if (background.BackgroundStatus.Equals("Approved"))

                if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(background);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BackgroundExists(background.BackgroundID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(background);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donate(int id, [Bind("BackgroundID,BackgroundName,BackgroundDescription,BackgroundAmount,CustomUserModelEmail,BackgroundStatus,DonationAmount")] DonationViewModel donationViewModel) {
            if (id != donationViewModel.BackgroundID) return NotFound();
            if (!User.IsInRole("Customer")) return RedirectToAction(nameof(Index));

            Console.WriteLine(donationViewModel);

            if (ModelState.IsValid) {
                if (!donationViewModel.BackgroundStatus.Equals("Approved")) return NotFound();

                Donation donation = new Donation {
                    DonationID = 0,
                    BackgroundID = donationViewModel.BackgroundID,
                    CustomerEmail = User.Identity.Name,
                    StaffEmail = donationViewModel.CustomUserModelEmail,
                    DonationAmount = donationViewModel.DonationAmount,
                    DonationDate = DateTime.Now
                };

                _context.Add(donation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Donations");
            }
            return View(donationViewModel);
        }

        // GET: Backgrounds/Donate/5
        public async Task<IActionResult> Donate(int? id) {
            if (id == null) {
                return NotFound();
            }

            var background = await _context.Background.FindAsync(id);
            if (background == null) return NotFound();

			DonationViewModel res = new DonationViewModel {
                DonationID = 0,
                BackgroundID = background.BackgroundID,
                BackgroundName = background.BackgroundName,
                BackgroundDescription = background.BackgroundDescription,
                BackgroundAmount = background.BackgroundAmount,
                CustomUserModelEmail = background.CustomUserModelEmail,
                BackgroundStatus = background.BackgroundStatus,
                DonationAmount = 0
			};
            return View(res);
        }

        // GET: Backgrounds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var background = await _context.Background
                .FirstOrDefaultAsync(m => m.BackgroundID == id);
            if (background == null)
            {
                return NotFound();
            }

            return View(background);
        }

        // POST: Backgrounds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var background = await _context.Background.FindAsync(id);
            _context.Background.Remove(background);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Author: Ka-Shing
        //GET: Backgrounds/Review/5
        public async Task<IActionResult> Review(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var background = await _context.Background
                .FirstOrDefaultAsync(m => m.BackgroundID == id);
            if (background == null)
            {
                return NotFound();
            }

            return View(background);
        }

        //POST: Backgrounds/Review/5
        [HttpPost, ActionName("Review")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReviewConfirmed(int id, string status)
        {
            var background = await _context.Background.FindAsync(id);

            if (status.Equals(Statuses.Approved.ToString()))
            {
                background.BackgroundStatus = Statuses.Approved.ToString();
            }
            else if (status.Equals(Statuses.Rejected.ToString()))
            {
                background.BackgroundStatus = Statuses.Rejected.ToString();
            }

            _context.Background.Update(background);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BackgroundExists(int id)
        {
            return _context.Background.Any(e => e.BackgroundID == id);
        }
    }
}
