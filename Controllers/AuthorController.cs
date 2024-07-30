using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MomentTre.Data;
using MomentTre.Models;

namespace MomentTre.Controllers
{
    //AuthorController class with support for CRUD
    public class AuthorController : Controller
    {
        //Readonly permissions
        private readonly ApplicationDbContext _context;

        //Context and host environments to handle database
        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Author
        //Returns the Index view for authors
        //Lists all authors along with the books by the author
        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors
            // Include related books
            .Include(a => a.Books)
            .ToListAsync();
            return View(authors);
        }

        // GET: Author/Details/5
        //Returns a NotFound function if there is no author with a matching ID
        //Returns the author details and includes the books by the author
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                // Include related books
                .Include(a => a.Books)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Author/Create
        //Returns the Create view for an author
        public IActionResult Create()
        {
            return View();
        }

        // POST: Author/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Author author)
        {
            //Check validation in model to ensure all required fields are set and valid
            if (ModelState.IsValid)
            {
                //Add author and redirect user to the Index view for authors
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Author/Edit/5
        //Returns the Edit view for a specific author
        //Returns a NotFound function if there is no author with a matching ID
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Author/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Author author)
        {
            //Returns a NotFound function if there is no author with a matching ID
            if (id != author.Id)
            {
                return NotFound();
            }

            //Check validation in model to ensure all required fields are set and valid
            if (ModelState.IsValid)
            {
                //Updates the author details and saves the changes
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //Redirects the user back to the author index view
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Author/Delete/5
        //Reurns the Delete view
        //Returns a NotFound function if there is no author with a matching ID
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Gets the author details 
            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Checks the database for an author with the given Id and removes it
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Checks if author exists
        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
