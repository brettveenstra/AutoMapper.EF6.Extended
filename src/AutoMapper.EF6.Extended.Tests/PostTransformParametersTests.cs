using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper.EF6.Extended.Tests.Shared.Samples.DTO;
using AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities;
using AutoMapper.EF6.Extended.Tests.Shared.Samples.Services;
using Moq;
using Should;
using Xunit;

namespace AutoMapper.EF6.Extended.Tests
{
  public class PostTransformParametersTests
  {
    public class TestingContext : DbContext
    {
      public virtual DbSet<OrderDetailEntity> OrderDetails { get; set; }
    }

    [Fact]
    public async Task ProjectToListAsync_ActionNotNull_ModifiesResults()
    {
      // Arrange
      var data = new List<OrderDetailEntity>
      {
        new OrderDetailEntity
        {
          Id = 1,
          OrderId = 1,
          ProductId = 1000,
          UnitPrice = 37.23m,
          Qty = 1m,
          DiscountAmount = 3.5m
        },
        new OrderDetailEntity {Id = 2, OrderId = 1, ProductId = 5, UnitPrice = 0.4756m, Qty = 10000m},
        new OrderDetailEntity {Id = 3, OrderId = 1, ProductId = 37, UnitPrice = 1.97m, Qty = 4.45m}
      };

      var set = new Mock<DbSet<OrderDetailEntity>>().SetupData(data);

      var context = new Mock<TestingContext>();
      context.Setup(c => c.OrderDetails).Returns(set.Object);

      var mapperConfiguration = new MapperConfiguration(cfg =>
      {
        cfg.CreateMap<OrderDetailEntity, OrderDetailDTO>()
          .ForMember(d => d.OrderReference, o => o.MapFrom(s => s.OrderId))
          .ForMember(d => d.LineAmount, o => o.MapFrom(s => s.UnitPrice*s.Qty - s.DiscountAmount.GetValueOrDefault(0)))
          .ForMember(d => d.Discount, o => o.MapFrom(s => s.DiscountAmount.GetValueOrDefault(0)))
          .ForMember(d => d.ShippingStatus, o => o.Ignore());
      });

      var mockShippingStatusProvider = new Mock<IShippingStatusProvider>();
      // basic shipping status (review discounts, start pulling inventory on orders under 100, otherwise await payment)
      mockShippingStatusProvider.Setup(s => s.GetStatusFor(It.Is<OrderDetailDTO>(f => f.Discount > 0)))
        .Returns(ShippingStatus.NeedsReview);
      mockShippingStatusProvider.Setup(
        s => s.GetStatusFor(It.Is<OrderDetailDTO>(f => f.Discount == 0 && f.LineAmount < 100)))
        .Returns(ShippingStatus.PullingInventory);
      mockShippingStatusProvider.Setup(
        s => s.GetStatusFor(It.Is<OrderDetailDTO>(f => f.Discount == 0 && f.LineAmount > 100)))
        .Returns(ShippingStatus.AwaitingPayment);

      var cancellationToken = new CancellationToken();

      // Act
      mapperConfiguration.AssertConfigurationIsValid();

      var sut =
        await
          context.Object.OrderDetails.ProjectToListActionAsync<OrderDetailDTO>(mapperConfiguration, 
            t => { t.ShippingStatus = mockShippingStatusProvider.Object.GetStatusFor(t); }, cancellationToken);

      // Assert
      mockShippingStatusProvider.Verify(s => s.GetStatusFor(It.IsAny<OrderDetailDTO>()),
        Times.Exactly(data.Count));
      mockShippingStatusProvider.VerifyAll();

      sut.Count.ShouldEqual(data.Count); // normal AutoMapper Projection behavior

      var first = sut[0];
      first.Id.ShouldEqual(1);
      first.OrderReference.ShouldEqual(1);
      first.LineAmount.ShouldEqual(data[0].UnitPrice*data[0].Qty - data[0].DiscountAmount.GetValueOrDefault(0));
      first.Discount.ShouldEqual(data[0].DiscountAmount.GetValueOrDefault(0));
      first.ShippingStatus.ShouldEqual(ShippingStatus.NeedsReview);

      var second = sut[1];
      second.Id.ShouldEqual(2);
      second.OrderReference.ShouldEqual(1);
      second.LineAmount.ShouldEqual(data[1].UnitPrice*data[1].Qty - data[1].DiscountAmount.GetValueOrDefault(0));
      second.Discount.ShouldEqual(data[1].DiscountAmount.GetValueOrDefault(0));
      second.ShippingStatus.ShouldEqual(ShippingStatus.AwaitingPayment);

      var third = sut[2];
      third.Id.ShouldEqual(3);
      third.OrderReference.ShouldEqual(1);
      third.LineAmount.ShouldEqual(data[2].UnitPrice*data[2].Qty - data[2].DiscountAmount.GetValueOrDefault(0));
      third.Discount.ShouldEqual(data[2].DiscountAmount.GetValueOrDefault(0));
      third.ShippingStatus.ShouldEqual(ShippingStatus.PullingInventory);
    }

    [Fact]
    public async Task ProjectToListAsync_ParameterizedProjections_AffectsResults()
    {
      // Arrange
      var firstOrder = new OrderEntity {Id = 1, OrderDate = DateTime.Now.AddDays(-7), CustomerId = 1};
      var secondOrder = new OrderEntity {Id = 2, OrderDate = DateTime.Now.AddDays(-2), CustomerId = 37};
      var futureOrder = new OrderEntity {Id = 3, OrderDate = DateTime.Now.AddDays(2), CustomerId = 44};

      var data = new List<OrderDetailEntity>
      {
        new OrderDetailEntity
        {
          Id = 1,
          OrderId = 1,
          ProductId = 1000,
          UnitPrice = 37.23m,
          Qty = 1m,
          DiscountAmount = 3.5m,
          Order = firstOrder
        },
        new OrderDetailEntity
        {
          Id = 2,
          OrderId = 2,
          ProductId = 5,
          UnitPrice = 0.4756m,
          Qty = 10000m,
          Order = secondOrder
        },
        new OrderDetailEntity
        {
          Id = 3,
          OrderId = 3,
          ProductId = 37,
          UnitPrice = 1.97m,
          Qty = 4.45m,
          Order = futureOrder
        }
      };

      var set = new Mock<DbSet<OrderDetailEntity>>().SetupData(data);

      var context = new Mock<TestingContext>();
      context.Setup(c => c.OrderDetails).Returns(set.Object);

      var mapperConfiguration = new MapperConfiguration(cfg =>
      {
        var effectiveDate = DateTime.MinValue;

        cfg.CreateMap<OrderDetailEntity, OrderDetailDTO>()
          .ForMember(d => d.OrderReference, o => o.MapFrom(s => s.OrderId))
          .ForMember(d => d.LineAmount, o => o.MapFrom(s => s.UnitPrice*s.Qty - s.DiscountAmount.GetValueOrDefault(0)))
          .ForMember(d => d.Discount, o => o.MapFrom(s => s.DiscountAmount.GetValueOrDefault(0)))
          .ForMember(d => d.PreOrder, o => o.MapFrom(s => s.Order.OrderDate > effectiveDate ? true : (bool?) null))
          .ForMember(d => d.ShippingStatus, o => o.Ignore());
      });

      var mockShippingStatusProvider = new Mock<IShippingStatusProvider>();
      // basic shipping status (review discounts, start pulling inventory on orders under 100, otherwise await payment)
      mockShippingStatusProvider.Setup(s => s.GetStatusFor(It.Is<OrderDetailDTO>(f => f.Discount > 0)))
        .Returns(ShippingStatus.NeedsReview);
      mockShippingStatusProvider.Setup(
        s => s.GetStatusFor(It.Is<OrderDetailDTO>(f => f.Discount == 0 && f.LineAmount < 100)))
        .Returns(ShippingStatus.PullingInventory);
      mockShippingStatusProvider.Setup(
        s => s.GetStatusFor(It.Is<OrderDetailDTO>(f => f.Discount == 0 && f.LineAmount > 100)))
        .Returns(ShippingStatus.AwaitingPayment);

      var cancellationToken = new CancellationToken();

      // Act
      mapperConfiguration.AssertConfigurationIsValid();

      var sut =
        await
          context.Object.OrderDetails.ProjectToListActionAsync<OrderDetailDTO>(mapperConfiguration,
            t => { t.ShippingStatus = mockShippingStatusProvider.Object.GetStatusFor(t); },
            new {effectiveDate = DateTime.Now}, cancellationToken);

      // Assert
      mockShippingStatusProvider.Verify(s => s.GetStatusFor(It.IsAny<OrderDetailDTO>()),
        Times.Exactly(data.Count));
      mockShippingStatusProvider.VerifyAll();

      sut.Count.ShouldEqual(data.Count); // normal AutoMapper Projection behavior

      var first = sut[0];
      first.Id.ShouldEqual(1);
      first.OrderReference.ShouldEqual(1);
      first.LineAmount.ShouldEqual(data[0].UnitPrice*data[0].Qty - data[0].DiscountAmount.GetValueOrDefault(0));
      first.Discount.ShouldEqual(data[0].DiscountAmount.GetValueOrDefault(0));
      first.ShippingStatus.ShouldEqual(ShippingStatus.NeedsReview);
      first.PreOrder.HasValue.ShouldBeFalse();

      var second = sut[1];
      second.Id.ShouldEqual(2);
      second.OrderReference.ShouldEqual(2);
      second.LineAmount.ShouldEqual(data[1].UnitPrice*data[1].Qty - data[1].DiscountAmount.GetValueOrDefault(0));
      second.Discount.ShouldEqual(data[1].DiscountAmount.GetValueOrDefault(0));
      second.ShippingStatus.ShouldEqual(ShippingStatus.AwaitingPayment);
      second.PreOrder.HasValue.ShouldBeFalse();

      var third = sut[2];
      third.Id.ShouldEqual(3);
      third.OrderReference.ShouldEqual(3);
      third.LineAmount.ShouldEqual(data[2].UnitPrice*data[2].Qty - data[2].DiscountAmount.GetValueOrDefault(0));
      third.Discount.ShouldEqual(data[2].DiscountAmount.GetValueOrDefault(0));
      third.ShippingStatus.ShouldEqual(ShippingStatus.PullingInventory);
      third.PreOrder.HasValue.ShouldBeTrue();
      third.PreOrder.Value.ShouldBeTrue();
    }
  }
}