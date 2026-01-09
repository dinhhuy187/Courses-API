# EduMart - Online Course Marketplace API 🎓

![.NET Core](https://img.shields.io/badge/.NET%20Core-8.0-purple?style=flat&logo=dotnet)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-blue?style=flat&logo=postgresql)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-cyan?style=flat)
![Status](https://img.shields.io/badge/Status-Development-green)

## 📖 Introduction
**EduMart** is a comprehensive backend API service for an Online Course Marketplace. It serves as the intermediary connecting Instructors (sellers) and Students (buyers). The system facilitates course creation, video streaming (via drive links), real-time interactions, and secure online payments.

This repository contains the **Backend Server** code, built with **ASP.NET Core Web API**, following **Clean Architecture** principles and utilizing **Code-First** migration with PostgreSQL.

## 🚀 Tech Stack

### Core Framework
* [cite_start]**Framework:** ASP.NET Core Web API (.NET 8.0) [cite: 45]
* **Language:** C#
* [cite_start]**Database:** PostgreSQL [cite: 54]
* [cite_start]**ORM:** Entity Framework Core (Code-First Approach) [cite: 242]

### Key Libraries & Integrations
* [cite_start]**Authentication:** JWT (JSON Web Tokens) & Refresh Tokens [cite: 242]
* **Security:** BCrypt for Password Hashing.
* [cite_start]**Real-time Communication:** SignalR (Chat & Notifications) [cite: 245]
* [cite_start]**Payment Gateway:** MoMo API Integration (IPN handling) [cite: 47]
* [cite_start]**Media Management:** Cloudinary (Image storage & optimization) [cite: 50]
* **Documentation:** Swagger/OpenAPI.

## ✨ Key Features

### 👤 User (Student)
* **Authentication:** Secure Login/Register, Forgot Password via Email.
* [cite_start]**Course Discovery:** Advanced search, filter by category/price, and view course details[cite: 113].
* **Enrollment:** Add to cart, purchase courses via MoMo, and view learning history.
* **Interaction:** Rate/Review courses, Real-time chat with instructors.

### 👨‍🏫 Instructor
* **Course Management:** Create and update courses, upload content (integration with Cloudinary/Drive links).
* [cite_start]**Dashboard:** View revenue statistics, track student enrollments, and analyze monthly growth[cite: 151].
* **Communication:** Receive real-time notifications and messages from students.

### 🛡️ Admin
* **Content Moderation:** Approve/Reject published courses.
* **User Management:** Manage user roles, lock violating accounts.
* [cite_start]**System Stats:** Overview of total revenue, users, and transactions[cite: 187].

## 🛠️ Installation & Setup

### Prerequisites
* [.NET SDK 8.0](https://dotnet.microsoft.com/download)
* [PostgreSQL](https://www.postgresql.org/download/)
* [Visual Studio 2022](https://visualstudio.microsoft.com/) or VS Code.

### Steps
1.  **Clone the repository**
    ```bash
    git clone [https://github.com/your-username/EduMart-Backend.git](https://github.com/your-username/EduMart-Backend.git)
    cd EduMart-Backend
    ```

2.  **Configure Environment Variables**
    Create a `.env` file in the root directory of the project. This file will store sensitive credentials and configurations.
    
    Copy the following content into your `.env` file and update the values:

    ```properties
    # ===========================
    # 🔐 JWT Configuration
    # ===========================
    JWT_KEY=your-super-secret-key-change-this
    JWT_ISSUER=courses_buynsell_api
    JWT_AUDIENCE=courses_buynsell_client
    JWT_EXPIRY_MINUTES=60

    # ===========================
    # 🗄️ Database Configuration
    # ===========================
    DB_CONNECTION="Host=localhost;Port=5432;Database=EduMartDb;Username=postgres;Password=yourpassword"

    # ===========================
    # 💳 MoMo Payment (Sandbox)
    # ===========================
    MOMO_PARTNER_CODE=MOMO
    MOMO_ACCESS_KEY=your-momo-access-key
    MOMO_SECRET_KEY=your-momo-secret-key
    MOMO_API_URL=[https://test-payment.momo.vn/v2/gateway/api/create](https://test-payment.momo.vn/v2/gateway/api/create)
    MOMO_RETURN_URL=https://localhost:5001/api/payment/return
    MOMO_NOTIFY_URL=https://localhost:5001/api/payment/notify
    MOMO_REQUEST_TYPE=captureWallet

    # ===========================
    # ☁️ Cloudinary (Media)
    # ===========================
    CLOUDINARY_CLOUD_NAME=your-cloud-name
    CLOUDINARY_API_KEY=your-api-key
    CLOUDINARY_API_SECRET=your-api-secret

    # ===========================
    # 📧 Email Service (SMTP)
    # ===========================
    SMTP_SERVER=smtp.gmail.com
    SMTP_PORT=587
    SENDER_EMAIL=your-email@gmail.com
    SENDER_PASSWORD=your-app-password
    SENDER_NAME="EduMart Support"
    ```

    > **Note:** Ensure that `.env` is added to your `.gitignore` file to prevent sensitive keys from being pushed to GitHub.

3.  **Database Migration**
    Apply Code-First migrations to generate the database schema:
    ```bash
    dotnet ef database update
    ```

4.  **Run the Application**
    ```bash
    dotnet run
    ```
    The API will be available at `https://localhost:5001` (or your configured port).
    Access **Swagger UI** at `https://localhost:5001/swagger/index.html` to test endpoints.

## 📂 Project Structure
```text
EduMart.Backend/
├── Controllers/       # API Endpoints
├── Data/              # DbContext & Migrations
├── Models/            # Domain Entities (EF Core)
├── DTOs/              # Data Transfer Objects
├── Services/          # Business Logic Layer
├── Hubs/              # SignalR Hubs (Real-time)
├── Interfaces/        # Repository Interfaces
└── Program.cs         # App Configuration & DI
