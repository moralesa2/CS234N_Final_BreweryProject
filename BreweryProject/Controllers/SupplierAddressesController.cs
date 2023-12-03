using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BreweryClasses.Models;

namespace BreweryProjectRESTAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierAddressesController : ControllerBase
    {
        private readonly BitsContext _context;

        public SupplierAddressesController(BitsContext context)
        {
            _context = context;
        }

        // GET: api/SupplierAddresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierAddress>>> GetSupplierAddresses()
        {
          if (_context.SupplierAddresses == null)
          {
              return NotFound();
          }
            return await _context.SupplierAddresses.ToListAsync();
        }

        // GET: api/SupplierAddresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<SupplierAddress>> GetSupplierAddress(int id)
        {
          if (_context.SupplierAddresses == null)
          {
              return NotFound();
          }
            var supplierAddress = await _context.SupplierAddresses.FindAsync(id);

            if (supplierAddress == null)
            {
                return NotFound();
            }

            return supplierAddress;
        }

        // PUT: api/SupplierAddresses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSupplierAddress(int id, SupplierAddress supplierAddress)
        {
            if (id != supplierAddress.SupplierId)
            {
                return BadRequest();
            }

            _context.Entry(supplierAddress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierAddressExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SupplierAddresses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SupplierAddress>> PostSupplierAddress(SupplierAddress supplierAddress)
        {
          if (_context.SupplierAddresses == null)
          {
              return Problem("Entity set 'BitsContext.SupplierAddresses'  is null.");
          }
            _context.SupplierAddresses.Add(supplierAddress);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SupplierAddressExists(supplierAddress.SupplierId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetSupplierAddress", new { id = supplierAddress.SupplierId }, supplierAddress);
        }

        // DELETE: api/SupplierAddresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSupplierAddress(int id)
        {
            if (_context.SupplierAddresses == null)
            {
                return NotFound();
            }
            var supplierAddress = await _context.SupplierAddresses.FindAsync(id);
            if (supplierAddress == null)
            {
                return NotFound();
            }

            _context.SupplierAddresses.Remove(supplierAddress);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierAddressExists(int id)
        {
            return (_context.SupplierAddresses?.Any(e => e.SupplierId == id)).GetValueOrDefault();
        }
    }
}
