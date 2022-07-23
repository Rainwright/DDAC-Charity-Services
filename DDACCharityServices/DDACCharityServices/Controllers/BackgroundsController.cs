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
            var bgList =  _context.Background.AsEnumerable();

            const string nameSearchKeyword = "Background Name";
            const string requestorSearchKeyword = "Requestor Name";
            const string statusSearchKeyword = "Status";

            var SearchKeywordList = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Selected=true, Text="-", Value=""},
                        new SelectListItem { Selected=false, Text=nameSearchKeyword, Value=nameSearchKeyword},
                        new SelectListItem { Selected=false, Text=requestorSearchKeyword, Value=requestorSearchKeyword},
                        new SelectListItem { Selected=false, Text=statusSearchKeyword, Value=statusSearchKeyword},
                    },
                    "Value", "Text", 1
                   );

            ViewBag.SearchKeywordList = SearchKeywordList;

            if (!string.IsNullOrEmpty(searchstring))
            {
                if (!string.IsNullOrEmpty(searchkeyword))
                {
                    switch (searchkeyword)
                    {
                        case nameSearchKeyword:
                            bgList = bgList.Where(background => background.BackgroundName.Contains(searchstring));
                            break;
                        case requestorSearchKeyword:
                            bgList = bgList.Where(background => background.CustomUserModelEmail.Contains(searchstring));
                            break;
                        case statusSearchKeyword:
                            bgList = bgList.Where(background => background.BackgroundStatus.Contains(searchstring));
                            break;
                    }
                }
            }

            List<CustomBackgroundModel> bgViewList = new List<CustomBackgroundModel>();

            bgList.ToList().ForEach(
                bg => {
                    var model = new CustomBackgroundModel();
                    model.CopyFromBackground(bg);
                    model.TotalDonations = _context.Donation
                        .Where(donation => donation.Background.BackgroundID == bg.BackgroundID)
                        .Sum(donation => donation.DonationAmount);
                    bgViewList.Add(model);
                }
            );

            return View(bgViewList);
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
        public async Task<IActionResult> Donate(int id, [Bind("DonationAmount")] Donation donation) {
            if (!User.IsInRole("Customer")) return RedirectToAction(nameof(Index));

            donation.BackgroundID = id;
            donation.Background = await _context.Background.FirstOrDefaultAsync(m => m.BackgroundID == id);
            donation.CustomerEmail = User.Identity.Name;
            donation.DonationDate = DateTime.Now;

            if (ModelState.IsValid) {
                if (!donation.Background.BackgroundStatus.Equals("Approved")) return NotFound();
                
                _context.Add(donation);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Donations");
            }
            return View(donation);
        }

        // GET: Backgrounds/Donate/5
        public async Task<IActionResult> Donate(int? id) {
            if (id == null) return NotFound();

            var background = await _context.Background.FindAsync(id);
            if (background == null || !background.BackgroundStatus.Equals("Approved")) return NotFound();

			Donation res = new Donation {
                DonationID = 0,
                DonationAmount = 0,
                DonationDate = DateTime.Now
			};
            res.Background = background;

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
