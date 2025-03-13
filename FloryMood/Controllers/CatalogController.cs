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
            DeliveryInfo = p.delivery
        }).ToList();

        ViewBag.CurrentCategory = category; // Передаем текущую категорию в представление

        return View(viewModel);
    }
}
