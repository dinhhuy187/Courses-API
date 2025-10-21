# Giai đoạn 1: Build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy file dự án
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ mã nguồn và build
COPY . ./
RUN dotnet publish -c Release -o /app/out

# Giai đoạn 2: Runtime (chạy ứng dụng)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy kết quả build từ giai đoạn trước
COPY --from=build /app/out ./

# Copy file .env nếu có (tùy chọn)
COPY .env .env

# Mở port (tùy theo app bạn đang dùng, mặc định ASP.NET là 8080 hoặc 5000)
EXPOSE 8080

# Lệnh khởi chạy
ENTRYPOINT ["dotnet", "courses_buynsell_api.dll"]
