set /p name=Enter a name for the migration: 

dotnet ef migrations add %name% -c PersistedGrantDbContext -o Data/Migrations/IdentityServer/PersistedGrantDb