using ECommerceMvcSite.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System;

public class ApprovedOrder
{
    public int Id { get; set; }

    [ForeignKey("Order")]
    public int OrderId { get; set; }

    public DateTime ApprovedDate { get; set; }

    public virtual Order Order { get; set; }
}
