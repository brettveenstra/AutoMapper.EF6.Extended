using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities
{
  public class ConditionEntity
  {
    [Key]
    public int Id { get; set; }

    public string Condition { get; set; }
    public List<string> Symptoms { get; set; }
  }
}