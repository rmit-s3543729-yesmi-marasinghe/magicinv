using System.Collections.Generic;

namespace MagicInventoryWebsite.Models
{
    public class Store
    {
        public int StoreID { get; set; }

        public string Name { get; set; }

        //This additional Information assosciates a Franchis Holder Login with a store
        public string User { get; set; }

        //how to creaty a FK (many)
        public ICollection<StoreInventory> StoreInventory { get; } = new List<StoreInventory>();
    }
}
