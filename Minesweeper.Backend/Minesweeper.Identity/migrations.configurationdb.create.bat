set /p name=Enter a name for the migration: 

dotnet ef migrations add %name% -c ConfigurationDbContext -o Data/Migrations/IdentityServer/ConfigurationDb