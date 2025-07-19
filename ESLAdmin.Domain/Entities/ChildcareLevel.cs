using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ESLAdmin.Domain.Entities;

//------------------------------------------------------------------------------
//
//                        Class ChildcareLevel
//
//------------------------------------------------------------------------------
[Table("CHILDCARELEVELS")]
public class ChildcareLevel
{
  [Key]
  [Column("CHILDCARELEVELID")]
  public long Id { get; set; }

  [Column("CHILDCARELEVELNAME")]
  [StringLength(32)]
  public string ChildcareLevelName { get; set; } = string.Empty;

  [Column("MAXCAPACITY")]
  public int MaxCapacity { get; set; }

  [Column("DISPLAYORDER")]
  public int DisplayOrder { get; set; }

  [Column("PLACESASSIGNED")]
  public int PlacesAssigned { get; set; }

  [Column("INITUSER")]
  public long InitUser { get; set; }

  [Column("INITDATE")]
  public DateTime InitDate { get; set; }

  [Column("USERCODE")]
  public long UserCode { get; set; }

  [Column("USERSTAMP")]
  public DateTime UserStamp { get; set; }

  [Column("GUID")]
  [MaxLength(38)]
  public string Guid { get; set; } = string.Empty;
}
