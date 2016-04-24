using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities
{
  public class PatientEntity
  {
    [Key]
    public int Id { get; set; }

    public string FirstNameEncrypted { get; set; }
    public string LastNameEncrypted { get; set; }
    public DateTime? BirthDay { get; set; }
    public string GenderEncrypted { get; set; }
    public string ContactPhoneEncrypted { get; set; }
    public string ReferenceNo { get; set; }

    public List<PatientConditionEntity> Conditions { get; set; }
  }
}