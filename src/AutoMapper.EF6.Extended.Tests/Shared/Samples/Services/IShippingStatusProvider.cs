using AutoMapper.EF6.Extended.Tests.Shared.Samples.DTO;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.Services
{
  public interface IShippingStatusProvider
  {
    ShippingStatus GetStatusFor(OrderDetailDTO detail);
  }
}