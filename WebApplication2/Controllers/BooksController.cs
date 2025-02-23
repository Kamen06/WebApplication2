using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;


namespace WebApplication2.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Books.Include(b => b.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .Include(b => b.Author).Include(b=>b.Reviews)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName");
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Genre,Summary,BookPicture,AuthorId")] Book book, IFormFile ImageFile)
        {
            book.Author = _context.Authors.FirstOrDefault(a => a.Id == book.AuthorId);
            ModelState.Remove("Author"); // ✅ Fix validation issue
            ModelState.Remove("BookPicture"); // ✅ Fix validation issue

            if (ModelState.IsValid)
            {
                // ✅ Handle image upload if a file is provided
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Generate a unique filename
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    // Save the uploaded image
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    // ✅ Save the file path in the database
                    book.BookPicture = "/uploads/" + fileName;
                }
                else
                {
                    // ✅ Set default image if no file is uploaded
                    book.BookPicture = "/uploads/default-book.png"; // Ensure this default image exists
                }

                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // ✅ Ensure the dropdown is repopulated if validation fails
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
            return View(book);
        }




        // GET: Books/Edit/5
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
            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
            return View(book);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Genre,Summary,AuthorId")] Book book, IFormFile? ImageFile)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            ModelState.Remove("Author"); // Fix validation for navigation property
            ModelState.Remove("BookPicture"); // Prevents ModelState from rejecting empty BookPicture

            if (ModelState.IsValid)
            {
                try
                {
                    var existingBook = await _context.Books
                        .AsNoTracking()
                        .FirstOrDefaultAsync(b => b.Id == id);

                    if (existingBook == null)
                    {
                        return NotFound();
                    }

                    // ✅ Preserve existing BookPicture if no new file is uploaded
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // ✅ Delete old book cover if it exists
                        if (!string.IsNullOrEmpty(existingBook.BookPicture))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingBook.BookPicture.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // ✅ Save new image path
                        book.BookPicture = "/uploads/" + fileName;
                    }
                    else
                    {
                        // ✅ Keep the existing book cover if no new image is uploaded
                        book.BookPicture = existingBook.BookPicture;
                    }

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
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Set<Author>(), "Id", "FullName", book.AuthorId);
            return View(book);
        }
        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
