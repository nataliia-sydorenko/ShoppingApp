using Newtonsoft.Json;
using Polly;
using ShoppingApp.Models;
using System;
using System.Text;

namespace ShoppingApp
{
    public class ShoppingService
    {
        public const string GetAllProducts = "products?limit=";
        public const string GetAllCategories = "products/categories";
        public const string GetProductsByCategory = "products/category/";
        public const string AddProduct = "products";
        public const string UpdateOrDeleteProduct = "products/";
        private readonly HttpClient _httpClient;

        public ShoppingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Product>?> GetAllProductsAsync(int limit)
        {
            var retryPolicy = Policy.Handle<Exception>().OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode).RetryAsync();
            var response = await GetAsync(GetAllProducts + limit.ToString(), retryPolicy);
            return JsonConvert.DeserializeObject<List<Product>>(response);
        }

        public async Task<List<string>?> GetAllCategoriesAsync()
        {
            var retryPolicy = Policy.Handle<Exception>().OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode).WaitAndRetryAsync(10, _ => TimeSpan.FromMinutes(5));
            var response = await GetAsync(GetAllCategories, retryPolicy);
            return JsonConvert.DeserializeObject<List<string>>(response);
        }

        public async Task<List<Product>?> GetProductsByCategoryAsync(string category)
        {
            var circuitBreakerPolicy = Policy.Handle<Exception>().OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode).CircuitBreakerAsync(3, TimeSpan.FromMinutes(10));
            var response = await GetAsync(GetProductsByCategory + category, circuitBreakerPolicy);
            return JsonConvert.DeserializeObject<List<Product>>(response);
        }

        public async Task AddProductAsync(Product product)
        {
            var fallbackPolicy = Policy.Handle<Exception>().OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode).FallbackAsync(new HttpResponseMessage() { Content = new StringContent("Failed to add a new product") });
            var requestContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await fallbackPolicy.ExecuteAsync(() => _httpClient.PostAsync(AddProduct, requestContent));
        }

        public async Task<Product> UpdateProductAsync(Product product)
        {
            var timeoutPolicy = Policy.TimeoutAsync(TimeSpan.FromSeconds(1));
            var requestContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
            var response = await timeoutPolicy.ExecuteAsync(() => _httpClient.PutAsync(UpdateOrDeleteProduct + product.Id, requestContent));
            var resconseContent = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Product>(resconseContent);
        }

        public async Task DeleteProductAsync(int id)
        {
            await _httpClient.DeleteAsync(UpdateOrDeleteProduct + id);
        }

        private async Task<string> GetAsync(string url, IAsyncPolicy<HttpResponseMessage> policy)
        {
            var response = await policy.ExecuteAsync(() => _httpClient.GetAsync(url));
            return await response.Content.ReadAsStringAsync();
        }
    }
}
