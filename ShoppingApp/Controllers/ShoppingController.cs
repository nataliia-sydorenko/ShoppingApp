using Microsoft.AspNetCore.Mvc;
using Polly;
using ShoppingApp.Models;
using System.Diagnostics;

namespace ShoppingApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ShoppingController : ControllerBase
    {
        
        private readonly ShoppingService _shoppingService;
        private readonly ILogger<ShoppingController> _logger;

        public ShoppingController(ILogger<ShoppingController> logger, ShoppingService shoppingService)
        {
            _logger = logger;
            _shoppingService = shoppingService;
        }

        [HttpGet("GetAllProducts")]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _shoppingService.GetAllProductsAsync(5);
            return Ok(products);
        }

        [HttpGet("GetAllCategories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _shoppingService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("GetProductsByCategory/{category}")]
        public async Task<IActionResult> GetProductsByCategory(string category)
        {
            var products = await _shoppingService.GetProductsByCategoryAsync(category);
            return Ok(products);
        }

        [HttpPost("AddProduct")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            await _shoppingService.AddProductAsync(product);
            return Ok();
        }

        [HttpPut("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct([FromHeader] Product product)
        {
            var newProduct = await _shoppingService.UpdateProductAsync(product);
            return Ok(newProduct);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _shoppingService.DeleteProductAsync(id);
            return Ok();
        }

        //[HttpGet("TestRetry")]
        //public async Task<IActionResult> TestRetry()
        //{
        //    var count = -1;
        //    var httpClient = new HttpClient();
        //    // by default retry once
        //    var retryPolicy = Policy.Handle<Exception>().OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode).RetryAsync();
        //    await retryPolicy.ExecuteAsync(() =>
        //    {
        //        count++;
        //        // returns Unauthorized ex
        //        return httpClient.GetAsync(@"https://api.openai.com/v1/models");
        //    });
        //    return Ok(new {retryCount = count});
        //}

        //[HttpGet("TestWaitAndRetry")]
        //public async Task<IActionResult> TestWaitAndRetry()
        //{
        //    var count = -1;
        //    var httpClient = new HttpClient();
        //    var retryPolicy = Policy.Handle<Exception>().OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode).WaitAndRetryAsync(2, _ => TimeSpan.FromSeconds(5));
        //    var sw = new Stopwatch();
        //    await retryPolicy.ExecuteAsync(() =>
        //    {
        //        count++;
        //        sw.Start();
        //        // returns Unauthorized ex
        //        return httpClient.GetAsync(@"https://api.openai.com/v1/models");
        //    });
        //    sw.Stop();
        //    return Ok(new { retryCount = count, waitingTime = sw.Elapsed.Seconds / count });
        //}

        //[HttpGet("TestCircuitBreaker")]
        //public async Task<IActionResult> TestCircuitBreaker()
        //{
        //    var count = 0;
        //    var httpClient = new HttpClient();
        //    var circuitBreakerPolicy = Policy.Handle<Exception>().OrResult<HttpResponseMessage>(m => !m.IsSuccessStatusCode).CircuitBreakerAsync(3, TimeSpan.FromSeconds(60));
        //    try
        //    {
        //        // when exceeds the allowed number of handled events throws ex
        //        for (int i = 0; i < 5; i++)
        //        {
        //            await circuitBreakerPolicy.ExecuteAsync(() =>
        //            {
        //                count++;
        //                // returns Unauthorized ex
        //                return httpClient.GetAsync(@"https://api.openai.com/v1/models");
        //            });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(ex.Message + $" Attempt count {count}");
        //    }
        //    return Ok();
        //}
    }
}