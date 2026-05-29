# TOEIC Learning App (MVC + .NET 10)

## Mục tiêu app
Ứng dụng web học TOEIC Listening + Reading theo lộ trình từng phần, có luyện tập, mock test, thống kê học tập, vocabulary, grammar clinic, và khu vực quản trị/giáo viên.

## Công nghệ sử dụng
- .NET 10
- ASP.NET Core MVC + Razor Views
- Entity Framework Core + SQLite
- ASP.NET Core Identity (Register/Login/Role)
- xUnit (unit test + integration-style test)

## Cấu trúc solution
- `Toeic.Web`: UI MVC, controllers, views
- `Toeic.Application`: nghiệp vụ ứng dụng (services)
- `Toeic.Domain`: entities + enums
- `Toeic.Infrastructure`: DbContext, Identity, migrations, seeders
- `Toeic.Tests`: test project

## Setup local
1. Cài .NET SDK 10.
2. Ở thư mục gốc repo, chạy:

```bash
dotnet restore
dotnet build
```

3. Chạy app:

```bash
dotnet run --project Toeic.Web
```

## Cách chạy migration
Migration đã được tạo sẵn trong `Toeic.Infrastructure/Persistence/Migrations`.

- Cập nhật DB theo migration mới nhất:

```bash
dotnet ef database update --project Toeic.Infrastructure --startup-project Toeic.Web
```

- Nếu cần tạo migration mới:

```bash
dotnet ef migrations add <MigrationName> --project Toeic.Infrastructure --startup-project Toeic.Web --output-dir Persistence/Migrations
```

## Cách tạo user admin
Role `Student`, `Teacher`, `Admin` được seed tự động khi app start.

Các bước tạo Admin:
1. Đăng ký tài khoản mới ở `/Account/Register`.
2. Tài khoản mới mặc định thuộc role `Student`.
3. Mở SQLite DB (file `Toeic.Web/toeic-learning-local.db`) và gán role `Admin` cho user trong bảng `AspNetUserRoles` (dùng `UserId` của user và `RoleId` tương ứng role `Admin` trong `AspNetRoles`).

## Chức năng đã có
- Auth: Register/Login/Logout, role-based auth, AccessDenied.
- Admin:
  - Quản lý Question: list + create/edit/delete/details.
  - Import câu hỏi bằng JSON (`/Admin/QuestionImport`).
- Practice:
  - Chọn Part, làm từng câu, xem đúng/sai + explanation.
  - Lưu `PracticeAttempt`.
  - Trang `Practice Result` thống kê theo Part/GrammarTag.
- Grammar Clinic:
  - Gom lỗi sai theo `GrammarTag`, xem chi tiết câu sai.
- Vocabulary:
  - Entity + seed 30 từ.
  - Trang list từ vựng.
  - Flashcards (`/Vocabulary/Flashcards`).
- Mock Test:
  - Seed `TOEIC Mini Test 01` (20 câu).
  - Start test, làm tuần tự, submit và tính điểm.
  - Kết quả: đúng/tổng, tỷ lệ đúng, danh sách câu sai + explanation.
  - Timer countdown + auto submit khi hết giờ.
- Dashboard:
  - Tổng luyện tập, accuracy tổng/theo Part.
  - Số mock test đã làm, điểm gần nhất.
  - Weakest grammar tag.
  - Recommended Next Study.
  - Assignment của học viên.
- Teacher:
  - Quản lý lớp học (`/Teacher/Classes`).
  - Thêm học viên theo email (chặn trùng).
  - Tạo assignment mock test cho lớp.

## Chức năng chưa làm / giới hạn hiện tại
- Chưa có upload audio/image cho các part listening.
- Chưa có adaptive engine thực sự (MSA/CAT).
- Assignment chưa chấm theo deadline, chưa có submission tracking theo assignment.
- Chưa có trang quản lý nâng cao cho Teacher/Admin (lọc, phân trang, báo cáo sâu).
- Integration test chưa cover full browser login cookie flow end-to-end.

## Roadmap tiếp theo
1. Bổ sung audio pipeline cho Listening Part 1-4.
2. Hoàn thiện assignment workflow (làm bài theo assignment + progress lớp).
3. Tăng độ sâu analytics (theo thời gian, độ khó, topic).
4. Cải thiện test coverage E2E cho auth + practice + mock test.
5. Tối ưu UX test player (review panel, autosave state).
