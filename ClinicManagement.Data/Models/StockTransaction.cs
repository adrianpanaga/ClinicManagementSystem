using System;
using System.Collections.Generic;

namespace ClinicManagement.Data.Models;

public partial class StockTransaction
{
    public int TransactionId { get; set; }

    public int ItemId { get; set; }

    public string TransactionType { get; set; } = null!;

    public int Quantity { get; set; }

    public DateTime TransactionDate { get; set; }

    public int PerformedByUserId { get; set; }

    public string? SourceDestination { get; set; }

    public string? Notes { get; set; }

    public int? RelatedRecordId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual InventoryItem Item { get; set; } = null!;

    public virtual User PerformedByUser { get; set; } = null!;
}
