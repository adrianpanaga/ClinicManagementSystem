using System;
using System.Collections.Generic;

namespace ClinicManagement.Data.Models;

public partial class InventoryItem
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? Description { get; set; }

    public string? Sku { get; set; }

    public string UnitOfMeasure { get; set; } = null!;

    public string? Category { get; set; }

    public int CurrentStock { get; set; }

    public int ReorderPoint { get; set; }

    public int? SupplierId { get; set; }

    public decimal? PurchasePrice { get; set; }

    public decimal? SellingPrice { get; set; }

    public bool? ExpiryDateTracking { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ItemBatch> ItemBatches { get; set; } = new List<ItemBatch>();

    public virtual ICollection<StockTransaction> StockTransactions { get; set; } = new List<StockTransaction>();

    public virtual Vendor? Supplier { get; set; }
}
