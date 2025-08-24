using ESLAdmin.Infrastructure.Persistence.Entities;

namespace ESLAdmin.Features.ChildcareLevels.Infrastructure.Persistence.Entities;

//------------------------------------------------------------------------------
//
//                        Class ChildcareLevel
//
//------------------------------------------------------------------------------
public class ChildcareLevel : EntityBase
{
    public long Id { get; set; } = 0;
    public string ChildcareLevelName { get; set; } = string.Empty;
    public int MaxCapacity { get; set; } = 0;
    public int DisplayOrder { get; set; } = 0;
    public int PlacesAssigned { get; set; } = 0;
}