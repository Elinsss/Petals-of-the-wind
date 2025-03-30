using Microsoft.AspNetCore.Mvc;

public class CatalogController : Controller
{
    private readonly ProductService _productService;

    public CatalogController(ProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> catalog(string category = "Тюльпаны")
    {
        var products = await _productService.GetProductsByCategoryAsync(category);
        var viewModel = products.Select(p => new ProductViewModel
        {
            Id = p.id,
            Name = p.name,
            Price = p.price,
            ImageUrl = p.imageurl,
            IsHit = p.is_hit,
            DeliveryInfo = p.delivery,
        }).ToList();

        ViewBag.CurrentCategory = category;

        return View(viewModel);
    }

    public async Task<IActionResult> Item(int id)
    {
        var product = await _productService.GetProductsById(id);
        if (product == null) return NotFound();
        return View(product);
    }
}
