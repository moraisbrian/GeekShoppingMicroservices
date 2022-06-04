using Microsoft.AspNetCore.Mvc;
using GeekShopping.Web.Services.Interfaces;
using GeekShopping.Web.Models;

namespace GeekShopping.Web.Controllers;

public class ProductController : Controller
{
    private readonly IProductService _service;

    public ProductController(IProductService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async Task<IActionResult> Index()
    {
        var products = await _service.FindAll();
        return View(products);
    }

    public IActionResult ProductCreate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> ProductCreate(ProductModel product)
    {
        if (ModelState.IsValid)
        {
            var response = await _service.Create(product);
            if (response != null)
                return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    public async Task<IActionResult> ProductUpdate(long id)
    {
        var product = await _service.FindById(id);
        if (product != null)
            return View(product);

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductUpdate(ProductModel product)
    {
        if (ModelState.IsValid)
        {
            var response = await _service.Update(product);
            if (response != null)
                return RedirectToAction(nameof(Index));
        }

        return View(product);
    }

    public async Task<IActionResult> ProductDelete(long id)
    {
        var product = await _service.FindById(id);
        if (product != null)
            return View(product);

        return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ProductDelete(ProductModel product)
    {
        var response = await _service.Delete(product.Id);
        if (response)
            return RedirectToAction(nameof(Index));
        
        return View(product);
    }
}