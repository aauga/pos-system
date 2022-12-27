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

## What is different from the Swagger document

- In the data model (its impossible to do this laboratory work without it), an employee can have only one position but can have many users. Why would you need many user accounts if you work in the same store? Most probably it was a mistake and it should've been a N:M connection between employee and position. But then the User entity did not store any info about Position, so it is unknown to which Position to assign the User to. It's a mishmash, so a decision was made to merge Position and User into Employee, so that for each store a new Employee would be created.
