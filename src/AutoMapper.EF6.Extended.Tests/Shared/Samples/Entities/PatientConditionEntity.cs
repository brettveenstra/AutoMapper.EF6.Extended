using System;
using System.ComponentModel.DataAnnotations;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities
{
  public class PatientConditionEntity
  {
    [Key]
    public int Id { get; set; }

    public int PatientId { get; set; }
    public int ConditionId { get; set; }

    public PatientEntity Patient { get; set; }
    public ConditionEntity Condition { get; set; }

    public DateTime? DeterminationDate { get; set; }
  }
}