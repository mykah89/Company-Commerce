using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Company.Commerce.Entity.Models
{
    public class Order
    {
        public Order()
        {
            OrderLines = new List<OrderLine>();
        }

        public Int32 OrderId { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public Decimal BasePrice { get; set; }

        public DateTime? ShippedDate { get; set; }

        public Decimal ShippingCost { get; set; }

        public Decimal Tax { get; set; }

        public Decimal TotalPrice
        {
            get
            {
                return this.BasePrice + this.ShippingCost + this.Tax;
            }
        }

        public Int32 UserId { get; set; }

        public virtual IList<OrderLine> OrderLines { get; set; }

        public virtual User User { get; set; }
    }
}
