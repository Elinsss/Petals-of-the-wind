using Supabase.Postgrest.Exceptions;
using Лепестки_ветра.Models;

public class WishlistService
{
    private readonly Supabase.Client _client;

    public WishlistService(SupabaseClientService clientService)
    {
        _client = clientService.Client;
    }

    public async Task<bool> AddToWishlistAsync(int userId, int productId)
    {
        try
        {
            var existingItem = await _client.From<Wishlist>()
                .Where(w => w.id_buyer == userId && w.id_product == productId)
                .Single();

            if (existingItem != null)
                return false;

            await _client.From<Wishlist>().Insert(new Wishlist { id_buyer = userId, id_product = productId });
            return true;
        }
        catch (PostgrestException)
        {
            return false;
        }
    }

    public async Task<bool> RemoveFromWishlistAsync(int userId, int productId)
    {
        try
        {
            await _client.From<Wishlist>()
                .Where(w => w.id_buyer == userId && w.id_product == productId)
                .Delete();
            return true;
        }
        catch (PostgrestException)
        {
            return false;
        }
    }

    public async Task<List<Wishlist>> GetWishlistAsync(int userId)
    {
        var response = await _client.From<Wishlist>()
            .Where(w => w.id_buyer == userId)
            .Get();

        foreach (var item in response.Models)
        {
            item.Product = (await _client.From<Product>()
                .Where(p => p.id == item.id_product)
                .Get()).Models.FirstOrDefault();
        }

        return response.Models;
    }
}
