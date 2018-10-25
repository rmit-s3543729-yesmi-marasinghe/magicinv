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
using Microsoft.AspNetCore.Identity;

namespace MagicInventoryWebsite.Controllers
{
    [Authorize(Roles = MagicConstants.CustomerRole)]
    public class CustomerController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MagicInventoryContext _context;
        //this is the username used as the CartID when adding to the Cart
        private string ShoppingCartId;

        public CustomerController(MagicInventoryContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Customer show the owner Items which the user can purchse within the stores
        // cannot purchase on this page since it is the owner items
        public async Task<IActionResult> Index(string productName)
        {
            ShoppingCartId = _userManager.GetUserName(User);
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


            var squery = _context.Stores.Select(x => x).ToList();

            // Storing the Store Name into ViewBag
            ViewBag.StoreName = squery;


            // Adding an order by to the query for the Product name.
            query = query.OrderBy(x => x.Product.ProductID);

            // Passing a List<OwnerInventory> model object to the View.
            return View(await query.ToListAsync());

        }

        public async Task<IActionResult> Store(string storeName, string productName)
        {
            //joins the StoreInventory to the Product table and the Stores to create the Product data
            var query = _context.StoreInventory.Include(s => s.Product).Include(s => s.Store).Where(x => x.Store.Name.Equals(storeName)).Select(x => x);

            //joins the StoreInventory to the Product table and the Stores to create the Franchise Holder data
            var squery = _context.Stores.Where(x => x.Name.Equals(storeName)).Select(x => x);
            var store = await squery.SingleOrDefaultAsync(y => y.Name == storeName);

            ViewBag.StoreID = store.StoreID;
            ViewBag.StoreName = storeName;
            //the store must be specified in order to make a purchase
            if (string.IsNullOrWhiteSpace(storeName))
            {
                return NotFound();

            }


            if (!string.IsNullOrWhiteSpace(productName))
            {

                // Adding a where to the query to filter the data.
                // Note for the first request productName is null thus the where is not always added.
                query = query.Where(x => x.Product.Name.Contains(productName));

                // Storing the search into ViewBag to populate the textbox with the same value for convenience.
                ViewBag.ProductName = productName;
            }

            // Adding an order by to the query for the Product ID.
            query = query.OrderBy(x => x.Product.Name);

            // Passing a List<OwnerInventory> model object to the View.
            return View(await query.ToListAsync());

        }

        public async Task<IActionResult> SelectStore()
        {
            // selecting all the stores
            var squery = _context.Stores.Select(x => x);

            // Storing the search into ViewBag to populate the textbox with the same value for convenience.
            ViewBag.StoreName = squery;


            return View(await squery.ToListAsync());

        }


        // GET: Customer/StoreCheck/id  this method goes to a specific store item
        public async Task<IActionResult> StoreCheck(int storeID, int productID)
        {

            //this query check that the item exists within the store
            var query = await _context.StoreInventory.Include(x => x.Store).SingleOrDefaultAsync(m =>
                (m.ProductID == productID) && (m.Store.StoreID == storeID));


            if (query == null)
            {
                return NotFound();
            }

            //These lists hold the data
            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID");
            ViewData["StoreID"] = new SelectList(_context.Stores, "StoreID", "StoreID");
            ViewData["StoreName"] = new SelectList(_context.Stores, "Name", "Name");

            //this gets the store name in a list with one element for display in the view
            var store = await _context.Stores.SingleOrDefaultAsync(y => y.StoreID == storeID);

            //this gets the product name in a list with one element for display in the view
            var product = await _context.Products.SingleOrDefaultAsync(y => y.ProductID == productID);

            //this query checks that the item exists withing the store inventory
            var squery =
                await _context.StoreInventory.SingleOrDefaultAsync(m => (m.ProductID == productID) && (m.StoreID == storeID));

            // this is put into a list containing only one item for display
            ViewBag.StoreName = store.Name;
            ViewBag.ProductName = product.Name;
            if (squery.StockLevel > 0)
            {
                //this is displayed to the user
                ViewBag.ProductAvailableQuantity = "In Stock";
                //this prevents users from purchasing more than available stock
                ViewBag.ProductAvailableQ = squery.StockLevel;

            }
            else
            {
                //this is displayed to the user
                ViewBag.ProductAvailableQuantity = "Out of Stock";
                //this prevents users from purchasing more than available stock
                ViewBag.ProductAvailableQ = 0;
            }

            ;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StoreCheck([Bind("StoreID,ProductID,CartID,Count")] Cart cart)
        {
            ShoppingCartId = _userManager.GetUserName(User);
            //check the user has a cart 
            ViewBag.UserID = ShoppingCartId;

            // Get the matching cart and product within store
            var cartItem = _context.Carts.SingleOrDefault(
                c => ((c.CartID == ShoppingCartId) && (c.ProductID == cart.ProductID) && (c.StoreID == cart.StoreID)));

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                if (ModelState.IsValid)
                {
                    //time stamps the adding into the cart
                    cart.DateCreated = DateTime.Now;
                    //adds the data into the database
                    _context.Carts.Add(cart);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
            }
            else
            {
                // If the item does exist in the cart, 
                // then add the new purchase to the quantity
                cartItem.Count += cart.Count;
                _context.Update(cartItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Save changes
            _context.SaveChanges();


            ViewData["ProductID"] = new SelectList(_context.Products, "ProductID", "ProductID", cart.ProductID);
            ViewData["StoreID"] = new SelectList(_context.Stores, "StoreID", "StoreID", cart.StoreID);

            // return RedirectToAction("Index", "ShoppingCart");
            return View(cart);

        }


    }
}
