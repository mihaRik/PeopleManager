# PeopleManager - OData API Integration

This repository contains the C# console application that interacts with the public OData API (v4) provided by OData.org. The application demonstrates various best practices for building scalable and maintainable C# applications using modern design patterns and asynchronous programming.

Features
-	**List People**: Displays a list of people retrieved from the OData API.
-	**Search and Filter**: Allows searching and filtering the list of people based on specific criteria.
-	**Show Details**: Displays detailed information about a specific person.
-	**Update Person**: Allows updating the details of a specific person. (Note: The update functionality is implemented but currently limited due to the non-functioning update endpoint in the API.)

Design & Implementation
-	**Repository Design Pattern**: Used to encapsulate data access logic and ensure separation of concerns.
-	**Dependency Injection**: Applied to achieve loose coupling and easier testing of components.
-	**Builder Pattern**: Utilized to create complex objects with multiple optional parameters.
-	**Asynchronous Programming**: Ensures non-blocking I/O operations for better performance.
-	**SOLID Principles**: All code adheres to SOLID principles for better maintainability and extensibility.
-	**Unit Tests**: Key functionalities are covered with unit tests to ensure correctness.

Limitations & Areas for Improvement
-	**API Update Endpoint**: The update endpoint of the provided API is not functioning properly. Regardless of the request body, the same response is always returned. However, the code is built to handle these responses and ensures smooth operation despite this issue.
-	**Logging**: While logging is not fully implemented, it is planned for future improvements.
-	**Error Handling**: Improved error handling for HTTP responses is recommended.
-	**Caching**: Caching of frequent responses to improve performance is another area for potential enhancement.
