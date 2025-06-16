using System;
using System.Collections.Generic;

namespace ClinicManagement.Data.Models;

public partial class ItemBatch
{
    public int BatchId { get; set; }

    public int ItemId { get; set; }

    public string BatchNumber { get; set; } = null!;

    public DateOnly ExpiryDate { get; set; }

    public int Quantity { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public string? Notes { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual InventoryItem Item { get; set; } = null!;
}
