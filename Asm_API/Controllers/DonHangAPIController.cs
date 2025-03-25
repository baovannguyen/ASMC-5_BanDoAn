using Asm5_BanHoanChinh1.Models;

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol.Core.Types;
using System.Net.Http;
using System.Text;


namespace lab8_bai11.Controllers
{
    public class DonHangAPIController : Controller
    {

        [HttpGet]
        public async Task<IActionResult> Getall()
        {

            List<DonHangModel> reservationList = new List<DonHangModel>();

            using (var httpClient = new HttpClient())

            {

                using (var response = await httpClient.GetAsync("https://localhost:7063/api/DonHangAPI"))

                {

                    string apiResponse = await response.Content.ReadAsStringAsync();

                    reservationList = JsonConvert.DeserializeObject<List<DonHangModel>>(apiResponse);

                }
            }
            return View(reservationList);
        }

        [HttpGet]
        public IActionResult CreateDonHang()
        {
            return View(); // Trả về form nhập sản phẩm
        }

        [HttpPost]
        public async Task<IActionResult> CreateDonHang(DonHangModel product)
        {
            DonHangModel receivedProduct = new DonHangModel();

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(
                    JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync("https://localhost:7063/api/DonHangAPI", content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    receivedProduct = JsonConvert.DeserializeObject<DonHangModel>(apiResponse);
                }
            }

            return RedirectToAction("Getall");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateDonHang(int id)
        {
            DonHangModel product = new DonHangModel();

            using (var HttpClient = new HttpClient())
            {
                using (var response = await HttpClient.GetAsync("https://localhost:7063/api/DonHangAPI/" + id))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    product = JsonConvert.DeserializeObject<DonHangModel>(apiResponse);
                }
            }
            return View(product);
        }

        // Xử lý cập nhật sản phẩm
        [HttpPost]
        public async Task<IActionResult> UpdateDonHang(DonHangModel product)
        {
            DonHangModel Product = new DonHangModel();
            using (var httpClient = new HttpClient())
            {
                var jsonContent = new StringContent(JsonConvert.SerializeObject(product), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync($"https://localhost:7063/api/DonHangAPI/{product.Id}", jsonContent))
                {
                    string apiProduct = await response.Content.ReadAsStringAsync();
                    ViewBag.Result = "Success";
                    Product = JsonConvert.DeserializeObject<DonHangModel>(apiProduct);
                }
            }

            return View(product); // Nếu lỗi, quay lại view cập nhật
        }


        [HttpPost]
        public async Task<IActionResult> DeleteDonHang(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync($"https://localhost:7063/api/DonHangAPI/{id}"))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.Result = "Success";
                    }
                    else
                    {
                        ViewBag.Result = "Failed";
                    }
                }
            }
            return RedirectToAction("Getall");
        }

    }
}
