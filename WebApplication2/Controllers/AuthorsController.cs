using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Authors.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.Include(r=>r.Books)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FullName,Deceased,ProfilePicture,Biography")] Author author, IFormFile ImageFile)
        {
            
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    author.ProfilePicture = "/uploads/" + fileName;
                }
                else
                {
                    author.ProfilePicture = "/uploads/default-profile.png"; 
                }

                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(author);
        }

        // GET: Authors/Edit/5
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

        //POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Deceased,Biography")] Author author)
        //{
        //    if (id != author.Id)
        //    {
        //        return NotFound();
        //    }

        //    //author.ProfilePicture = _context.Authors.Find(author.Id).ProfilePicture;
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(author);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AuthorExists(author.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(author);
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Deceased,Biography")] Author author, IFormFile? ImageFile)
        {
            if (id != author.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingAuthor = await _context.Authors
                        .AsNoTracking()
                        .FirstOrDefaultAsync(a => a.Id == id);

                    if (existingAuthor == null)
                    {
                        return NotFound();
                    }

                    // ✅ If no new file is uploaded, preserve the existing profile picture
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(stream);
                        }

                        // ✅ Delete old image if it exists
                        if (!string.IsNullOrEmpty(existingAuthor.ProfilePicture))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingAuthor.ProfilePicture.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // ✅ Save new image path
                        author.ProfilePicture = "/uploads/" + fileName;
                    }
                    else
                    {
                        // ✅ Preserve existing profile picture
                        author.ProfilePicture = existingAuthor.ProfilePicture;
                    }

                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Authors.Any(e => e.Id == author.Id))
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
            return View(author);
        }

        /////3.1
        //public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Deceased,ProfilePicture,Biography")] Author author)
        //{
        //    if (id != author.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(author);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AuthorExists(author.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(author);
        //}
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Deceased,Biography,ProfilePicture")] Author author/*, IFormFile ImageFile*/)
        //{
        //    if (id != author.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var existingAuthor = await _context.Authors.FindAsync(id);

        //            if (existingAuthor == null)
        //            {
        //                return NotFound();
        //            }

        //            // ✅ Handle image upload if a new file is provided
        //            if (ImageFile != null && ImageFile.Length > 0)
        //            {
        //                // Generate a unique filename
        //                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
        //                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

        //                // Save the new image
        //                using (var stream = new FileStream(filePath, FileMode.Create))
        //                {
        //                    await ImageFile.CopyToAsync(stream);
        //                }

        //                // ✅ Delete the old image if it exists
        //                if (!string.IsNullOrEmpty(existingAuthor.ProfilePicture))
        //                {
        //                    var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingAuthor.ProfilePicture.TrimStart('/'));
        //                    if (System.IO.File.Exists(oldImagePath))
        //                    {
        //                        System.IO.File.Delete(oldImagePath);
        //                    }
        //                }

        //                // ✅ Save new image path in the database
        //                existingAuthor.ProfilePicture = "/uploads/" + fileName;
        //            }

        //            // ✅ Update other fields
        //            existingAuthor.FullName = author.FullName;
        //            existingAuthor.Deceased = author.Deceased;
        //            existingAuthor.Biography = author.Biography;

        //            _context.Update(existingAuthor);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!AuthorExists(author.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(author);
        //}
        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Id == id);
        }
    }
}
