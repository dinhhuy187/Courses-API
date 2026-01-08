# Website mua bán khóa học trực tuyến

Hướng dẫn cài đặt và chạy dự án (.NET Core Backend + React Frontend).

## Yêu cầu hệ thống
* .NET SDK (Core)
* Node.js & npm
* PostgreSQL

---

## 1. Backend (.NET Core)

**Cấu hình:**
Mở file `appsettings.json` và cập nhật chuỗi kết nối (Connection String) tới **PostgreSQL** của bạn.

**Lệnh chạy:**

```bash
# Di chuyển vào thư mục Backend
cd courses-buynsell-api

# Khôi phục các gói thư viện
dotnet restore

# Chạy migration để khởi tạo Database (BẮT BUỘC)
dotnet ef database update

# Khởi chạy server
dotnet run
```

## 2. Frontend (React)
**Lệnh chạy:**
# Di chuyển vào thư mục Frontend
cd frontend-folder

# Cài đặt node modules
npm install

# Khởi chạy dự án
npm start
# Hoặc dùng lệnh dưới nếu dùng Vite:
npm run dev