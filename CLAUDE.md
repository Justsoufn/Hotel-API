# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Hotel API - built with ASP.NET Core Web API (.NET).

## Build and Development Commands

```bash
# All commands run from HotelApi/ directory

# Restore dependencies
dotnet restore

# Run development server (http://localhost:5000)
dotnet run

# Build
dotnet build

# Build for production
dotnet publish -c Release
```

## Architecture

This project follows standard ASP.NET Core Web API conventions:

- **Controllers/** - API endpoints organized by resource (e.g., RoomsController, ReservationsController)
- **Models/** - Domain entities and DTOs
- **Services/** - Business logic layer
- **Data/** - Database context and repository implementations
- **Program.cs** - Application entry point and service configuration

## API Conventions

- RESTful endpoints following `/api/[controller]` routing
- Use DTOs for request/response payloads, not domain entities directly
- Implement proper HTTP status codes (200, 201, 400, 404, etc.)
- Use async/await for all I/O operations

## Key Technologies

- ASP.NET
- Entity Framework Core - using in-memory database