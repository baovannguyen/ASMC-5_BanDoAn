using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Asm5_BanHoanChinh1.Models;
using Asm5_BanHoanChinh1.data;

namespace Asm5_BanHoanChinh1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonHangAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DonHangAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/DonHangAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DonHangModel>>> GetDonHangs()
        {
            return await _context.DonHangs.ToListAsync();
        }

        // GET: api/DonHangAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DonHangModel>> GetDonHangModel(int id)
        {
            var donHangModel = await _context.DonHangs.FindAsync(id);

            if (donHangModel == null)
            {
                return NotFound();
            }

            return donHangModel;
        }

        // PUT: api/DonHangAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDonHangModel(int id, DonHangModel donHangModel)
        {
            if (id != donHangModel.Id)
            {
                return BadRequest();
            }

            var existingOrder = await _context.DonHangs.FindAsync(id);
            if (existingOrder == null)
            {
                return NotFound();
            }

            // Cập nhật thuộc tính
            existingOrder.KhachHangId = donHangModel.KhachHangId;
            existingOrder.NgayDat = donHangModel.NgayDat;
            existingOrder.TrangThai = donHangModel.TrangThai;
            existingOrder.TongTien = donHangModel.TongTien;

            // Đánh dấu entity là Modified
            _context.Entry(existingOrder).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return NoContent();
        }


        // POST: api/DonHangAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DonHangModel>> PostDonHangModel(DonHangModel donHangModel)
        {
            _context.DonHangs.Add(donHangModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDonHangModel", new { id = donHangModel.Id }, donHangModel);
        }

        // DELETE: api/DonHangAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDonHangModel(int id)
        {
            var donHangModel = await _context.DonHangs.FindAsync(id);
            if (donHangModel == null)
            {
                return NotFound();
            }

            _context.DonHangs.Remove(donHangModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DonHangModelExists(int id)
        {
            return _context.DonHangs.Any(e => e.Id == id);
        }
    }
}
