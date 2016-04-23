using System.ComponentModel.DataAnnotations;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities
{
  public class OrderDetailEntity
  {
    [Key]
    public int Id { get; set; }

    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Qty { get; set; }
    public decimal? DiscountAmount { get; set; }
    public OrderEntity Order { get; set; }
  }
}