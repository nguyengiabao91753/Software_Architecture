# 🧾 VoucherAPI — Hướng dẫn chạy và test hệ thống CQRS (.NET 9 + RabbitMQ + EF Core)

---

## 1️⃣ Chuẩn bị môi trường

### 🧩 Yêu cầu hệ thống
- .NET SDK **9.0+**
- QL Server (đã tạo sẵn 2 database: `VoucherWriteDB`, `VoucherReadDB`)
- ✅ Docker (để chạy RabbitMQ)

---

## 2️⃣ Khởi động RabbitMQ (Message Broker)

Chạy lệnh bên dưới để tạo container RabbitMQ:

```bash
docker run -d --hostname rabbit --name rabbit \
 -p 15672:15672 -p 5672:5672 rabbitmq:3-management


Sau khi khởi chạy, mở giao diện quản lý RabbitMQ tại:
👉 http://localhost:15672

Username: guest
Password: guest

🧱 3️⃣ Tạo và cập nhật database

EF Core sẽ tự động tạo bảng theo cấu hình DbContext.

Chạy lệnh dưới để tạo bảng trong ReadDB (nếu chưa có):

dotnet ef database update --startup-project Services/VoucherAPI/Voucher.QueryAPI


Hoặc tương tự với WriteDB:

dotnet ef database update --startup-project Services/VoucherAPI/Voucher.CommandAPI

🚀 4️⃣ Chạy các API song song

Mở 2 terminal riêng biệt:

🧠 Terminal 1 — Command API (WriteDB)
dotnet run --project Services/VoucherAPI/Voucher.CommandAPI


Port: 5000
Chức năng: Ghi dữ liệu (Create, Update, Publish event)

🔍 Terminal 2 — Query API (ReadDB)
dotnet run --project Services/VoucherAPI/Voucher.QueryAPI


Port: 5003
Chức năng: Đọc dữ liệu (truy vấn từ ReadDB)

🧪 5️⃣ Test API bằng Postman
🟢 (1) Tạo mới voucher

POST

http://localhost:5000/vouchers


Body (JSON):

{
  "voucherCode": "HELLO2025",
  "description": "Giảm 25% mừng năm mới",
  "discountType": "percent",
  "discountValue": 25,
  "startDate": "2025-01-01T00:00:00",
  "endDate": "2025-12-31T23:59:59",
  "quantity": 100
}


Kết quả mong đợi:

Trả về 201 Created cùng voucherId.

Bảng VoucherWriteDB.dbo.Vouchers có dữ liệu mới.

RabbitMQ hiển thị message trong queue voucher-created-queue.

📋 (2) Lấy danh sách voucher (từ ReadDB)

GET

http://localhost:5003/vouchers


Kết quả mong đợi:

200 OK

Danh sách voucher đã đồng bộ qua RabbitMQ (Consumer trong QueryAPI).

🔁 (3) Tăng lượt sử dụng voucher

PUT

http://localhost:5000/vouchers/{id}/use


Ví dụ:

http://localhost:5000/vouchers/460622f7-b2b8-4e84-8024-953b73555108/use


Kết quả mong đợi:

"Usage count increased successfully"

⚙️ (4) Cập nhật trạng thái voucher

PUT

http://localhost:5000/vouchers/{id}/status


Body (JSON):

{
  "status": "inactive"
}


Kết quả mong đợi:

"Voucher status updated to 'inactive' successfully"

🧰 6️⃣ Kiểm tra dữ liệu trong SQL Server

Chạy truy vấn sau để xác nhận đồng bộ giữa WriteDB và ReadDB:

-- Ghi (WriteDB)
SELECT VoucherCode, DiscountValue, UsedCount, Status
FROM VoucherWriteDB.dbo.Vouchers;

-- Đọc (ReadDB)
SELECT VoucherCode, DiscountValue, UsedCount, Status
FROM VoucherReadDB.dbo.Vouchers;


✅ Dữ liệu trong ReadDB sẽ tự động được cập nhật khi có event từ RabbitMQ.

🧩 7️⃣ Kiểm tra RabbitMQ

Vào tab Queues tại http://localhost:15672

Bạn sẽ thấy:

voucher-created-queue (consumer đang running)

voucher-created-queue_error (nếu consumer bị lỗi)

Khi gửi POST /vouchers, RabbitMQ sẽ nhận 1 message và QueryAPI sẽ consume để ghi vào ReadDB.

📂 8️⃣ Cấu trúc thư mục chính
Software_Architecture/
├── Services/
│   └── VoucherAPI/
│       ├── Voucher.CommandAPI/      # API ghi dữ liệu (WriteDB)
│       ├── Voucher.QueryAPI/        # API đọc dữ liệu (ReadDB)
│       ├── Voucher.Application/     # MediatR Commands, Handlers
│       ├── Voucher.Infrastructure/  # EF Core, Repository, DB Context
│       └── Voucher.Shared/          # DTO, Events, Contracts
│
├── docker-compose.yml
└── RUN_GUIDE.md                     # File hướng dẫn này

✅ 9️⃣ Tóm tắt kết quả mong đợi
Chức năng	Method	URL	Kết quả mong đợi
Tạo voucher	POST	/vouchers	201 Created
Xem danh sách voucher	GET	/vouchers (port 5003)	200 OK
Tăng lượt sử dụng	PUT	/vouchers/{id}/use	200 OK
Cập nhật trạng thái	PUT	/vouchers/{id}/status	200 OK
Kiểm tra RabbitMQ	-	voucher-created-queue	Message được consume