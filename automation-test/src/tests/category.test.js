// src/tests/category.test.js
import request from 'supertest';
import { faker } from '@faker-js/faker';
import { DEFAULT_TIMEOUT, BASE_URL } from '../config';
import { createAndLoginUser } from '../utils/authHelper'; // <--- Import hàng về

describe('Category API (Protected)', () => {
    let adminToken = '';
    let studentToken = '';
    let createdCategoryId = 0;

    // Dữ liệu category test
    const newCategory = {
        name: `Cat_${faker.commerce.department()}_${Date.now()}`
    };

    // === SETUP GỌN GÀNG ===
    beforeAll(async () => {
        try {
            console.log('🚀 Starting setup...'); // Log để xem nó có chạy vào đây không

            // 1. Login Admin
            const adminSession = await createAndLoginUser('Admin');
            adminToken = adminSession.token;
            console.log('✅ Admin Token:', adminToken ? 'Got it' : 'MISSING!');

            // 2. Login Student
            const studentSession = await createAndLoginUser('Buyer');
            studentToken = studentSession.token;
            console.log('✅ Student Token:', studentToken ? 'Got it' : 'MISSING!');

        } catch (error) {
            // IN RA LỖI CỤ THỂ
            console.error('❌ SETUP FAILED:', error);
            // Nếu lỗi do API trả về, in thêm chi tiết
            if (error.response) {
                console.error('Response Body:', error.response.body);
            }
        }
    }, DEFAULT_TIMEOUT);


    // === TEST CASES (Code test chính không đổi) ===

    it('POST /Category - Admin should be able to create category', async () => {
        const res = await request(BASE_URL)
            .post('/Category')
            .set('Authorization', `Bearer ${adminToken}`) // Dùng token đã lấy
            .send(newCategory);

        expect(res.statusCode).toEqual(201);
    });

    it('GET /Category - Admin should see the new category', async () => {
        const res = await request(BASE_URL)
            .get('/Category')
            .set('Authorization', `Bearer ${adminToken}`);

        expect(res.statusCode).toEqual(200);

        // --- SỬA ĐOẠN NÀY ---
        // Tìm category vừa tạo
        const foundCat = res.body.find(c => c.name === newCategory.name);

        // Kiểm tra xem có tìm thấy không
        if (!foundCat) {
            console.error('⚠️ Warning: Cannot find the created category. Did the POST test fail?');
            // Cho test fail có kiểm soát thay vì crash
            expect(foundCat).toBeDefined();
        } else {
            createdCategoryId = foundCat.id;
            console.log('Found created category ID:', createdCategoryId);
        }
    });

    // ... Các test update/delete/forbidden giữ nguyên ...

    it('POST /Category - Student should NOT be able to create', async () => {
        const res = await request(BASE_URL)
            .post('/Category')
            .set('Authorization', `Bearer ${studentToken}`)
            .send({ name: 'Hacker', description: 'Test' });

        expect(res.statusCode).toEqual(403);
    });
});