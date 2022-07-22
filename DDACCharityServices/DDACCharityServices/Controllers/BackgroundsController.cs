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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var background = await _context.Background.FindAsync(id);
            if (background == null)
            {
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
