using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using AutoMapper.EF6.Extended.Tests.Shared.Encryption;
using AutoMapper.EF6.Extended.Tests.Shared.Samples.DTO;
using AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities;
using Moq;
using Should;
using Xunit;

namespace AutoMapper.EF6.Extended.Tests
{
  public class PostTransformTransformingImplementersTests
  {
    public class TestingContext : DbContext
    {
      public virtual DbSet<PatientEntity> Patients { get; set; }
      public virtual DbSet<ConditionEntity> Conditions { get; set; }
      public virtual DbSet<PatientCondition> PatientConditions { get; set; }
    }

    [Fact]
    public async Task ProjectToListAsync_FinalDTOImplementsPostTransformer_AdjustsItems()
    {
      // Arrange
      var conditions = new List<ConditionEntity>
      {
        new ConditionEntity {Id = 1, Condition = "Hypertension"},
        new ConditionEntity {Id = 2, Condition = "Obesity"}
      };

      var data = new List<PatientEntity>
      {
        new PatientEntity
        {
          Id = 1,
          ContactPhoneEncrypted = EncryptionHelper.Encrypt("123456"),
          GenderEncrypted = EncryptionHelper.Encrypt("Male"),
          FirstNameEncrypted = EncryptionHelper.Encrypt("Bart"),
          LastNameEncrypted = EncryptionHelper.Encrypt("Simpson"),
          ReferenceNo = "XYYZ00001",
          BirthDay = DateTime.Now.AddYears(-10)
        },
        new PatientEntity
        {
          Id = 2,
          ContactPhoneEncrypted = EncryptionHelper.Encrypt("123456"),
          GenderEncrypted = EncryptionHelper.Encrypt("Male"),
          FirstNameEncrypted = EncryptionHelper.Encrypt("Homer"),
          LastNameEncrypted = EncryptionHelper.Encrypt("Simpson"),
          ReferenceNo = "XYYZ00010",
          BirthDay = DateTime.Now.AddYears(-39)
        },
        new PatientEntity
        {
          Id = 3,
          ContactPhoneEncrypted = EncryptionHelper.Encrypt("123456"),
          GenderEncrypted = EncryptionHelper.Encrypt("Female"),
          FirstNameEncrypted = EncryptionHelper.Encrypt("Marge"),
          LastNameEncrypted = EncryptionHelper.Encrypt("Simpson"),
          ReferenceNo = "XYYZ00002"
        }
      };

      var set = new Mock<DbSet<PatientEntity>>().SetupData(data);

      var context = new Mock<TestingContext>();
      context.Setup(c => c.Patients).Returns(set.Object);

      var mapperConfiguration = new MapperConfiguration(cfg =>
      {
        var effectiveDate = DateTime.MinValue;

        cfg.CreateMap<PatientEntity, PatientTransformingDTO>()
          .ForMember(d => d.PatientReferenceNo, o => o.MapFrom(s => s.ReferenceNo))
          .ForMember(d => d.PatientInfoHelper, o => o.MapFrom(s => s))
          .ForMember(d => d.FirstName, o => o.Ignore())
          .ForMember(d => d.LastName, o => o.Ignore())
          .ForMember(d => d.Gender, o => o.Ignore())
          .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.BirthDay))
          .ForMember(d => d.Conditions, o => o.Ignore())
          .ForMember(d => d.Age,
            o =>
              o.MapFrom(
                s => s.BirthDay != null ? (int?) ((effectiveDate - s.BirthDay.Value).TotalDays/365.25) : null as int?));
      });

      // Act
      mapperConfiguration.AssertConfigurationIsValid();

      var sut =
        await
          context.Object.Patients.ProjectToListAsync<PatientTransformingDTO>(mapperConfiguration,
            new {effectiveDate = DateTime.Now});

      // Assert
      sut.Count.ShouldEqual(data.Count);

      var first = sut[0];
      first.Id.ShouldEqual(1);
      first.FirstName.ShouldEqual("Bart");
      first.LastName.ShouldEqual("Simpson");
      first.Gender.HasValue.ShouldBeTrue();
      first.Gender.Value.ShouldEqual(Gender.Male);
      first.DateOfBirth.HasValue.ShouldBeTrue();
      first.Age.ShouldEqual(10);
      first.PatientReferenceNo.ShouldEqual("XYYZ00001");

      var second = sut[1];
      second.Id.ShouldEqual(2);
      second.FirstName.ShouldEqual("Homer");
      second.LastName.ShouldEqual("Simpson");
      second.Gender.HasValue.ShouldBeTrue();
      second.Gender.Value.ShouldEqual(Gender.Male);
      second.DateOfBirth.HasValue.ShouldBeTrue();
      second.Age.ShouldEqual(39);
      second.PatientReferenceNo.ShouldEqual("XYYZ00010");

      var third = sut[2];
      third.Id.ShouldEqual(3);
      third.FirstName.ShouldEqual("Marge");
      third.LastName.ShouldEqual("Simpson");
      third.Gender.HasValue.ShouldBeTrue();
      third.Gender.Value.ShouldEqual(Gender.Female);
      third.DateOfBirth.ShouldBeNull();
      third.Age.ShouldBeNull();
    }
  }
}