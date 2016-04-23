using System.Data.Entity;
using AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities;

namespace AutoMapper.EF6.Extended.Tests.Shared.Data
{
  public class TestingContext : DbContext
  {
    public virtual DbSet<OrderDetailEntity> OrderDetails { get; set; }
  }
}