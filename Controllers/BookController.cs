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
    //BookController class with support for CRUD
    public class BookController : Controller
    {
        //Readonly permissions
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string wwwRootPath;

        //Context and host environments to handle database and store files in project
        public BookController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            wwwRootPath = hostEnvironment.WebRootPath;
        }

        // GET: Book
        //Returns the Index view for books
        //Lists all books along with the author of the book
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Books.Include(b => b.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Book/Details/5
        //Returns a NotFound function if there is no book with a matching ID
        //Returns the book details and includes Author
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Book/Create
        //Teturns the Create view for a book
        //Includes a list of authors to select from for the book
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name");
            return View();
        }

        // POST: Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,ImageFile,AuthorId")] Book book)
        {
            if (ModelState.IsValid)
            {
                //check if image has been set
                if (book.ImageFile != null)
                {
                    //Generate unique file name for the image by combining the file name, a timestamp and the file type
                    string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                    string extension = Path.GetExtension(book.ImageFile.FileName);

                    //Ensures there are no spaces and adds timestamp to file name
                    book.ImageName = fileName = fileName.Replace(" ", String.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                    //Setting the path to where the image is to be stored
                    string path = Path.Combine(wwwRootPath + "/images", fileName);

                    //Store image in filesystem 
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await book.ImageFile.CopyToAsync(fileStream);
                    }
                }

                //Add book and redirect user to the Index view for books
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //Includes a list of authors to select from for the book
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name", book.AuthorId);
            return View(book);
        }

        // GET: Book/Edit/5
        //Returns the Edit view for a specific book
        //Returns a NotFound function if there is no book with a matching ID
        //Includes a list of authors to select from for the book
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name", book.AuthorId);
            return View(book);
        }

        // POST: Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,ImageFile,AuthorId,ImageName")] Book book)
        {
            //Returns a NotFound function if there is no book with a matching ID
            if (id != book.Id)
            {
                return NotFound();
            }

            //Check validation in model to ensure all required fields are set and valid
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if a new image file is uploaded
                    if (book.ImageFile != null)
                    {
                        //Generate unique file name for the image by combining the file name, a timestamp and the file type
                        string fileName = Path.GetFileNameWithoutExtension(book.ImageFile.FileName);
                        string extension = Path.GetExtension(book.ImageFile.FileName);

                        //Ensures there are no spaces and adds timestamp to file name
                        string newFileName = fileName.Replace(" ", string.Empty) + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/images", newFileName);

                        // Store new image file in project
                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await book.ImageFile.CopyToAsync(fileStream);
                        }

                        // Delete the old image file if it exists and a new image has been set
                        if (!string.IsNullOrEmpty(book.ImageName))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath + "/images", book.ImageName);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Update the image name in the database
                        book.ImageName = newFileName;
                    }
                    else
                    {
                        // If no new image is uploaded, keep the current image name
                        _context.Entry(book).Property(x => x.ImageName).IsModified = false;
                    }

                    // Update the book details
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //Redirects the user back to the Index view
                return RedirectToAction(nameof(Index));
            }
            //Includes a list of authors to select from for the book
            ViewData["AuthorId"] = new SelectList(_context.Authors, "Id", "Name", book.AuthorId);
            return View(book);
        }


        // GET: Book/Delete/5
        //Reurns the Delete view
        //Returns a NotFound function if there is no book with a matching ID
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Gets the book details and author 
            var book = await _context.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Checks the database for a book with the given Id and removes it
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            //Saves the updated database and redirects the user back to the book Index view
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Checks if book exists
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
