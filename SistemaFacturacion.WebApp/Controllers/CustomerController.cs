using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using SistemaFacturacion.WebApp.Models;

namespace SistemaFacturacion.WebApp.Controllers
{
    public class CustomerController : Controller
    {


        private readonly IHttpClientFactory _clientFactory;

        public CustomerController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("ProductsApi");
            var response = await client.GetAsync("api/Customer/GetAllAsync");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Customers = JsonSerializer.Deserialize<List<Customer>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(Customers);
            }

            return View(new List<Customer>());
        }

        public IActionResult Create()
        {
            return View();
        }

       [HttpPost]
        public async Task<IActionResult> Create(Customer Customer)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("ProductsApi");
                var json = JsonSerializer.Serialize(Customer);
                var  content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/Customer", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(Customer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient("ProductsApi");
            var response = await client.GetAsync($"api/Customer/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var Customer = JsonSerializer.Deserialize<Customer>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(Customer);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Customer Customer)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("ProductsApi");
                var json = JsonSerializer.Serialize(Customer);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/Customer/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(Customer);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("ProductsApi");
            var response = await client.DeleteAsync($"api/Customer/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}
