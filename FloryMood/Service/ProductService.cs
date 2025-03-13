using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest;
using Supabase.Postgrest.Exceptions;
using Лепестки_ветра.Models;

public class ProductService
{
    private readonly Supabase.Client _client;

    public ProductService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    public async Task<List<Product?>> GetProductAsync()
    {
        var response = await _client.From<Product>().Get();

        return response.Models;
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        var response = await _client.From<Product>()
            .Where(p => p.type == category)
            .Get();

        return response.Models;
    }
}