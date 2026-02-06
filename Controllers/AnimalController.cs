using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NET_MVC.Data;
using NET_MVC.Models;
using System.Text.Json;
namespace NET_MVC.Controllers
{
    public class AnimalController : Controller
    {
        private readonly AppDbContext _context;
        public AnimalController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var animals = _context.Animals
                .Include(a => a.Category)
                .Include(a => a.AnimalKeepers)
                .ThenInclude(ak => ak.Keeper)
                .ToList();
            return View(animals);
        }
    }
}
