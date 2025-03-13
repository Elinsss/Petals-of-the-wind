
public class SupabaseClientService
{
    private readonly Supabase.Client _client;

    public SupabaseClientService(string supabaseUrl, string supabaseKey)
    {
        _client = new Supabase.Client(supabaseUrl, supabaseKey);
    }

    public async Task InitializeAsync()
    {
        await _client.InitializeAsync();
    }

    public Supabase.Client Client => _client;
}
