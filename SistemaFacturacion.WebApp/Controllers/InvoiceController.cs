using Microsoft.AspNetCore.Mvc;
using SistemaFacturacion.WebApp.Models;
using System.Text.Json;
using System.Text;
using SistemaFacturacion.WebApp.DTO;

namespace SistemaFacturacion.WebApp.Controllers
{

    
    public class InvoiceController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public InvoiceController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("ProductsApi");
            var response = await client.GetAsync("api/Invoice");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var invoices = JsonSerializer.Deserialize<List<InvoiceListItemDTO>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(invoices);
            }

            return View(new List<InvoiceListItemDTO>());
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Invoice Invoice)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("ProductsApi");
                var json = JsonSerializer.Serialize(Invoice);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/Invoice/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(Invoice);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("ProductsApi");
            var response = await client.DeleteAsync($"api/Invoice/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }


        public async Task<IActionResult> Create()
        {
            var client = _clientFactory.CreateClient("ProductsApi");
            var customersResponse = await client.GetAsync("api/customer/GetAllAsync");
            var productsResponse = await client.GetAsync("api/product");

            if (customersResponse.IsSuccessStatusCode)
            {

                var customersContent = await customersResponse.Content.ReadAsStringAsync();
                var productContent = await productsResponse.Content.ReadAsStringAsync();

                ViewBag.Customers = JsonSerializer.Deserialize<List<Customer>>(customersContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Products = JsonSerializer.Deserialize<List<Product>>(productContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });



                return View(new Invoice());


            }



            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("ProductsApi");
                invoice.Date = DateTime.Now;
                var json = JsonSerializer.Serialize(invoice);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/invoice", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View("Error");
        }
    }
}
