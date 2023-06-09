﻿using System.ComponentModel.DataAnnotations.Schema; 

namespace Order.Domain.AggregateModels;
[Table("OrderItem")]
public class OrderItem
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public virtual Orders Order { get; set; }
    public virtual Product Product { get; set; }

    public OrderItem(int OrderId, int ProductId, int Quantity, decimal Price) {
        this.OrderId = OrderId;
        this.ProductId = ProductId;
        this.Quantity = Quantity;
        this.Price = Price;
    }
    public OrderItem()
    {
        
    }

    //chi tinh toan -+ field entity cua no , get/set value nhung field trong this entity
    public decimal SubTotal()
    {
        return this.Quantity * this.Price;
    }
}
