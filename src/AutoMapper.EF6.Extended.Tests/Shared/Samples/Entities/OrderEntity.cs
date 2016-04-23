using System;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities
{
  public class OrderEntity
  {
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }
  }
}