
# Point-of-Sale system

Point-of-sale system project for VU Software Design and Architecture course. Not all endpoints are implemented from the given documentation, only the order flow (order, item, cart, payment and delivery creation and querying) and a basic auth for employees.

## Installation

- Clone the repository
- [Update database](#update-database)
- [Run the application](#run-the-application)
- A generated swagger documentation will appear at [http://localhost:5000/swagger](http://localhost:5000/swagger) or [https://localhost:5001/swagger](https://localhost:5001/swagger). The database will have an admin employee created (username "admin", password "admin"). Authenticate using the provided "Authorize" button and send requests using Swagger or send requests through Postman with a "Basic Auth" header.

## Run the application

```
dotnet run --project WebApi
```

## Update database

```
dotnet ef database update -p Infrastructure -s WebApi
```

## What is different from the Swagger document

- In the request body of PUT, PUST order command we have decided to omit the TenantId and EmployeeId fields, as they are known from auth.
- In the data model (its impossible to do this laboratory work without it), an employee can have only one position but can have many users. Why would you need many user accounts if you work in the same store? Most probably it was a mistake and it should've been a N:M connection between employee and position. But then the User entity did not store any info about Position, so it is unknown to which Position to assign the User to. It's a mishmash, so a decision was made to merge Position and User into Employee, so that for each store a new Employee would be created.
