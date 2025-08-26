using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Features.Countries.UpdateCountry;

//------------------------------------------------------------------------------
//
//                           class UpdateCountryRequest
//
//------------------------------------------------------------------------------
public class UpdateCountryRequest
{
  public long CountryId { get; set; }
  public string CountryName { get; set; } = string.Empty;
  public string LanguageName {  get; set; } = string.Empty;
  public long UserCode { get; set; }
  public string Guid { get; set; } = string.Empty;
}

//------------------------------------------------------------------------------
//
//                           class UpdateCountryResponse
//
//------------------------------------------------------------------------------
public class UpdateCountryResponse
{
  public long CountryId { get; set; }
  public string Guid { get; set; } = string.Empty;
}
