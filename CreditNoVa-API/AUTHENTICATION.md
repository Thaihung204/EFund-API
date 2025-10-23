# Authentication System Documentation

## Tổng quan

Hệ thống authentication được xây dựng dựa trên flow của Credit Survey, sử dụng JWT (JSON Web Token) và Refresh Token để bảo mật API.

## Kiến trúc Authentication

### 1. Components chính

- **AuthController**: Xử lý các endpoint authentication
- **AuthService**: Logic xử lý authentication
- **User Model**: Lưu trữ thông tin user và refresh token
- **JWT Middleware**: Xác thực token trong các request

### 2. Flow Authentication

```
1. User Register/Login
   ↓
2. Server tạo JWT Token + Refresh Token
   ↓
3. Client lưu tokens
   ↓
4. Client gửi JWT Token trong header mỗi request
   ↓
5. Server validate JWT Token
   ↓
6. Nếu token hết hạn → dùng Refresh Token để lấy token mới
```

## API Endpoints

### Authentication Endpoints

#### 1. Register User
```
POST /api/auth/register
Content-Type: application/json

{
  "username": "string",
  "email": "string",
  "password": "string",
  "confirmPassword": "string",
  "fullName": "string",
  "phoneNumber": "string",
  "address": "string",
  "gender": "string"
}
```

#### 2. Login
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "string",
  "password": "string"
}
```

#### 3. Get Current User
```
GET /api/auth/me
Authorization: Bearer {jwt_token}
```

#### 4. Refresh Token
```
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "string"
}
```

#### 5. Logout
```
POST /api/auth/logout
Authorization: Bearer {jwt_token}
Content-Type: application/json

{
  "refreshToken": "string"
}
```

### Protected Endpoints (Credit Survey)

Tất cả endpoints trong `/api/survey` đều yêu cầu authentication:

```
GET /api/survey - Lấy danh sách surveys của user
POST /api/survey - Tạo survey mới (tự động liên kết với user hiện tại)
GET /api/survey/{id} - Lấy survey theo ID
PUT /api/survey/{id} - Cập nhật survey
DELETE /api/survey/{id} - Xóa survey
```

## Security Features

### 1. Password Hashing
- Sử dụng BCrypt để hash password
- Salt tự động được tạo

### 2. JWT Token
- Expiry: 1 giờ
- Claims: UserId, Username, Email, Role
- Signed với secret key

### 3. Refresh Token
- Expiry: 7 ngày
- Lưu trong database
- Có thể revoke khi logout

### 4. Authorization
- Tất cả Credit Survey endpoints yêu cầu authentication
- User chỉ có thể truy cập surveys của chính mình

## Database Schema

### User Table
```sql
- Id (Guid, PK)
- Username (string, unique)
- Email (string, unique)
- PasswordHash (string)
- FullName (string)
- PhoneNumber (string, unique)
- Role (string, default: "User")
- IsActive (bool, default: true)
- RefreshToken (string, nullable)
- RefreshTokenExpiry (datetime, nullable)
- CreatedAt, UpdatedAt
```

### CreditSurvey Table
```sql
- Id (Guid, PK)
- UserId (Guid, FK to User)
- ... (các trường survey khác)
```

## Configuration

### JWT Settings (appsettings.json)
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "EFund-API",
    "Audience": "EFund-Client",
    "ExpiryMinutes": 60
  }
}
```

## Usage Examples

### 1. Complete Authentication Flow

```javascript
// 1. Register
const registerResponse = await fetch('/api/auth/register', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'testuser',
    email: 'test@example.com',
    password: 'password123',
    confirmPassword: 'password123',
    fullName: 'Test User',
    phoneNumber: '0123456789'
  })
});

// 2. Login
const loginResponse = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    email: 'test@example.com',
    password: 'password123'
  })
});

const { token, refreshToken } = await loginResponse.json();

// 3. Use protected endpoint
const surveysResponse = await fetch('/api/survey', {
  headers: { 'Authorization': `Bearer ${token}` }
});
```

### 2. Token Refresh

```javascript
// Khi JWT token hết hạn
const refreshResponse = await fetch('/api/auth/refresh', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({ refreshToken })
});

const { token: newToken } = await refreshResponse.json();
```

## Error Handling

### Common Error Responses

```json
// 401 Unauthorized
{
  "message": "Email hoặc mật khẩu không đúng"
}

// 400 Bad Request
{
  "message": "Email hoặc username đã tồn tại"
}

// 401 Unauthorized (Invalid token)
{
  "message": "User not authenticated"
}
```

## Testing

Sử dụng file `EFund-API-Auth.http` để test các endpoints authentication.

## Migration

Sau khi thêm authentication, chạy migration để cập nhật database:

```bash
dotnet ef migrations add AddAuthenticationFields
dotnet ef database update
```
