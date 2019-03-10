set /p name=Enter a name for the migration: 

dotnet ef migrations add %name% -c MinesweeperIdentityDbContext -o Data/Migrations/IdentityDb