# 🚗 Smart Parking Management System

## Giới thiệu

**Smart Parking Management System** là hệ thống quản lý và đặt chỗ bãi đỗ xe được xây dựng nhằm hỗ trợ khách hàng tìm kiếm bãi đỗ xe phù hợp, đặt chỗ trước và theo dõi thông tin đặt chỗ một cách thuận tiện.

Bên cạnh đó, hệ thống còn hỗ trợ nhân viên và chủ bãi xe trong việc quản lý phương tiện ra/vào, vị trí đỗ xe, đơn đặt chỗ, thông tin khách hàng và hoạt động vận hành của bãi đỗ xe.

Dự án được phát triển theo mô hình **Web Application kết hợp RESTful API**, sử dụng **ASP.NET Core**, **C#** và **SQL Server**.

---

## Mục tiêu của hệ thống

Hệ thống được xây dựng nhằm:

* Giúp khách hàng dễ dàng tìm kiếm và đặt trước chỗ đỗ xe.
* Hạn chế tình trạng mất thời gian tìm chỗ đỗ khi bãi xe đông.
* Hỗ trợ nhân viên thực hiện quy trình **Check-in / Check-out** phương tiện.
* Theo dõi trạng thái các vị trí đỗ xe theo từng khu vực.
* Hỗ trợ chủ bãi xe quản lý hoạt động kinh doanh và vận hành bãi đỗ.
* Quản lý tập trung thông tin khách hàng, phương tiện, đơn đặt chỗ và bãi xe.
* Xây dựng nền tảng có khả năng mở rộng thêm các chức năng thông minh trong tương lai.

---

## Đối tượng sử dụng

Hệ thống bao gồm các nhóm người dùng chính:

### Khách hàng

Khách hàng có thể:

* Đăng ký và đăng nhập tài khoản.
* Quản lý thông tin cá nhân.
* Quản lý phương tiện và biển số xe.
* Tìm kiếm bãi đỗ xe.
* Xem thông tin chi tiết bãi xe.
* Kiểm tra vị trí còn trống.
* Đặt chỗ đỗ xe trước.
* Theo dõi lịch sử đặt chỗ.
* Hủy hoặc quản lý đơn đặt chỗ.
* Đánh giá và nhận xét bãi đỗ xe.
* Nhận thông báo từ hệ thống.

### Nhân viên bãi xe

Nhân viên có thể:

* Quản lý phương tiện ra/vào bãi.
* Check-in phương tiện.
* Check-out phương tiện.
* Kiểm tra thông tin đặt chỗ của khách.
* Theo dõi trạng thái các vị trí đỗ xe.
* Xác nhận phương tiện và biển số xe.
* Theo dõi các phương tiện hiện đang có trong bãi.

### Chủ bãi xe

Chủ bãi xe có thể:

* Đăng ký tài khoản chủ bãi xe.
* Quản lý thông tin bãi đỗ xe.
* Quản lý các khu vực và vị trí đỗ.
* Quản lý nhân viên.
* Theo dõi tình trạng hoạt động của bãi xe.
* Quản lý đơn đặt chỗ.
* Theo dõi đánh giá của khách hàng.
* Phản hồi đánh giá.
* Theo dõi tình trạng sử dụng các vị trí đỗ xe.

### Quản trị viên

Quản trị viên có thể:

* Quản lý tài khoản người dùng.
* Quản lý chủ bãi xe.
* Quản lý thông tin hệ thống.
* Kiểm soát và hỗ trợ hoạt động của toàn bộ nền tảng.

---

## Chức năng chính

### Xác thực và phân quyền

* Đăng ký tài khoản.
* Đăng nhập / đăng xuất.
* Xác thực bằng JWT.
* Phân quyền theo vai trò.
* Quản lý thông tin hồ sơ cá nhân.

### Quản lý bãi đỗ xe

* Quản lý thông tin bãi xe.
* Quản lý khu vực đỗ xe.
* Quản lý vị trí đỗ xe.
* Theo dõi vị trí:

  * Trống.
  * Đã được đặt.
  * Đang có xe.
  * Không khả dụng.

### Tìm kiếm bãi xe

Khách hàng có thể tìm kiếm bãi xe dựa trên:

* Tên bãi xe.
* Khu vực.
* Loại phương tiện.
* Khoảng thời gian muốn gửi xe.
* Tình trạng còn chỗ.

### Đặt chỗ

* Chọn bãi xe.
* Chọn thời gian gửi xe.
* Chọn phương tiện.
* Kiểm tra vị trí còn trống.
* Tạo đơn đặt chỗ.
* Theo dõi trạng thái đơn.
* Xem lịch sử đặt chỗ.

### Quản lý phương tiện

* Thêm phương tiện.
* Cập nhật phương tiện.
* Quản lý biển số xe.
* Liên kết phương tiện với đơn đặt chỗ.

### Check-in / Check-out

Nhân viên có thể thực hiện:

**Check-in**

Khách đến bãi → nhập/kiểm tra biển số → xác nhận đặt chỗ → chọn vị trí → xe vào bãi.

**Check-out**

Nhập biển số → tìm thông tin xe đang gửi → xác nhận thời gian → hoàn tất trả xe → cập nhật vị trí về trạng thái trống.

### Đánh giá bãi xe

* Khách hàng đánh giá bãi xe.
* Chấm điểm theo số sao.
* Viết nhận xét.
* Hiển thị điểm đánh giá trung bình.
* Chủ bãi xe có thể phản hồi đánh giá.

### Thông báo

Hệ thống hỗ trợ gửi thông báo liên quan đến:

* Đơn đặt chỗ.
* Trạng thái đặt chỗ.
* Check-in.
* Check-out.
* Các thay đổi liên quan đến tài khoản hoặc dịch vụ.

---

## Kiến trúc hệ thống

Dự án được chia thành hai thành phần chính:

```text
ParkingManagement
│
├── ParkingManagement.API
│   ├── Controllers
│   ├── DTOs
│   ├── Models
│   ├── Services
│   ├── Data
│   └── Program.cs
│
├── ParkingManagement.Web
│   ├── Controllers
│   ├── ViewModels
│   ├── Services
│   ├── Views
│   ├── wwwroot
│   └── Program.cs
│
└── README.md
```

### ParkingManagement.API

Đảm nhiệm:

* Xử lý nghiệp vụ.
* Truy xuất dữ liệu.
* Xác thực người dùng.
* Phân quyền.
* Cung cấp RESTful API cho Web Client.

### ParkingManagement.Web

Đảm nhiệm:

* Giao diện người dùng.
* Nhận thao tác từ người dùng.
* Gửi request tới API.
* Hiển thị dữ liệu và kết quả xử lý.

---

## Mô hình hoạt động

```text
Người dùng
    │
    ▼
ParkingManagement.Web
    │
    │ HTTP Request
    ▼
ParkingManagement.API
    │
    ▼
Business Logic / Services
    │
    ▼
Entity Framework Core
    │
    ▼
SQL Server
```

Hệ thống áp dụng mô hình phân tách giữa **Presentation Layer**, **Business Logic** và **Data Access**, giúp mã nguồn dễ quản lý, bảo trì và mở rộng.

---

## Công nghệ sử dụng

### Backend

* C#
* ASP.NET Core
* ASP.NET Core Web API
* Entity Framework Core
* RESTful API
* JWT Authentication

### Frontend

* ASP.NET Core MVC
* Razor View
* HTML5
* CSS3
* JavaScript
* Bootstrap

### Database

* Microsoft SQL Server
* Entity Framework Core

### Công cụ phát triển

* Visual Studio
* Visual Studio Code
* SQL Server Management Studio
* Git
* GitHub
* Postman / Swagger

---

## Một số API chính

Ví dụ cấu trúc API:

```text
/api/v1/auth
/api/v1/parkinglots
/api/v1/bookings
/api/v1/reviews
/api/v1/notifications
/api/v1/vehicles
```

Các API được sử dụng để kết nối giữa **ParkingManagement.Web** và **ParkingManagement.API**.

---

## Cài đặt và chạy dự án

### 1. Clone repository

```bash
git clone <repository-url>
```

Di chuyển vào thư mục dự án:

```bash
cd ParkingManagement
```

---

### 2. Cấu hình Database

Cập nhật connection string trong:

```text
ParkingManagement.API/appsettings.json
```

Ví dụ:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ParkingManagement;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

### 3. Cấu hình API

Trong project Web, kiểm tra địa chỉ API.

Ví dụ:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7264"
  }
}
```

Địa chỉ trên phải trùng với địa chỉ mà project **ParkingManagement.API** đang chạy.

---

### 4. Khởi chạy API

Chạy:

```text
ParkingManagement.API
```

Ví dụ API:

```text
https://localhost:7264
```

---

### 5. Khởi chạy Web

Sau khi API đã chạy, khởi động:

```text
ParkingManagement.Web
```

Sau đó truy cập địa chỉ localhost được hiển thị trên terminal hoặc Visual Studio.

---

## Một số dữ liệu chính của hệ thống

Hệ thống quản lý các nhóm dữ liệu như:

```text
Users
Vehicles
ParkingLots
ParkingAreas
ParkingSpots
Bookings
BookingDetails
Reviews
Notifications
Employees
```

Quan hệ giữa các dữ liệu được quản lý thông qua **Entity Framework Core**.

---

## Luồng đặt chỗ cơ bản

```text
Đăng nhập
    ↓
Tìm kiếm bãi xe
    ↓
Xem chi tiết bãi xe
    ↓
Chọn thời gian gửi
    ↓
Chọn phương tiện
    ↓
Kiểm tra chỗ trống
    ↓
Đặt chỗ
    ↓
Nhận xác nhận
    ↓
Đến bãi xe
    ↓
Check-in
    ↓
Đỗ xe
    ↓
Check-out
```

---

##  Hướng phát triển

Trong tương lai, hệ thống có thể mở rộng thêm:

* Gợi ý bãi xe gần nhất.
* Hiển thị số lượng chỗ trống theo thời gian thực.
* Thanh toán trực tuyến.
* QR Code cho Check-in / Check-out.
* Nhận diện biển số xe bằng AI.
* Camera giám sát phương tiện.
* Thống kê doanh thu cho chủ bãi.
* Dashboard quản trị.
* Dự đoán nhu cầu đỗ xe.
* Chatbot hỗ trợ khách hàng.
* Ứng dụng Mobile cho khách hàng.

---

## Mục đích thực hiện

Dự án được xây dựng phục vụ cho mục đích học tập và đồ án chuyên ngành **Hệ thống thông tin quản lý**, đồng thời giúp vận dụng các kiến thức về:

* Phân tích và thiết kế hệ thống.
* Cơ sở dữ liệu.
* Lập trình hướng đối tượng.
* Phát triển Web.
* RESTful API.
* Kiến trúc MVC.
* Authentication và Authorization.
* Quản lý mã nguồn bằng Git/GitHub.

---

## Tác giả

**Nguyễn Ngọc Thanh Trúc**

Sinh viên ngành **Hệ thống thông tin quản lý**

---

## 📄 License

Dự án được phát triển chủ yếu cho mục đích học tập và nghiên cứu.
