/*This code uses sections of https://docs.microsoft.com/en-us/aspnet/mvc/overview/older-versions/mvc-music-store/mvc-music-store-part-8 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagicInventoryWebsite.Data;
using MagicInventoryWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;



namespace MagicInventoryWebsite.Controllers
{
    [Authorize(Roles = MagicConstants.CustomerRole)]
    public class ShoppingCartController : Controller
    {
        private readonly MagicInventoryContext _storeDB;
        // this is used to get the username for verification and use ad the cartID
        private readonly UserManager<ApplicationUser> _userManager;

        private string userId;
        public ShoppingCartController(MagicInventoryContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _storeDB = context;
        }


        //
        // GET: /ShoppingCart/
        public async Task<IActionResult> Index()
        {
            userId = _userManager.GetUserName(User);

            //check the user has a cart (cart is set up within CustomerController)
            ViewBag.UserID = userId;
            var cartList = await _storeDB.Carts.Include(x => x.Product).Include(y => y.Store).Where(cart => cart.CartID == userId).ToListAsync();
            var viewModel = new ShoppingCartViewModel(); ;
            if ((userId) != null)
            {
                // Set up our ViewModel
                viewModel = new ShoppingCartViewModel
                {
                    CartItems = cartList,
                    CartTotal = GetTotal(userId)
                };

            }

            // Return the view
            return View(viewModel);
        }

        // GET: /ShoppingCart/Order
        //this method was not used since credit card verification was not completed
        public async Task<IActionResult> Order()
        {
            userId = _userManager.GetUserName(User);

            //check the user has a cart 
            ViewBag.UserID = userId;
            var cartList = await _storeDB.Carts.Include(x => x.Product).Include(y => y.Store).Where(cart => cart.CartID == userId).ToListAsync();
            var viewModel = new ShoppingCartViewModel(); ;
            if ((userId) != null)
            {

                // Set up our ViewModel
                viewModel = new ShoppingCartViewModel
                {
                    CartItems = cartList,
                    CartTotal = GetTotal(userId)
                };

            }

            // Return the view
            return View(viewModel);
        }

        [HttpPost, ActionName("Order")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOrder()
        {
            userId = _userManager.GetUserName(User);

            //check the user has a cart 
            ViewBag.UserID = userId;
            var cartList = await _storeDB.Carts.Include(x => x.Product).Include(y => y.Store).Where(cart => cart.CartID == userId).ToListAsync();
            var viewModel = new ShoppingCartViewModel(); ;
            var order = new Order();
            var orderList = new List<OrderDetail>();
            if ((userId) != null)
            {

                // Set up our ViewModel
                viewModel = new ShoppingCartViewModel
                {
                    CartItems = cartList,
                    CartTotal = GetTotal(userId)
                };

            }

            //create an order
            order = new Order
            {
                Username = userId,
                Total = GetTotal(userId),
                OrderDate = DateTime.Now,
            };

            decimal orderTotal = 0;

            var cartItems = cartList;
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    ProductID = item.ProductID,
                    StoreID = item.StoreID,
                    OrderID = order.OrderID,
                    Price = item.Product.Price,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Product.Price);

                _storeDB.OrderDetails.Add(orderDetail);
                orderList.Add(orderDetail);

            }

            foreach (var item in cartItems)
            {
                var toUpdateS = await _storeDB.StoreInventory.SingleOrDefaultAsync(e =>
                    (e.ProductID == item.ProductID) && (e.StoreID == item.StoreID));
                //decrease the stockLevel of the store item after purchase
                if (toUpdateS != null)
                {
                    toUpdateS.StockLevel -= item.Count;
                    _storeDB.StoreInventory.Update(toUpdateS);
                }

            }

            // Set the order's total to the orderTotal count
            order.Total = orderTotal;
            order.OrderDetails = orderList;
            _storeDB.Orders.Add(order);


            // Save the order
            _storeDB.SaveChanges();
            // Empty the shopping cart
            EmptyCart(userId);

            return RedirectToAction(nameof(Index));


        }

        // GET: Cart/Update is the method of deleting an item form the cart
        public async Task<IActionResult> UpdateCart(int recordID)
        {
            // Remove the item from the cart
            var cart = await _storeDB.Carts.Include(x => x.Product).Include(y => y.Store).SingleOrDefaultAsync(c => c.RecordID == recordID);

            //if null this record id does not exist
            if (cart == null)
            {
                return NotFound();
            }

            ViewBag.RecordID = recordID;

            return View(cart);
        }

        [HttpPost, ActionName("UpdateCart")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCartNow(int recordID)
        {

            // Remove the item from the cart
            var cart = await _storeDB.Carts.Include(x => x.Product).Include(y => y.Store).SingleOrDefaultAsync(c => c.RecordID == recordID);

            //if null this record id does not exist
            if (cart == null)
            {
                return NotFound();


            }

            //removes the entire cart item if there is only one    
            _storeDB.Carts.Remove(cart);
            // Save changes
            _storeDB.SaveChanges();

            return RedirectToAction(nameof(Index));


        }

        public decimal GetTotal(string shoppingCartId)
        {
            // Multiply item prices by count of that product to get 
            // the current price for each of those products in the cart
            // sum all totals to get the cart total
            decimal? total = (from cartItems in _storeDB.Carts
                              where cartItems.CartID == shoppingCartId
                              select (int?)cartItems.Count *
                                     cartItems.Product.Price).Sum();

            return total ?? decimal.Zero;
        }

        public void EmptyCart(string shoppingCartId)
        {
            var cartItems = _storeDB.Carts.Where(
                cart => cart.CartID == shoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _storeDB.Carts.Remove(cartItem);
            }
            // Save changes
            _storeDB.SaveChanges();
        }
    }
}
