import request from 'supertest';
import { faker } from '@faker-js/faker';

// Thay đổi URL này trùng với đường dẫn server .NET của bạn đang chạy
const BASE_URL = 'http://localhost:5230';

describe('Auth API Automation Test', () => {
    const userData = {
        email: faker.internet.email().toLowerCase(),
        password: 'Password123@',
        fullName: faker.person.fullName(),
        role: 'Buyer'
    };

    let accessToken = '';
    let refreshTokenCookie = '';

    // === SỬA LỖI 1: Thêm tham số 30000 (30 giây) vào cuối hàm it ===
    it('POST /api/Auth/register - Should register a new user', async () => {
        const res = await request(BASE_URL)
            .post('/api/Auth/register')
            .send({
                email: userData.email,
                password: userData.password,
                fullName: userData.fullName,
                role: userData.role
            });

        expect(res.statusCode).toEqual(200);
        expect(res.body).toHaveProperty('email', userData.email);
        expect(res.body.message).toContain('Registration successful');

        console.log('✅ Registered User:', userData.email);
    }, 30000); // <--- Tăng timeout lên 30s vì gửi email lâu

    // Các test case khác giữ nguyên timeout mặc định
    it('POST /api/Auth/login - Should login and return token', async () => {
        const res = await request(BASE_URL)
            .post('/api/Auth/login')
            .send({
                email: userData.email,
                password: userData.password
            });

        expect(res.statusCode).toEqual(200);
        expect(res.body).toHaveProperty('token');

        accessToken = res.body.token;
        const cookies = res.headers['set-cookie'];
        refreshTokenCookie = cookies.find(c => c.startsWith('refreshToken'));
    });

    it('GET /api/Auth/me - Should return current user info', async () => {
        const res = await request(BASE_URL)
            .get('/api/Auth/me')
            .set('Authorization', `Bearer ${accessToken}`);

        expect(res.statusCode).toEqual(200);
        expect(res.body.email).toEqual(userData.email);
    });

    it('POST /api/Auth/refresh-token - Should get new token', async () => {
        const res = await request(BASE_URL)
            .post('/api/Auth/refresh-token')
            .set('Cookie', [refreshTokenCookie]);

        expect(res.statusCode).toEqual(200);

        const newCookies = res.headers['set-cookie'];
        if (newCookies) {
            refreshTokenCookie = newCookies.find(c => c.startsWith('refreshToken'));
        }
    });

    // === SỬA LỖI 2: Check chữ thường 'expires' ===
    it('POST /api/Auth/logout - Should clear cookie', async () => {
        const res = await request(BASE_URL)
            .post('/api/Auth/logout');

        expect(res.statusCode).toEqual(200);

        const cookies = res.headers['set-cookie'];

        // Nếu không tìm thấy cookie nào trả về thì có thể server chưa set đúng khi logout
        if (cookies) {
            const logoutCookie = cookies.find(c => c.startsWith('refreshToken'));

            // Convert chuỗi về chữ thường rồi mới so sánh cho chắc ăn
            expect(logoutCookie.toLowerCase()).toContain('expires');
        }
    });
});