using System;
using System.Collections.Generic;
using AutoMapper.EF6.Extended.Tests.Shared.Encryption;
using AutoMapper.EF6.Extended.Tests.Shared.Samples.Entities;

namespace AutoMapper.EF6.Extended.Tests.Shared.Samples.DTO
{
  public class PatientTransformingDTO : IPostProjectionTransformer
  {
    public int Id { get; set; }
    public string PatientReferenceNo { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Gender? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public List<PatientCondition> Conditions { get; set; }

    /// <summary>
    ///   This would be marked IGNORED in Serialized settings (e.g. [JsonIgnore])
    /// </summary>
    public PatientEntity PatientInfoHelper { get; set; }

    public void Transform()
    {
      FirstName = EncryptionHelper.Decrypt(PatientInfoHelper.FirstNameEncrypted);
      LastName = EncryptionHelper.Decrypt(PatientInfoHelper.LastNameEncrypted);

      if (PatientInfoHelper.GenderEncrypted != null)
      {
        var genderDecrypted = EncryptionHelper.Decrypt(PatientInfoHelper.GenderEncrypted);
        Gender = (Gender?) Enum.Parse(typeof(Gender), genderDecrypted, true);
      }
    }
  }
}