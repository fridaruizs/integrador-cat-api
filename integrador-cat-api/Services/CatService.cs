using integrador_cat_api.Models;
using System.Text.Json;

namespace integrador_cat_api.Services
{
    public interface ICatService
    {
        IEnumerable<Cat> GetAll();
        Cat GetById(int id);
        Cat Add(string name);
        Task<string> AssignImageToCatAsync(int id);
        Task<bool> CheckExternalApiHealthAsync();

    }
    public class CatService: ICatService
    {
        private readonly List<Cat> _cats = new List<Cat>();
        private readonly HttpClient _httpClient;
        private int _nextId = 1;

        public CatService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://api.thecatapi.com/v1/");
        }

        public IEnumerable<Cat> GetAll() => _cats;

        public Cat GetById(int id) => _cats.FirstOrDefault(c => c.Id == id);
        public Cat GetByName(string name) => _cats.FirstOrDefault(c => c.Name == name);

        public Cat Add(string name)
        {
            var cat = new Cat
            {
                Id = _nextId++,
                Name = name,
                CreatedAt = DateTime.UtcNow
            };
            _cats.Add(cat);
            return cat;
        }

        public async Task<string> AssignImageToCatAsync(int id)
        {
            var cat = GetById(id);
            if (cat == null)
            {
                throw new KeyNotFoundException($"Cat with id {id} not found.");
            }

            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<CatImage>>("images/search");
                if (response != null && response.Count > 0)
                {
                    cat.ImageUrl = response[0].Url;
                    return cat.ImageUrl;
                }
                else
                {
                    throw new Exception("No cat images returned from the API.");
                }
            }
            catch (HttpRequestException e)
            {
                throw new Exception("Error fetching cat image from API.", e);
            }
            catch (System.Text.Json.JsonException e)
            {
                throw new Exception("Error parsing API response.", e);
            }
        }

        public async Task<bool> CheckExternalApiHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("images/search");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private class CatImage
        {
            public string Id { get; set; }
            public string Url { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

    }
}
