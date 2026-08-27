using Microsoft.AspNetCore.Mvc;
using ParkingManagement.Web.Models.ParkingLot;
using ParkingManagement.Web.Services.Interfaces;

namespace ParkingManagement.Web.Controllers;

public class HomeController : Controller
{
    private readonly IParkingLotService _parkingLotService;

    public HomeController(IParkingLotService parkingLotService)
    {
        _parkingLotService = parkingLotService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _parkingLotService.SearchAsync(null);

        var parkingLots = result?.Success == true && result.Data != null
            ? result.Data
            : new List<ParkingLotViewModel>();

        return View(parkingLots);
    }
    public IActionResult Guide() => View();
    public IActionResult News() => View();
    public IActionResult Contact() => View();
}