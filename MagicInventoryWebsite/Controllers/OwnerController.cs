using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MagicInventoryWebsite.Data;
using MagicInventoryWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Win32.SafeHandles;
using SQLitePCL;

namespace MagicInventoryWebsite.Controllers
{


    [Authorize(Roles = MagicConstants.OwnerRole)]
    public class OwnerController : Controller
    {
        private readonly MagicInventoryContext _context;
        private readonly IQueryable<StockRequestVM> _stockReq;

        public OwnerController(MagicInventoryContext context)
        {
            _context = context;

            //JOINS Query to get all the nessecary data for the stock Request
            _stockReq = (from sq in _context.StockRequests
                         join st in _context.Stores on sq.StoreID equals st.StoreID
                         join pr in _context.Products on sq.ProductID equals pr.ProductID
                         join oi in _context.OwnerInventory on sq.ProductID equals oi.ProductID
                         select new StockRequestVM
                         {
                             StockRequestVMID = sq.StockRequestID,
                             StoreName = st.Name,
                             StoreID = st.StoreID,
                             ProductName = pr.Name,
                             Quantity = sq.Quantity,
                             OwnerStockLevel = oi.StockLevel,
                             ProductID = oi.ProductID,
                             StockAvailability = oi.StockLevel >= sq.Quantity

                         });

        }

        // GET: Owner
        public async Task<IActionResult> Index(string productName)
        {
            // Eager loading the Owner Inventory table - join between OwnerInventory and the Product table.
            // the .Include ensures the name will not be blank during the joining
            var query = _context.OwnerInventory.Include(x => x.Product).Select(x => x);

            if (!string.IsNullOrWhiteSpace(productName))
            {
                // Adding a where to the query to filter the data.
                // Note for the first request productName is null thus the where is not always added.
                query = query.Where(x => x.Product.Name.Contains(productName));

                // Storing the search into ViewBag to populate the textbox with the same value for convenience.
                ViewBag.ProductName = productName;
            }

            // Adding an order by to the query for the Product ID.
            query = query.OrderBy(x => x.Product.ProductID);

            // Passing a List<OwnerInventory> model object to the View.
            return View(await query.ToListAsync());
        }

        // GET: Owner
        public async Task<IActionResult> StockRequests()
        {
            // Passing a virtual List<StockRequestVM> model object to the View.
            return View(await _stockReq.ToListAsync());
        }

        // GET: Owner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //gets the owner inventory item with the current id
            var ownerInventory = await _context.OwnerInventory
                .Include(o => o.Product)
                .SingleOrDefaultAsync(m => m.ProductID == id);
            //checks if it exists
            if (ownerInventory == null)
            {
                return NotFound();
            }
            //stores this for display
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", ownerInventory.ProductID);
            return View(ownerInventory);
        }


        // POST: Owner/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductID,StockLevel")] OwnerInventory ownerInventory)
        {
            //checks that it is correct
            if (id != ownerInventory.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ownerInventory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerInventoryExists(ownerInventory.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", ownerInventory.ProductID);
            return View(ownerInventory);
        }

        // GET: Owner/Delete/5
        public async Task<IActionResult> Approve(int? requestId)
        {
            if (requestId == null)
            {
                return NotFound();
            }

            ViewBag.RequestID = requestId;
            // check the request id and the stock availabity
            var available = await _stockReq.Where(x => x.OwnerStockLevel >= x.Quantity).SingleOrDefaultAsync(m => m.StockRequestVMID == requestId);

            //if null this request id does not exist
            if (available == null)
            {
                return NotFound();
            }

            return View(available);
        }

        // POST: Owner/Delete/5
        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveConfirmed(int requestId)
        {
            //this line gets the current request mathcing the given ID
            var stockRequest = await _context.StockRequests.SingleOrDefaultAsync(m => m.StockRequestID == requestId);
            var cProductId = stockRequest.ProductID;
            var cStoreId = stockRequest.StoreID;
            OwnerInventory toUpdateO = new OwnerInventory();
            StoreInventory toUpdateS = new StoreInventory();
            bool toAdd = true;
            //the stock request is removed from the table
            _context.StockRequests.Remove(stockRequest);

            //decrease the stockLevel of the OwnerInventory item
            if (OwnerInventoryExists(cProductId))
            {
                toUpdateO = await _context.OwnerInventory.SingleOrDefaultAsync(e => e.ProductID == cProductId);
                toUpdateO.StockLevel -= stockRequest.Quantity;
            }

            toUpdateS = await _context.StoreInventory.SingleOrDefaultAsync(e => (e.ProductID == cProductId) && (e.StoreID == cStoreId));
            //increase the stockLevel of the store item
            if (toUpdateS != null)
            {
                toUpdateS.StockLevel += stockRequest.Quantity;
                toAdd = false;
            }
            else
            {
                toUpdateS = new StoreInventory()
                {
                    StoreID = cStoreId,
                    ProductID = cProductId,
                    StockLevel = stockRequest.Quantity
                };
            }

            if (ModelState.IsValid)
            {

                try
                {
                    //updates the OwnerInventory 
                    _context.OwnerInventory.Update(toUpdateO);
                    // checks whether it needs to update or add to the Store Inventory
                    if (!toAdd)
                        _context.StoreInventory.Update(toUpdateS);
                    else
                        _context.StoreInventory.Add(toUpdateS);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OwnerInventoryExists(cProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }



            // await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        //if it deose not exist it returns false
        private bool OwnerInventoryExists(int id)
        {
            return _context.OwnerInventory.SingleOrDefaultAsync(e => e.ProductID == id) != null;
        }


    }
}
