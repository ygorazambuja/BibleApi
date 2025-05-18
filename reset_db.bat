dotnet ef migrations remove --project Api
dotnet ef database drop --project Api
dotnet ef migrations add InitMigrate --project Api
dotnet ef database update --project Api