-- =====================================================
-- INSERT USERS (Người dùng)
-- =====================================================
INSERT INTO public."Users" 
("FullName", "Email", "PasswordHash", "Role", "CreatedAt", "PhoneNumber", "IsEmailVerified", "RefreshToken", "RefreshTokenExpiryTime", "AvatarUrl")
VALUES
-- Admin mặc định
('Nguyễn Văn Admin', 'admin@coursehub.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Admin', NOW() - INTERVAL '365 days', '0901234567', true, NULL, NULL,
 'https://cdn.tgdd.vn/Products/Images/44/335362/macbook-air-13-inch-m4-xanh-da-troi-600x600.jpg'),

-- Admin mới thêm
('Sang', 'sang@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Admin', NOW() - INTERVAL '300 days', '0900000000', true, NULL, NULL,
 'https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/44/327098/hp-15-fc0085au-r5-a6vv8pa-170225-110652-878-600x600.jpg'),

-- Sellers
('Trần Thị Hoa', 'hoa.tran@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Seller', NOW() - INTERVAL '200 days', '0912345678', true, NULL, NULL,
 'https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/44/358086/macbook-pro-14-inch-m5-16gb-512gb-thumb-638962954605863722-600x600.jpg'),

('Lê Văn Minh', 'minh.le@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Seller', NOW() - INTERVAL '180 days', '0923456789', false, NULL, NULL,
 'https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/s/a/samsung-galaxy-z-flip-7-1.jpg'),

('Phạm Thị Lan', 'lan.pham@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Seller', NOW() - INTERVAL '150 days', '0934567890', true, NULL, NULL,
 'https://cdn.tgdd.vn/Products/Images/44/335362/macbook-air-13-inch-m4-xanh-da-troi-600x600.jpg'),

('Hoàng Văn Nam', 'nam.hoang@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Seller', NOW() - INTERVAL '120 days', '0945678901', true, NULL, NULL,
 'https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/44/327098/hp-15-fc0085au-r5-a6vv8pa-170225-110652-878-600x600.jpg'),

-- Buyers
('Đỗ Văn Hùng', 'hung.do@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Buyer', NOW() - INTERVAL '100 days', '0956789012', true, NULL, NULL,
 'https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/44/358086/macbook-pro-14-inch-m5-16gb-512gb-thumb-638962954605863722-600x600.jpg'),

('Vũ Thị Mai', 'mai.vu@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Buyer', NOW() - INTERVAL '90 days', '0967890123', false, NULL, NULL,
 'https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/s/a/samsung-galaxy-z-flip-7-1.jpg'),

('Bùi Văn Tuấn', 'tuan.bui@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Buyer', NOW() - INTERVAL '80 days', '0978901234', true, NULL, NULL,
 'https://cdn.tgdd.vn/Products/Images/44/335362/macbook-air-13-inch-m4-xanh-da-troi-600x600.jpg'),

('Đinh Thị Hương', 'huong.dinh@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Buyer', NOW() - INTERVAL '70 days', '0989012345', true, NULL, NULL,
 'https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/44/327098/hp-15-fc0085au-r5-a6vv8pa-170225-110652-878-600x600.jpg'),

('Ngô Văn Long', 'long.ngo@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Buyer', NOW() - INTERVAL '60 days', '0990123456', false, NULL, NULL,
 'https://cdnv2.tgdd.vn/mwg-static/tgdd/Products/Images/44/358086/macbook-pro-14-inch-m5-16gb-512gb-thumb-638962954605863722-600x600.jpg'),

('Trương Thị Thu', 'thu.truong@gmail.com', 'Y6IE4cbribo1c2b2aq4IJg==.VJbIsl4pg2TOL3ZL4Xf2Pneez/s756/dq6ej15kjDPc=', 'Buyer', NOW() - INTERVAL '50 days', '0901234568', true, NULL, NULL,
 'https://cdn2.cellphones.com.vn/insecure/rs:fill:358:358/q:90/plain/https://cellphones.com.vn/media/catalog/product/s/a/samsung-galaxy-z-flip-7-1.jpg');


-- =====================================================
-- INSERT CATEGORIES (Danh mục khóa học)
-- =====================================================
INSERT INTO public."Categories" ("Name", "CreatedAt")
VALUES
('Lập trình Web', NOW() - INTERVAL '400 days'),
('Lập trình Mobile', NOW() - INTERVAL '390 days'),
('Data Science', NOW() - INTERVAL '380 days'),
('Thiết kế đồ họa', NOW() - INTERVAL '370 days'),
('Marketing Digital', NOW() - INTERVAL '360 days'),
('Kinh doanh', NOW() - INTERVAL '350 days'),
('Ngoại ngữ', NOW() - INTERVAL '340 days'),
('Phát triển cá nhân', NOW() - INTERVAL '330 days');

-- =====================================================
-- INSERT COURSES (Khóa học)
-- =====================================================
INSERT INTO public."Courses" 
("Title", "Description", "Price", "Level", "TeacherName", "ImageUrl", "DurationHours", "TotalPurchased", "AverageRating", "CreatedAt", "UpdatedAt", "SellerId", "CategoryId", "IsApproved")
VALUES
-- Khóa học đã được duyệt
('Lập trình React từ cơ bản đến nâng cao', 'Khóa học giúp bạn nắm vững React, từ JSX, Components đến Hooks và Redux. Phù hợp cho người mới bắt đầu.', 1500000, 'Beginner', 'Trần Thị Hoa', 'https://images.unsplash.com/photo-1633356122544-f134324a6cee', 40, 150, 4.8, NOW() - INTERVAL '180 days', NOW() - INTERVAL '10 days', 2, 1, true),

('Python cho Data Science', 'Học Python từ đầu, pandas, numpy, matplotlib và các thuật toán Machine Learning cơ bản.', 2000000, 'Intermediate', 'Phạm Thị Lan', 'https://images.unsplash.com/photo-1526379095098-d400fd0bf935', 50, 200, 4.9, NOW() - INTERVAL '150 days', NOW() - INTERVAL '5 days', 4, 3, true),

('Flutter - Xây dựng ứng dụng di động đa nền tảng', 'Khóa học Flutter giúp bạn tạo ứng dụng iOS và Android từ một mã nguồn duy nhất.', 1800000, 'Beginner', 'Hoàng Văn Nam', 'https://images.unsplash.com/photo-1512941937669-90a1b58e7e9c', 45, 120, 4.7, NOW() - INTERVAL '120 days', NOW() - INTERVAL '8 days', 5, 2, true),

('Adobe Photoshop cho người mới bắt đầu', 'Khóa học Photoshop từ A-Z, thiết kế banner, poster, chỉnh sửa ảnh chuyên nghiệp.', 1200000, 'Beginner', 'Trần Thị Hoa', 'https://images.unsplash.com/photo-1626785774573-4b799315345d', 30, 180, 4.6, NOW() - INTERVAL '100 days', NOW() - INTERVAL '12 days', 2, 4, true),

('Marketing trên Facebook & Instagram', 'Chiến lược marketing hiệu quả trên mạng xã hội, chạy quảng cáo Facebook Ads và Instagram Ads.', 1000000, 'Beginner', 'Lê Văn Minh', 'https://images.unsplash.com/photo-1611162617474-5b21e879e113', 25, 250, 4.5, NOW() - INTERVAL '90 days', NOW() - INTERVAL '15 days', 3, 5, true),

-- Khóa học chưa được duyệt
('Node.js và Express - Backend Development', 'Xây dựng RESTful API với Node.js, Express, MongoDB. Tích hợp JWT authentication.', 1700000, 'Intermediate', 'Lê Văn Minh', 'https://images.unsplash.com/photo-1627398242454-45a1465c2479', 38, 0, 0, NOW() - INTERVAL '30 days', NOW() - INTERVAL '30 days', 3, 1, false),

('Tiếng Anh giao tiếp cơ bản', 'Học tiếng Anh giao tiếp hàng ngày, phát âm chuẩn, từ vựng thực tế.', 800000, 'Beginner', 'Phạm Thị Lan', 'https://images.unsplash.com/photo-1546410531-bb4caa6b424d', 35, 0, 0, NOW() - INTERVAL '20 days', NOW() - INTERVAL '20 days', 4, 7, false),

('Quản trị doanh nghiệp hiện đại', 'Các kỹ năng quản lý, lãnh đạo, xây dựng đội nhóm và phát triển doanh nghiệp bền vững.', 2500000, 'Advanced', 'Hoàng Văn Nam', 'https://images.unsplash.com/photo-1454165804606-c3d57bc86b40', 60, 0, 0, NOW() - INTERVAL '15 days', NOW() - INTERVAL '15 days', 5, 6, false);

-- =====================================================
-- INSERT COURSE CONTENTS (Nội dung khóa học)
-- =====================================================
INSERT INTO public."CourseContents" ("Title", "Description", "CourseId")
VALUES
-- Course 1: React
('Giới thiệu về React', 'Tổng quan về React, JSX và Virtual DOM', 1),
('Components và Props', 'Cách tạo và sử dụng components, truyền props', 1),
('State và Lifecycle', 'Quản lý state, lifecycle methods trong React', 1),
('React Hooks', 'useState, useEffect, useContext và custom hooks', 1),
('Redux cơ bản', 'State management với Redux', 1),

-- Course 2: Python Data Science
('Python cơ bản', 'Cú pháp Python, biến, vòng lặp, hàm', 2),
('Pandas và NumPy', 'Xử lý dữ liệu với Pandas và NumPy', 2),
('Visualization với Matplotlib', 'Vẽ biểu đồ và trực quan hóa dữ liệu', 2),
('Machine Learning cơ bản', 'Thuật toán Linear Regression, Decision Tree', 2),

-- Course 3: Flutter
('Dart programming', 'Ngôn ngữ Dart cơ bản cho Flutter', 3),
('Flutter Widgets', 'StatelessWidget, StatefulWidget, Material Design', 3),
('Navigation và Routing', 'Điều hướng giữa các màn hình', 3),
('State Management', 'Provider, Bloc pattern trong Flutter', 3),

-- Course 4: Photoshop
('Giao diện Photoshop', 'Làm quen với workspace và tools', 4),
('Layers và Masks', 'Làm việc với layers, layer masks', 4),
('Retouching ảnh', 'Chỉnh sửa và làm đẹp ảnh', 4),

-- Course 5: Marketing
('Facebook Marketing Overview', 'Giới thiệu về Facebook Marketing', 5),
('Tạo Facebook Ads', 'Cách tạo và tối ưu quảng cáo Facebook', 5),
('Instagram Marketing', 'Chiến lược marketing trên Instagram', 5);

-- =====================================================
-- INSERT COURSE SKILLS (Kỹ năng học được)
-- =====================================================
INSERT INTO public."CourseSkills" ("Name", "CourseId")
VALUES
-- Course 1
('React.js', 1), ('JavaScript ES6+', 1), ('Redux', 1), ('React Hooks', 1),
-- Course 2
('Python', 2), ('Pandas', 2), ('Machine Learning', 2), ('Data Visualization', 2),
-- Course 3
('Flutter', 3), ('Dart', 3), ('Mobile Development', 3), ('UI/UX Design', 3),
-- Course 4
('Photoshop', 4), ('Graphic Design', 4), ('Photo Editing', 4),
-- Course 5
('Facebook Ads', 5), ('Social Media Marketing', 5), ('Content Marketing', 5);

-- =====================================================
-- INSERT TARGET LEARNERS (Đối tượng học viên)
-- =====================================================
INSERT INTO public."TargetLearners" ("Description", "CourseId")
VALUES
-- Course 1
('Người mới bắt đầu học lập trình web', 1),
('Lập trình viên muốn học React', 1),
('Sinh viên công nghệ thông tin', 1),

-- Course 2
('Người quan tâm đến Data Science', 2),
('Lập trình viên muốn chuyển sang AI/ML', 2),
('Nhà phân tích dữ liệu', 2),

-- Course 3
('Lập trình viên muốn làm mobile app', 3),
('Người mới bắt đầu với Flutter', 3),

-- Course 4
('Người yêu thích thiết kế đồ họa', 4),
('Nhân viên marketing cần thiết kế', 4),

-- Course 5
('Chủ doanh nghiệp nhỏ', 5),
('Nhân viên marketing', 5),
('Freelancer cần học marketing online', 5);

-- =====================================================
-- INSERT ENROLLMENTS (Đăng ký khóa học)
-- =====================================================
INSERT INTO public."Enrollments" ("EnrollAt", "BuyerId", "CourseId")
VALUES
-- User 6 (Đỗ Văn Hùng)
(NOW() - INTERVAL '95 days', 6, 1),
(NOW() - INTERVAL '90 days', 6, 2),

-- User 7 (Vũ Thị Mai)
(NOW() - INTERVAL '85 days', 7, 1),
(NOW() - INTERVAL '80 days', 7, 4),

-- User 8 (Bùi Văn Tuấn)
(NOW() - INTERVAL '75 days', 8, 2),
(NOW() - INTERVAL '70 days', 8, 3),
(NOW() - INTERVAL '65 days', 8, 5),

-- User 9 (Đinh Thị Hương)
(NOW() - INTERVAL '60 days', 9, 1),
(NOW() - INTERVAL '55 days', 9, 5),

-- User 10 (Ngô Văn Long)
(NOW() - INTERVAL '50 days', 10, 4),

-- User 11 (Trương Thị Thu)
(NOW() - INTERVAL '45 days', 11, 2),
(NOW() - INTERVAL '40 days', 11, 3);

-- =====================================================
-- INSERT REVIEWS (Đánh giá khóa học)
-- =====================================================
INSERT INTO public."Reviews" ("Star", "CreatedAt", "Comment", "BuyerId", "CourseId")
VALUES
-- Course 1
(5, NOW() - INTERVAL '90 days', 'Khóa học rất hay, giảng viên nhiệt tình. Tôi đã học được rất nhiều về React!', 6, 1),
(5, NOW() - INTERVAL '82 days', 'Nội dung dễ hiểu, ví dụ thực tế. Rất đáng để học!', 7, 1),
(4, NOW() - INTERVAL '58 days', 'Khóa học tốt nhưng cần thêm nhiều bài tập thực hành hơn.', 9, 1),

-- Course 2
(5, NOW() - INTERVAL '88 days', 'Khóa học Python tuyệt vời! Từ cơ bản đến nâng cao đều rất chi tiết.', 6, 2),
(5, NOW() - INTERVAL '72 days', 'Giảng viên giỏi, kiến thức chắc chắn. Rất hài lòng!', 8, 2),
(5, NOW() - INTERVAL '43 days', 'Best course về Data Science mà tôi từng học.', 11, 2),

-- Course 3
(5, NOW() - INTERVAL '68 days', 'Flutter rất thú vị, học xong có thể làm app luôn.', 8, 3),
(4, NOW() - INTERVAL '38 days', 'Khóa học hay nhưng hơi nhanh một chút.', 11, 3),

-- Course 4
(4, NOW() - INTERVAL '78 days', 'Học Photoshop hiệu quả, giờ tôi có thể tự thiết kế được rồi.', 7, 4),
(5, NOW() - INTERVAL '48 days', 'Giảng viên dạy rất dễ hiểu, không bị khó như tôi nghĩ.', 10, 4),

-- Course 5
(4, NOW() - INTERVAL '63 days', 'Khóa học marketing thực chiến, áp dụng được ngay.', 8, 5),
(5, NOW() - INTERVAL '53 days', 'Rất hữu ích cho công việc của tôi. Recommend!', 9, 5);

-- =====================================================
-- INSERT FAVORITES (Yêu thích khóa học)
-- =====================================================
INSERT INTO public."Favorites" ("UserId", "CourseId")
VALUES
(6, 3), (6, 4), (6, 5),
(7, 2), (7, 3), (7, 5),
(8, 1), (8, 4),
(9, 2), (9, 3), (9, 4),
(10, 1), (10, 2), (10, 3),
(11, 1), (11, 4), (11, 5);

-- =====================================================
-- INSERT CARTS (Giỏ hàng)
-- =====================================================
INSERT INTO public."Carts" ("UserId", "CreatedAt")
VALUES
(6, NOW() - INTERVAL '5 days'),
(7, NOW() - INTERVAL '3 days'),
(10, NOW() - INTERVAL '2 days');

-- =====================================================
-- INSERT CART ITEMS (Sản phẩm trong giỏ hàng)
-- =====================================================
INSERT INTO public."CartItems" ("CartId", "CourseId")
VALUES
-- Cart của User 6
(1, 3),
(1, 5),

-- Cart của User 7
(2, 2),
(2, 3),

-- Cart của User 10
(3, 1),
(3, 2),
(3, 5);

-- =====================================================
-- INSERT TRANSACTIONS (Giao dịch)
-- =====================================================
INSERT INTO public."Transactions" 
("TransactionCode", "PaymentMethod", "TotalAmount", "CreatedAt", "UpdatedAt", "BuyerId")
VALUES
('TXN001-2024-001', 'VNPay', 3500000, NOW() - INTERVAL '95 days', NOW() - INTERVAL '95 days', 6),
('TXN001-2024-002', 'MoMo', 1500000, NOW() - INTERVAL '85 days', NOW() - INTERVAL '85 days', 7),
('TXN001-2024-003', 'Banking', 1200000, NOW() - INTERVAL '80 days', NOW() - INTERVAL '80 days', 7),
('TXN001-2024-004', 'VNPay', 5500000, NOW() - INTERVAL '75 days', NOW() - INTERVAL '75 days', 8),
('TXN001-2024-005', 'MoMo', 2500000, NOW() - INTERVAL '60 days', NOW() - INTERVAL '60 days', 9),
('TXN001-2024-006', 'Banking', 1200000, NOW() - INTERVAL '50 days', NOW() - INTERVAL '50 days', 10),
('TXN001-2024-007', 'VNPay', 3800000, NOW() - INTERVAL '45 days', NOW() - INTERVAL '45 days', 11);

-- =====================================================
-- INSERT TRANSACTION DETAILS (Chi tiết giao dịch)
-- =====================================================
INSERT INTO public."TransactionDetails" ("Price", "TransactionId", "CourseId")
VALUES
-- Transaction 1 (User 6 mua 2 khóa học)
(1500000, 1, 1),
(2000000, 1, 2),

-- Transaction 2 (User 7 mua 1 khóa học)
(1500000, 2, 1),

-- Transaction 3 (User 7 mua 1 khóa học)
(1200000, 3, 4),

-- Transaction 4 (User 8 mua 3 khóa học)
(2000000, 4, 2),
(1800000, 4, 3),
(1700000, 4, 5),

-- Transaction 5 (User 9 mua 2 khóa học)
(1500000, 5, 1),
(1000000, 5, 5),

-- Transaction 6 (User 10 mua 1 khóa học)
(1200000, 6, 4),

-- Transaction 7 (User 11 mua 2 khóa học)
(2000000, 7, 2),
(1800000, 7, 3);

-- =====================================================
-- INSERT NOTIFICATIONS (Thông báo cho Seller)
-- =====================================================
INSERT INTO public."Notifications" 
("Message", "CreatedAt", "IsRead", "SellerId")
VALUES
-- Notifications for Seller 2 (Trần Thị Hoa)
('Bạn có đơn hàng mới từ khóa học "Lập trình React từ cơ bản đến nâng cao"', NOW() - INTERVAL '95 days', true, 2),
('Học viên Vũ Thị Mai đã đăng ký khóa học của bạn', NOW() - INTERVAL '85 days', true, 2),
('Bạn nhận được đánh giá 5 sao cho khóa học React', NOW() - INTERVAL '90 days', true, 2),
('Khóa học "Adobe Photoshop cho người mới bắt đầu" đã được duyệt', NOW() - INTERVAL '100 days', true, 2),

-- Notifications for Seller 3 (Lê Văn Minh)
('Khóa học "Marketing trên Facebook & Instagram" đã được duyệt', NOW() - INTERVAL '90 days', true, 3),
('Bạn có khóa học đang chờ duyệt: Node.js và Express', NOW() - INTERVAL '30 days', false, 3),

-- Notifications for Seller 4 (Phạm Thị Lan)
('Khóa học "Python cho Data Science" được 200 học viên đăng ký', NOW() - INTERVAL '50 days', true, 4),
('Bạn nhận được đánh giá 5 sao từ học viên', NOW() - INTERVAL '88 days', true, 4),
('Khóa học "Tiếng Anh giao tiếp cơ bản" đang chờ duyệt', NOW() - INTERVAL '20 days', false, 4),

-- Notifications for Seller 5 (Hoàng Văn Nam)
('Khóa học Flutter đã có 120 học viên đăng ký', NOW() - INTERVAL '60 days', true, 5),
('Bạn có khóa học mới đang chờ duyệt', NOW() - INTERVAL '15 days', false, 5);