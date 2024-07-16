using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Movie_Project.Data;
using Movie_Project.Models;

namespace Movie_Project.Controllers
{
    public class MovieController : Controller
    {
        private readonly MovieDBContext _context;
        private readonly IWebHostEnvironment _environment;

        public MovieController(MovieDBContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }


        // GET: Movies
        public async Task<IActionResult> Index()
        {
            var movieDBContext = _context.Movies.Include(m => m.gener);
            return View(await movieDBContext.ToListAsync());
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.gener)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // GET: Movies/Create
        public IActionResult Create()
        {
            ViewData["GenerId"] = new SelectList(_context.Geners, "Id", "genre");
            ViewData["GenerName"] = new SelectList(_context.Geners, "Id", "genre");

            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MovieId,Title,Image,actors,FilmProducer,Description,YearOfRelease,rating,Duartion,WatchLink,DownloadeLink,GenerId")] Movie movie, IFormFile img_file)
        {
            string path = Path.Combine(_environment.WebRootPath, "Img"); // wwwroot/Img/
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (img_file != null)
            {
                path = Path.Combine(path, img_file.FileName); // for exmple : /Img/Photoname.png
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await img_file.CopyToAsync(stream);
                    //ViewBag.Message = string.Format("<b>{0}</b> uploaded.</br>", img_file.FileName.ToString());
                    movie.Image = img_file.FileName;

                }
            }
            else
            {
                movie.Image = "default.jpeg"; // to save the default image path in database.
            }
            try
            {
                _context.Add(movie);
                _context.SaveChanges();
                return RedirectToAction("Index", "Movie");
            }
            catch (Exception ex) { ViewBag.exc = ex.Message; }
            ViewData["GenerId"] = new SelectList(_context.Geners, "Id", "genre", movie.GenerId);
            //ViewData["GenerName"] = new SelectList(_context.Geners, "genre", "genrename", movie.GenerName);

            return View();
            //if (ModelState.IsValid)
            //{
             
            //    _context.Add(movie);
            //    await _context.SaveChangesAsync();
            //    return RedirectToAction("Index", "Movie");
            //}
            //ViewData["GenerId"] = new SelectList(_context.Geners, "Id", "genre", movie.GenerId);
            //return View(movie);


        }

        // GET: Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            ViewData["GenerId"] = new SelectList(_context.Geners, "Id", "genre", movie.GenerId);
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MovieId,Title,Image,actors,FilmProducer,Description,YearOfRelease,rating,Duartion,WatchLink,DownloadeLink,GenerId")] Movie movie)
        {
            if (id != movie.MovieId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(movie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MovieExists(movie.MovieId))
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
            ViewData["GenerId"] = new SelectList(_context.Geners, "Id", "genre", movie.GenerId);
            return View(movie);
        }

        // GET: Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Movies == null)
            {
                return NotFound();
            }

            var movie = await _context.Movies
                .Include(m => m.gener)
                .FirstOrDefaultAsync(m => m.MovieId == id);
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Movies == null)
            {
                return Problem("Entity set 'MovieDBContext.Movies'  is null.");
            }
            var movie = await _context.Movies.FindAsync(id);
            if (movie != null)
            {
                _context.Movies.Remove(movie);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MovieExists(int id)
        {
          return (_context.Movies?.Any(e => e.MovieId == id)).GetValueOrDefault();
        }


        public IActionResult FilterByYear()
        {

            return View();

        }
        //filter Movies with Year
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterByYear(int YearOfRelease)
        {

            var filter1 = _context.Movies.Where(x => x.YearOfRelease == YearOfRelease).ToList();

            return View(filter1);

        }
        public IActionResult FilterByGener()
        {
            return View();
        }
        //filter Movies with Gener
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FilterByGener(string Gener)
        {
            var selectedgener = getId(Gener);
            int generID = selectedgener.Id;

            var filter2 = _context.Movies.Where(x => x.GenerId == generID).ToList();

            return View(filter2);

        }
        private Gener getId(string Gener)
        {

            var gener = _context.Geners.FirstOrDefault(u => u.genre == Gener);

            //var geners = _context.Geners.Where(x=>x.genre==Gener);

            return gener;
        }
        public IActionResult Search()
        {

            return View();

        }
        //filter Movies with Year
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(string Title)
        {

            var search = _context.Movies.Where(x => x.Title.Contains(Title)).ToList();

            return View(search);

        }

        public async Task<IActionResult> IndexAdmin()
        {
            return _context.Movies != null ?
                          View(await _context.Movies.ToListAsync()) :
                          Problem("Entity set 'MovieDBContext.Movies'  is null.");
            //return View();

        }
        public async Task<IActionResult> IndexUser()
        {
            return _context.Movies != null ?
                          View(await _context.Movies.ToListAsync()) :
                          Problem("Entity set 'MovieDBContext.Movies'  is null.");
            //return View();

        }
        
    }
}
