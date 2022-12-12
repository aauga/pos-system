# Point-of-Sale system

Point-of-sale system project for VU Software Design and Architecture course.

## Run the application

```
dotnet run --project WebApi
```

## Create a migration

```
dotnet ef migration add NAME_OF_MIGRATION -p Infrastructure -s WebApi
```

## Update database

```
dotnet ef database update -p Infrastructure -s WebApi
```
