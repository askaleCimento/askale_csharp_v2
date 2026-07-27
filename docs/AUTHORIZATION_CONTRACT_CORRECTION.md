# Authorization contract correction

The existing application does not authorize screens with a single role identifier.
It uses JWT role claims such as `ROLE_{moduleId}_SEE`, `ROLE_{moduleId}_ADD`,
`ROLE_{moduleId}_EDIT`, `ROLE_{moduleId}_DELETE`, `ROLE_{moduleId}_LOGS`, and
`ROLE_{moduleId}_APPROVE`.

The authentication endpoints therefore expose `authorities` and no longer expose
`roleId` in the authentication response or JWT custom claims. The database
`AdminUser.roleId` field remains an internal relation used to build the authority
claims from `RoleDetail`.

Example response fragment:

```json
{
  "user": {
    "id": 1,
    "username": "user",
    "name": "User",
    "authorities": [
      "ROLE_12_SEE",
      "ROLE_12_EDIT"
    ]
  }
}
```
