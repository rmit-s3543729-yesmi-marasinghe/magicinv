using System;
using System.Collections.Generic;
using System.Linq;

using MagicInventoryWebsite.Data;
using Microsoft.AspNetCore.Mvc;

namespace MagicInventoryWebsite.Models
{
    public class ShoppingCartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}
