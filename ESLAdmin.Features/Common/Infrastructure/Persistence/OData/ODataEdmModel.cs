using ESLAdmin.Infrastructure.Persistence.Entities;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace ESLAdmin.Features.Common.Infrastructure.Persistence.OData;

public static class ODataEdmModel
{
  public static IEdmModel GetEdmModel()
  {
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<ClassLevel>("ClassLevels");
    return builder.GetEdmModel();
  }
}
