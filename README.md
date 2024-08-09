# Task_Manager_App

The Task Manager App is an efficient tool designed to streamline project and task management. Built using ASP.NET Core and .NET 6, this application follows the principles of Clean Architecture to ensure maintainability and scalability.


## Features

- **Project Section**: Users can create and manage multiple projects and add other users to their own projects.
- **Task Section**: Within each project, users can create, assign, and manage tasks.
- **Task Operation Request Section**(integrated within the same API endpoints as the Task section): Users can request task operations (e.g., assign, unassign, and task completion requests). 
- **User Section**: Includes authentication and user management (Identity, JWT, refresh tokens, etc.).
- **Email Notifications**: Uses MailKit for sending emails for resetting user passwords.
- **Input Validation**: Model validation for user inputs to ensure data integrity.
- **Design Patterns Used**: Dependency Injection, Unit of Work, Repository, and Service Layer.
- **Data Access**: Utilizes Entity Framework Core for database interactions.
- **RESTful API**: Implements a RESTful API architecture.
- **API Documentation**: Integrates Swagger for API testing and documentation.
- **Clean Architecture**: The project follows Clean Architecture principles, with separate layers for API, Application, Domain, and Infrastructure.
- **Exception Handling**: Defines custom exception classes (e.g., ServiceException, RepositoryException) to handle exceptions in different layers effectively.
- **etc.**


## Technologies

- **ASP.NET Core**
- **Dotnet6**
- **SQL Server**

## Demo

[Project Demo Video](https://raw.githubusercontent.com/karim1378/Task_Manager_App/main/Demo/demo.mp4)
