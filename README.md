# HandiHub 🛠️

**HandiHub** is a web-based platform designed to connect users with skilled artisans and handicraft service providers. Built with ASP.NET Core Web API and SQL Server, this system includes role-based access for Admins, Management, and Customers.

## 🌟 Features

- 🧑‍💼 **Role-Based Access** (Admin, Management, Customer)
- 👨‍🎨 **Artist Management** with bios and contact details
- 🛒 **Product Listings** (handicrafts or services)
- 📦 **Orders and Payments**
- 🔐 **User Authentication & Authorization**
- 📄 **Swagger UI** for API testing

## 🏗️ Tech Stack

| Layer           | Technology                  |
|----------------|-----------------------------|
| Backend        | ASP.NET Core Web API         |
| Database       | SQL Server                   |
| ORM            | Entity Framework Core        |
| API Docs       | Swagger / Swashbuckle        |
| Authentication | JWT (planned or in progress) |


## 📁 Project Structure

handi_hub/
├── Controllers/
├── Data/
├── Dtos/
├── Models/
├── Properties/
├── Repository/
├── Program.cs
├── appsettings.json
└── README.md




## 🛠️ How to Run the Project

1. **Clone the Repository**
   ```bash
   git clone https://github.com/marktmng/handi_hub.git
   cd handi_hub

2. Update Configuration

Open appsettings.json and set your SQL Server connection string.

3. Run the Project

dotnet watch run

4. Access Swagger

Navigate to: https://localhost:{port}/swagger


📌 Future Enhancements
🔐 JWT Authentication and user registration

💻 Frontend (React)

🔎 Advanced filtering and search

💳 Payment gateway integration
