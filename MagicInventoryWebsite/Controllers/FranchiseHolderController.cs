
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MagicInventoryWebsite.Models;
using MagicInventoryWebsite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace MagicInventoryWebsite.Controllers
{
    [Authorize(Roles = MagicConstants.FranchiseHolderRole)]
    public class FranchiseHolderController : Controller
    {
        private readonly MagicInventoryContext _context;

        public FranchiseHolderController(MagicInventoryContext context)
        {
            _context = context;
        }

        // GET: FranchiseHolder
        public async Task<IActionResult> Index(string productName)
        {
            //joins the StoreInventory to the Product table and the Stores to create the Franchise Holder data
            var query = _context.StoreInventory.Include(s => s.Product).Include(s => s.Store).Select(x => x);


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

        // GET: FranchiseHolder
        public async Task<IActionResult> Add(int? storeid, string productName)
        {

            if (storeid == null)
            {
                return NotFound();
            }

            //selects the the ProductID for the items in the store 
            var siQuery = _context.StoreInventory.Where(z => z.StoreID == storeid).Select(y => y.ProductID);

            //selects all the item in the Owner inventory except the productIDs in the Store
            var ownerInvNotInStore = _context.OwnerInventory.Where(x => !siQuery.Contains(x.ProductID))
                .Include(s => s.Product).Include(s => s.Product).Select(x => x);
            if (!string.IsNullOrWhiteSpace(productName))
            {
                // Adding a where to the query to filter the data.
                // Note for the first request productName is null thus the where is not always added.
                ownerInvNotInStore = ownerInvNotInStore.Where(x => x.Product.Name.Contains(productName));

                // Storing the search into ViewBag to populate the textbox with the same value for convenience.
                ViewBag.ProductName = productName;
            }

            // Storing the search into ViewBag to populate the textbox with the same value for convenience.
            ViewBag.StoreID = storeid;

            // Passing a List<OwnerInventory> model object to the View.
            return View(await ownerInvNotInStore.ToListAsync());

        }






        // GET: FranchiseHolder/StockRequest/id  this method goes to a specific store item
        //This method is used for requests for items within the store and for a new item request
        public async Task<IActionResult> StockRequest(int storeid, int productid)
        {

            //this query check that the item exists within the store
            var query = await _context.StoreInventory.SingleOrDefaultAsync(m =>
                (m.ProductID == productid) && (m.StoreID == storeid));

            //this query checks that the item exists withing the owner inventory
            var oquery =
                await _context.OwnerInventory.SingleOrDefaultAsync(m => (m.ProductID == productid)); //--to add item

            if (query == null && oquery == null)
            {
                return NotFound();
            }

            //These lists hold the data
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            ViewData["StoreID"] = new SelectList(_context.Stores, "StoreID", "StoreID");

            //this gets the store name in a list with one element for display in the view
            var store = await _context.Stores.SingleOrDefaultAsync(y => y.StoreID == storeid);

            //this gets the product name in a list with one element for display in the view
            var product = await _context.Products.SingleOrDefaultAsync(y => y.ProductID == productid);

            // this is put into a list containing only one item for display
            ViewBag.StoreName = store.Name;
            ViewBag.ProductName = product.Name;
            ViewBag.ProductAvailableQuantity = oquery.StockLevel;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StockRequest([Bind("StoreID,ProductID,Quantity")] StockRequest stockRequest)
        {

            if (ModelState.IsValid)
            {
                //adds the data into the database
                _context.Add(stockRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", stockRequest.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Stores, "StoreID", "StoreID", stockRequest.StoreID);
            return View(stockRequest);
        }




    }
}
