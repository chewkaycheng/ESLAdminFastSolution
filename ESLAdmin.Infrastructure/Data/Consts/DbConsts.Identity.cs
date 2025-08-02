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
  const string ID = "id";
  const string FIRSTNAME = "firstname";
  const string LASTNAME = "lastName";
  const string USERNAME = "userName";
  const string EMAIL = "email";
  const string PHONENUMBER = "phoneNumber";

  const string GETALLUSERS =
    @"select
        ""Id"" AS id,
        ""FirstName"" AS firstname,
        ""LastName"" AS lastName,
        ""UserName"" AS userName,
        ""Email"" AS email,
        ""PhoneNumber"" AS phoneNumber
      from
        ""AspNetUsers""";

  const string GETUSERROLES =
    @""
        ur."UserId",
        ur."RoleId",
         r."Name"
      FROM
        "AspNetUserRoles"  ur
      JOIN
        "AspNetRoles" r
      ON
        ur."RoleId" = r."Id"
}
