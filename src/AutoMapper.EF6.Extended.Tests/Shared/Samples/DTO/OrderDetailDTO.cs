using AutoMapper.EF6.Extended.Tests.Shared.Samples.Services;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.DTO
{
  public class OrderDetailDTO
  {
    public int Id { get; set; }
    public int OrderReference { get; set; }
    public decimal LineAmount { get; set; }
    public ShippingStatus? ShippingStatus { get; set; }
    public decimal Discount { get; set; }

    [IgnoreMap]
    public bool? PreOrder { get; set; }
  }
}