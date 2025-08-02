using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESLAdmin.Infrastructure.Data.Consts;

//------------------------------------------------------------------------------
//
//                       static class DbConstsIdentity
//
//------------------------------------------------------------------------------
public static class DbConstsIdentity
{
  public const string ID = "id";
  public const string FIRSTNAME = "firstname";
  public const string LASTNAME = "lastName";
  public const string USERNAME = "userName";
  public const string EMAIL = "email";
  public const string PHONENUMBER = "phoneNumber";

  public const string SQL_GETALL =
    @"select
        ""Id"" AS id,
        ""FirstName"" AS firstname,
        ""LastName"" AS lastName,
        ""UserName"" AS userName,
        ""Email"" AS email,
        ""PhoneNumber"" AS phoneNumber
      from
        ""AspNetUsers""";

  public const string SQL_GETUSERROLES =
    @"select
        UR.""UserId"",
        UR.""RoleId"",
         R.""Name""
      from
        ""AspNetUserRoles""  UR
      join
        ""AspNetRoles"" R
      on
        UR.""RoleId"" = R.""Id""";
}
