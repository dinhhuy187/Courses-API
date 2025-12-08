module.exports = {
    testEnvironment: 'node',
    testTimeout: 10000,
    verbose: true,

    // Dùng babel để xử lý file js
    transform: {
        '^.+\\.js$': 'babel-jest',
    },

    // Quan trọng: Bảo Jest "hãy dịch cả code trong node_modules của faker"
    transformIgnorePatterns: [
        "node_modules/(?!@faker-js/faker)"
    ],
};