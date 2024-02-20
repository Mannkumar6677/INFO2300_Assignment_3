# Supplier/Manufacturer Management System (.NET)

## Overview

The Supplier/Manufacturer Management System is a .NET solution designed to integrate supplier, manufacturer, and product management. The system includes:

- **User Management**: Manages user accounts, roles, and audit logs.
- **Product Data**: Handles parts, manufacturers, suppliers, and their interrelationships.
- **Projects**: Manages customer information and job assignments.
- **BOM Management**: Creates and maintains Bills of Materials (BOMs).
- **Procurement**: Generates RFQs, creates Purchase Orders (POs), and processes packing slips.
- **Reporting**: Provides dashboards and detailed reports.

## Table of Contents

- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [License](#license)


## Installation

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0) (or later)
- Visual Studio 2022 or Visual Studio Code

### Steps

1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/supplier-manufacturer-management.git
   cd supplier-manufacturer-management
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build
   ```
4. Run the application:
   ```bash
   dotnet run
   ```

The application will be available at `http://localhost:5000` (or another configured port).

## Usage

Once running, navigate to `http://localhost:5000` in your browser. The system includes:

- **User Management**: Login, profile updates, and role-based access control.
- **Product Data**: Managing parts, manufacturers, and suppliers.
- **Projects**: Registering customers and managing job details.
- **BOM Management**: Creating and managing BOMs.
- **Procurement**: Generating RFQs and POs, and processing packing slips.
- **Reporting**: Viewing dashboards and generating reports.

## Project Structure

```
/SupplierManufacturerManagement
├── /src
│   ├── /Controllers
│   ├── /Models
│   ├── /Views
│   ├── /Services
│   └── Program.cs
├── /Tests
│   └── SupplierManufacturerManagement.Tests.csproj
├── /docs
│   ├── README.md
│   └── Wiki.md
├── SupplierManufacturerManagement.sln
└── LICENSE
```

## License

This project is licensed under the MIT License.

