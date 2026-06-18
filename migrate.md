# Commands to run in your VS Code terminal

## 1. Restore all NuGet packages
dotnet restore

## 2. Install EF Core CLI tool (run once, globally)
dotnet tool install --global dotnet-ef

## 3. Create the migration (reads your models, generates SQL)
dotnet ef migrations add InitialCreate

## 4. Apply migration to database (creates the tables)
dotnet ef database update

## 5. Run the API
dotnet run
