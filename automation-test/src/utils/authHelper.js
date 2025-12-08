import request from 'supertest';
import { faker } from '@faker-js/faker';
import { BASE_URL } from '../config';

/**
 * Hàm tạo user và login lấy token
 * @param {string} role - 'Admin', 'Seller', 'Student'
 */
export const createAndLoginUser = async (role = 'Buyer') => {
    let userData;

    try {
        // === TRƯỜNG HỢP 1: ADMIN (Dùng tài khoản cứng có sẵn) ===
        if (role === 'Admin') {
            console.log(`...Logging in EXISTING ADMIN: sang@gmail.com`);

            // Dữ liệu cứng bạn cung cấp
            userData = {
                email: 'sang@gmail.com',
                password: 'Test123456',
                fullName: 'Super Admin',
                role: 'Admin'
            };

            // Gọi thẳng API Login, không cần Register
            const loginRes = await request(BASE_URL)
                .post('/api/Auth/login')
                .send({
                    email: userData.email,
                    password: userData.password
                });

            if (loginRes.statusCode !== 200) {
                throw new Error(`Login Admin Failed: ${JSON.stringify(loginRes.body)}`);
            }

            return {
                token: loginRes.body.token,
                user: userData,
                userId: loginRes.body.id
            };
        }

        // === TRƯỜNG HỢP 2: CÁC ROLE KHÁC (Tạo mới ngẫu nhiên) ===
        // Logic: Tạo user fake -> Register -> Login

        userData = {
            email: faker.internet.email().toLowerCase(),
            password: 'Password123@',
            fullName: `Test ${role} ${Date.now()}`,
            role: role
        };

        console.log(`...Registering New ${role}: ${userData.email}`);

        // 1. Register
        const regRes = await request(BASE_URL)
            .post('/api/Auth/register')
            .send(userData);

        if (regRes.statusCode !== 200) {
            throw new Error(`Register Failed: ${JSON.stringify(regRes.body)}`);
        }

        // 2. Login
        const loginRes = await request(BASE_URL)
            .post('/api/Auth/login')
            .send({
                email: userData.email,
                password: userData.password
            });

        if (loginRes.statusCode !== 200) {
            throw new Error(`Login Failed: ${JSON.stringify(loginRes.body)}`);
        }

        return {
            token: loginRes.body.token,
            user: userData,
            userId: loginRes.body.id
        };

    } catch (error) {
        console.error(`❌ Error setup user ${role}:`);
        if (error.response) {
            console.error('Response:', error.response.body);
        }
        throw error;
    }
};