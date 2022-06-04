using BookListMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookListMVC2.Controllers
{
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _db;
        [BindProperty]
        public Book Book { get; set; }
        public BooksController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id) // A single link to both Create & Update Books
        {
            Book = new Book();
            if (id == null)
            {
                // Show the create view
                return View(Book);
            } else
            {
                // Update
                Book = _db.Books.FirstOrDefault(u => u.Id == id);
                if(Book == null)
                {
                    return NotFound();
                } else
                {
                    // Display Update view for the found book
                    return View(Book);
                }

            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert()
        { // Also need to check if ModelState is valid
            if (ModelState.IsValid)
            {
                if (Book.Id == null)
                { // If it does not exist, create
                    _db.Add(Book);
                }
                else
                { // Otherwise, update existing book
                    _db.Update(Book);
                }
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region API Calls

        [HttpGet]
        public IActionResult GetAll() // API made available for DataTables JS library
        {
            return Json(new { data = _db.Books.ToList() });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id) // JSON response for sweet alerts JS library
        {
            var bookFromDb = await _db.Books.FindAsync(id);
            _db.Books.Remove(bookFromDb);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
