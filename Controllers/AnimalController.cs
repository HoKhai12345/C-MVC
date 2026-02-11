using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NET_MVC.Data;
using NET_MVC.Dto;
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

        [Route("danh-sach-thu-cung")]
        [Authorize]
        public IActionResult Index()
        {
            //var animals = _context.Animals
            //    .Include(a => a.Category)
            //    .Include(a => a.AnimalKeepers)
            //    .ThenInclude(ak => ak.Keeper)
            //    .ToList();
            
            var animals = _context.Animals
        .Select(a => new AnimalViewModel
        {
            Id = a.Id,
            Name = a.Name,
            // Truy cập bảng Categories (1-N)
            Age = a.Age,
            CategoryName = a.Category.Name,
            // Truy cập bảng trung gian để lấy tên Keeper (N-N)
            ListKeeper = a.AnimalKeepers.Select(ak => ak.Keeper.FullName).ToArray()
        })
        .ToList();
            return Ok(animals);
        }

        [Route("chi-tiet-thu-cung/{id}")]
        public IActionResult Detail(int id)
        {
            var animal = new
            {
                Id = 1,
                Name = "Chó",
                Price = 6000,
            };
            //ViewBag.DogName = "Pitbull";

            var pet = _context.Animals
                       .Include(a => a.Category) // "Join" với bảng Category (Loài)
                       .Include(a => a.AnimalKeepers)
                       .FirstOrDefault(x => x.Id == id); // Điều kiện WHERE Id = id

            // Kiểm tra xem có dữ liệu không
            if (animal == null)
            {
                return NotFound();
            }

            ViewBag.FakeAnimal = animal;
            return View(pet);
        }
    }
}
