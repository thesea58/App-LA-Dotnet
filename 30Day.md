Dưới đây là cách chia dự án thành **các bước nhỏ theo từng ngày** để một coding agent có thể làm dần, không bị quá tải vì yêu cầu quá lớn.

Mục tiêu MVP trước: **web học TOEIC Listening + Reading**, dùng **C# .NET 10**, **ASP.NET Core MVC**, có thể dùng **Blazor component** cho phần làm bài tương tác.

---

# Nguyên tắc giao việc cho Agent

Không giao kiểu:

> “Hãy tạo app TOEIC đầy đủ có login, test, dashboard, vocab, grammar, admin…”

Mà giao theo mẫu:

```text
Bạn chỉ thực hiện task của ngày hôm nay.
Không tự mở rộng scope.
Chỉ tạo/sửa các file cần thiết.
Sau khi code xong, hãy liệt kê:
1. File đã tạo/sửa
2. Cách chạy
3. Cách test
4. Việc chưa làm
```

---

# Kế hoạch 30 ngày cho MVP TOEIC App

## Tuần 1 — Khởi tạo project và kiến trúc nền

### Ngày 1 — Tạo solution .NET 10 MVC

**Mục tiêu:** tạo skeleton project.

**Việc cần làm:**

* Tạo solution `ToeicLearningApp`
* Tạo project MVC `Toeic.Web`
* Tạo các project phụ:

  * `Toeic.Domain`
  * `Toeic.Application`
  * `Toeic.Infrastructure`
  * `Toeic.Tests`

**Prompt cho Agent:**

```text
Tạo solution .NET 10 cho web app học TOEIC theo mô hình MVC.

Yêu cầu:
- Solution name: ToeicLearningApp
- Projects:
  - Toeic.Web: ASP.NET Core MVC
  - Toeic.Domain: class library
  - Toeic.Application: class library
  - Toeic.Infrastructure: class library
  - Toeic.Tests: xUnit test project
- Thiết lập project reference đúng:
  - Web tham chiếu Application, Infrastructure, Domain
  - Application tham chiếu Domain
  - Infrastructure tham chiếu Application và Domain
  - Tests tham chiếu Application, Domain
- Không cần tạo chức năng nghiệp vụ.
- Chỉ cần project build thành công.
```

**Hoàn thành khi:**

```bash
dotnet build
```

chạy thành công.

---

### Ngày 2 — Tạo layout MVC cơ bản

**Mục tiêu:** tạo giao diện khung.

**Việc cần làm:**

* Home page
* Navigation menu
* Các link chính:

  * Dashboard
  * Practice
  * Vocabulary
  * Grammar
  * Mock Test
  * Admin

**Prompt cho Agent:**

```text
Trong project Toeic.Web, tạo layout MVC cơ bản cho web học TOEIC.

Yêu cầu:
- Sử dụng Razor Views.
- Tạo menu chính gồm:
  - Dashboard
  - Practice
  - Vocabulary
  - Grammar
  - Mock Test
  - Admin
- Trang Home giới thiệu app học TOEIC 2026.
- Giao diện đơn giản, rõ ràng, chưa cần database.
- Không thêm chức năng login.
```

---

### Ngày 3 — Cấu hình Entity Framework Core

**Mục tiêu:** chuẩn bị database.

**Việc cần làm:**

* Cài EF Core
* Tạo `AppDbContext`
* Cấu hình SQL Server hoặc SQLite cho dev
* Tạo connection string

**Prompt cho Agent:**

```text
Cấu hình Entity Framework Core cho project ToeicLearningApp.

Yêu cầu:
- Tạo AppDbContext trong Toeic.Infrastructure/Persistence.
- Dùng SQLite cho môi trường local development.
- Cấu hình DbContext trong Program.cs.
- Tạo connection string trong appsettings.json.
- Chưa cần tạo entity phức tạp.
- Tạo migration đầu tiên tên InitDatabase.
```

---

### Ngày 4 — Tạo Authentication cơ bản

**Mục tiêu:** người dùng có thể đăng ký, đăng nhập.

**Việc cần làm:**

* ASP.NET Core Identity
* User model `ApplicationUser`
* Login/Register/Logout

**Prompt cho Agent:**

```text
Thêm ASP.NET Core Identity vào project Toeic.Web.

Yêu cầu:
- Tạo ApplicationUser kế thừa IdentityUser.
- Tích hợp Identity với AppDbContext.
- Bật chức năng Register, Login, Logout.
- Sau khi login, redirect về Dashboard.
- Chưa cần phân quyền Student/Teacher/Admin.
```

---

### Ngày 5 — Tạo Role: Student, Teacher, Admin

**Mục tiêu:** phân quyền nền.

**Việc cần làm:**

* Seed role
* Gán user mới mặc định là Student
* Admin area chỉ Admin vào được

**Prompt cho Agent:**

```text
Thêm role-based authorization cho app.

Yêu cầu:
- Tạo 3 roles: Student, Teacher, Admin.
- Khi user đăng ký mới, mặc định gán role Student.
- Tạo AdminController với action Index.
- Chỉ user role Admin mới truy cập được /Admin.
- Nếu chưa có quyền thì trả về AccessDenied.
```

---

## Tuần 2 — Thiết kế dữ liệu TOEIC

### Ngày 6 — Tạo entity Question

**Mục tiêu:** lưu câu hỏi TOEIC.

**Entity chính:**

```csharp
Question
- Id
- Part
- Skill
- QuestionText
- Explanation
- Difficulty
- Topic
- GrammarTag
- CreatedAt
```

**Prompt cho Agent:**

```text
Tạo entity Question cho ngân hàng câu hỏi TOEIC.

Yêu cầu:
- Entity đặt trong Toeic.Domain/Entities.
- Các field:
  - Id
  - Part: enum ToeicPart
  - Skill: enum ToeicSkill
  - QuestionText
  - Explanation
  - Difficulty
  - Topic
  - GrammarTag
  - CreatedAt
- Tạo enum ToeicPart gồm Part1 đến Part7.
- Tạo enum ToeicSkill gồm Listening, Reading.
- Thêm DbSet<Question> vào AppDbContext.
- Tạo migration AddQuestion.
```

---

### Ngày 7 — Tạo entity AnswerOption

**Mục tiêu:** lưu đáp án A/B/C/D.

```csharp
AnswerOption
- Id
- QuestionId
- Label
- Text
- IsCorrect
```

**Prompt cho Agent:**

```text
Tạo entity AnswerOption cho câu hỏi TOEIC.

Yêu cầu:
- Một Question có nhiều AnswerOption.
- AnswerOption gồm:
  - Id
  - QuestionId
  - Label: A/B/C/D
  - Text
  - IsCorrect
- Cấu hình relationship trong AppDbContext.
- Tạo migration AddAnswerOption.
```

---

### Ngày 8 — Seed dữ liệu câu hỏi mẫu Part 5

**Mục tiêu:** có dữ liệu để test.

**Prompt cho Agent:**

```text
Tạo seed data cho 10 câu hỏi TOEIC Reading Part 5.

Yêu cầu:
- Dữ liệu tự biên soạn, không copy từ ETS.
- Mỗi câu có 4 đáp án A/B/C/D.
- Có đúng 1 đáp án đúng.
- Có Explanation tiếng Việt.
- Có GrammarTag, ví dụ:
  - Part of speech
  - Preposition
  - Verb tense
  - Conjunction
- Seed tự chạy khi app start nếu database chưa có câu hỏi.
```

---

### Ngày 9 — Trang Admin quản lý Question

**Mục tiêu:** xem danh sách câu hỏi.

**Prompt cho Agent:**

```text
Tạo trang Admin quản lý Question.

Yêu cầu:
- URL: /Admin/Questions
- Chỉ Admin truy cập được.
- Hiển thị danh sách câu hỏi gồm:
  - Id
  - Part
  - Skill
  - QuestionText
  - Difficulty
  - Topic
  - GrammarTag
- Chưa cần Create/Edit/Delete.
```

---

### Ngày 10 — CRUD Question đơn giản

**Mục tiêu:** admin tạo/sửa/xóa câu hỏi.

**Prompt cho Agent:**

```text
Thêm CRUD cho Question trong Admin area.

Yêu cầu:
- Create Question
- Edit Question
- Delete Question
- Xem detail Question kèm danh sách AnswerOptions
- Validate:
  - QuestionText không rỗng
  - Part bắt buộc
  - Skill bắt buộc
- Chưa cần upload audio/image.
```

---

## Tuần 3 — Chức năng luyện tập theo Part

### Ngày 11 — Tạo PracticeController

**Mục tiêu:** chọn Part để luyện.

**Prompt cho Agent:**

```text
Tạo PracticeController cho học viên luyện TOEIC theo từng Part.

Yêu cầu:
- URL: /Practice
- Hiển thị danh sách Part 1 đến Part 7.
- Khi chọn Part, chuyển đến /Practice/Part/{partNumber}
- Trang Part hiển thị câu hỏi thuộc Part đó.
- Chỉ user đã login mới luyện được.
```

---

### Ngày 12 — Làm bài một câu hỏi

**Mục tiêu:** user chọn đáp án và biết đúng/sai.

**Prompt cho Agent:**

```text
Tạo chức năng làm 1 câu hỏi TOEIC.

Yêu cầu:
- Hiển thị QuestionText và các AnswerOptions.
- User chọn đáp án.
- Sau khi submit:
  - Hiển thị đúng/sai.
  - Hiển thị đáp án đúng.
  - Hiển thị Explanation tiếng Việt.
- Chưa cần lưu lịch sử làm bài.
```

---

### Ngày 13 — Lưu lịch sử làm bài

**Entity:**

```csharp
PracticeAttempt
- Id
- UserId
- QuestionId
- SelectedAnswerOptionId
- IsCorrect
- AnsweredAt
```

**Prompt cho Agent:**

```text
Thêm chức năng lưu lịch sử luyện tập.

Yêu cầu:
- Tạo entity PracticeAttempt.
- Lưu user, question, đáp án đã chọn, đúng/sai, thời gian trả lời.
- Khi user submit câu trả lời, ghi vào database.
- Tạo migration AddPracticeAttempt.
```

---

### Ngày 14 — Trang kết quả luyện tập

**Mục tiêu:** xem thống kê cơ bản.

**Prompt cho Agent:**

```text
Tạo trang Practice Result cho học viên.

Yêu cầu:
- URL: /Practice/Result
- Hiển thị:
  - Tổng số câu đã làm
  - Số câu đúng
  - Tỷ lệ đúng
  - Thống kê theo Part
  - Thống kê theo GrammarTag
- Chỉ hiển thị dữ liệu của user hiện tại.
```

---

### Ngày 15 — Grammar Clinic cơ bản

**Mục tiêu:** học grammar theo lỗi sai.

**Prompt cho Agent:**

```text
Tạo trang Grammar Clinic.

Yêu cầu:
- URL: /Grammar
- Dựa trên PracticeAttempt sai của user.
- Gom lỗi theo GrammarTag.
- Hiển thị các nhóm lỗi, ví dụ:
  - Part of speech
  - Verb tense
  - Preposition
  - Conjunction
- Mỗi nhóm hiển thị số câu sai.
- Khi click vào nhóm, hiển thị các câu user từng làm sai.
```

---

## Tuần 4 — Vocabulary, Mock Test, Dashboard

### Ngày 16 — Tạo entity Vocabulary

```csharp
VocabularyEntry
- Id
- Word
- MeaningVi
- ExampleSentence
- Topic
- PartOfSpeech
```

**Prompt cho Agent:**

```text
Tạo chức năng Vocabulary cho TOEIC.

Yêu cầu:
- Tạo entity VocabularyEntry.
- Field:
  - Word
  - MeaningVi
  - ExampleSentence
  - Topic
  - PartOfSpeech
- Seed 30 từ vựng TOEIC tự biên soạn theo chủ đề:
  - Office
  - Meeting
  - Travel
  - Finance
  - Recruitment
  - Shopping
- Tạo trang /Vocabulary hiển thị danh sách từ.
```

---

### Ngày 17 — Flashcard học từ vựng

**Prompt cho Agent:**

```text
Tạo chức năng flashcard từ vựng.

Yêu cầu:
- URL: /Vocabulary/Flashcards
- Hiển thị từng từ một.
- Mặt trước: Word + PartOfSpeech.
- Mặt sau: MeaningVi + ExampleSentence.
- Có nút Next.
- Chưa cần thuật toán spaced repetition.
```

---

### Ngày 18 — Tạo Mock Test entity

```csharp
MockTest
MockTestQuestion
MockTestAttempt
MockTestAnswer
```

**Prompt cho Agent:**

```text
Tạo cấu trúc dữ liệu cho Mock Test TOEIC.

Yêu cầu:
- MockTest:
  - Id
  - Title
  - Description
  - DurationMinutes
- MockTestQuestion:
  - MockTestId
  - QuestionId
  - OrderNo
- MockTestAttempt:
  - Id
  - UserId
  - MockTestId
  - StartedAt
  - SubmittedAt
  - Score
- MockTestAnswer:
  - MockTestAttemptId
  - QuestionId
  - SelectedAnswerOptionId
  - IsCorrect
- Tạo migration AddMockTest.
```

---

### Ngày 19 — Tạo mock test ngắn 20 câu

**Mục tiêu:** chưa cần full 200 câu.

**Prompt cho Agent:**

```text
Tạo chức năng mock test ngắn.

Yêu cầu:
- Tạo seed MockTest đầu tiên tên "TOEIC Mini Test 01".
- Gồm 20 câu lấy từ question bank.
- URL: /MockTest
- User có thể bấm Start.
- Khi start, tạo MockTestAttempt.
- Hiển thị lần lượt 20 câu.
```

---

### Ngày 20 — Submit Mock Test và tính điểm

**Prompt cho Agent:**

```text
Hoàn thiện submit Mock Test.

Yêu cầu:
- User làm 20 câu.
- Khi submit:
  - Lưu toàn bộ đáp án vào MockTestAnswer.
  - Tính số câu đúng.
  - Lưu Score vào MockTestAttempt.
  - Chuyển đến trang Result.
- Trang Result hiển thị:
  - Số câu đúng / tổng câu
  - Tỷ lệ đúng
  - Danh sách câu sai
  - Explanation cho từng câu sai
```

---

## Tuần 5 — Dashboard và cải thiện học tập

### Ngày 21 — Dashboard cá nhân

**Prompt cho Agent:**

```text
Tạo Dashboard cho học viên.

Yêu cầu:
- URL: /Dashboard
- Hiển thị:
  - Tổng số câu đã luyện
  - Tỷ lệ đúng tổng
  - Tỷ lệ đúng theo Part
  - Số mock test đã làm
  - Điểm mock test gần nhất
  - GrammarTag yếu nhất
- Chỉ lấy dữ liệu user hiện tại.
```

---

### Ngày 22 — Gợi ý học tiếp

**Prompt cho Agent:**

```text
Thêm chức năng gợi ý học tiếp trên Dashboard.

Logic đơn giản:
- Nếu Part nào tỷ lệ đúng thấp nhất, gợi ý luyện Part đó.
- Nếu GrammarTag nào sai nhiều nhất, gợi ý mở Grammar Clinic.
- Nếu chưa làm mock test, gợi ý làm TOEIC Mini Test 01.

Hiển thị dưới dạng card "Recommended Next Study".
```

---

### Ngày 23 — Teacher: tạo lớp học

**Prompt cho Agent:**

```text
Tạo chức năng Teacher quản lý lớp học.

Yêu cầu:
- Chỉ role Teacher hoặc Admin truy cập.
- Entity ClassRoom:
  - Id
  - Name
  - Description
  - TeacherId
  - CreatedAt
- Trang /Teacher/Classes:
  - Danh sách lớp
  - Tạo lớp mới
  - Xem chi tiết lớp
```

---

### Ngày 24 — Teacher: thêm học viên vào lớp

**Prompt cho Agent:**

```text
Thêm chức năng thêm học viên vào lớp.

Yêu cầu:
- Entity ClassEnrollment:
  - Id
  - ClassRoomId
  - StudentId
  - EnrolledAt
- Teacher có thể thêm Student vào lớp bằng email.
- Trang chi tiết lớp hiển thị danh sách học viên.
- Không cho thêm trùng học viên vào cùng một lớp.
```

---

### Ngày 25 — Teacher: giao bài mock test

**Prompt cho Agent:**

```text
Tạo chức năng Teacher giao mock test cho lớp.

Yêu cầu:
- Entity Assignment:
  - Id
  - ClassRoomId
  - MockTestId
  - Title
  - DueDate
  - CreatedAt
- Teacher tạo assignment cho một lớp.
- Student trong lớp nhìn thấy assignment trên Dashboard.
- Chưa cần chấm theo deadline.
```

---

## Tuần 6 — Hoàn thiện MVP

### Ngày 26 — Admin import câu hỏi bằng JSON

**Prompt cho Agent:**

```text
Tạo chức năng Admin import Question bằng JSON.

Yêu cầu:
- URL: /Admin/QuestionImport
- Admin paste JSON vào textarea.
- JSON gồm:
  - QuestionText
  - Part
  - Skill
  - Explanation
  - Difficulty
  - Topic
  - GrammarTag
  - Options[]
- Validate:
  - Có ít nhất 2 options.
  - Có đúng 1 đáp án đúng.
  - QuestionText không rỗng.
- Nếu hợp lệ thì lưu vào database.
```

---

### Ngày 27 — UI cải thiện cho Test Player

**Prompt cho Agent:**

```text
Cải thiện giao diện làm bài.

Yêu cầu:
- Tạo Test Player layout dùng chung cho Practice và Mock Test.
- Có:
  - Vùng câu hỏi
  - Vùng đáp án
  - Nút Previous / Next
  - Thanh tiến độ
  - Số câu hiện tại / tổng số câu
- Không cần timer phức tạp.
```

---

### Ngày 28 — Timer cho Mock Test

**Prompt cho Agent:**

```text
Thêm timer cho Mock Test.

Yêu cầu:
- Mỗi MockTest có DurationMinutes.
- Khi user start test, hiển thị countdown timer.
- Nếu hết giờ, tự động submit.
- Lưu SubmittedAt.
- Dùng JavaScript đơn giản hoặc Blazor component nếu phù hợp.
```

---

### Ngày 29 — Test và sửa lỗi

**Prompt cho Agent:**

```text
Viết test cho các service chính.

Yêu cầu:
- Unit test cho logic tính điểm MockTest.
- Unit test cho logic tính tỷ lệ đúng theo Part.
- Unit test cho logic tìm GrammarTag yếu nhất.
- Integration test đơn giản:
  - User login
  - Start mock test
  - Submit answer
- Không refactor lớn.
```

---

### Ngày 30 — Dọn project và viết README

**Prompt cho Agent:**

```text
Dọn project và viết tài liệu README.

Yêu cầu README gồm:
- Mục tiêu app
- Công nghệ sử dụng
- Cách setup local
- Cách chạy migration
- Cách tạo user admin
- Các chức năng đã có
- Các chức năng chưa làm
- Roadmap tiếp theo

Không thêm feature mới trong ngày này.
```

---

# Backlog sau MVP

Sau 30 ngày, app đã có bản học TOEIC cơ bản. Các chức năng sau nên để giai đoạn 2:

| Chức năng                   | Ghi chú                                            |
| --------------------------- | -------------------------------------------------- |
| Listening Part 1–4 có audio | Cần xử lý file audio, transcript, player           |
| Upload ảnh cho Part 1       | Cần quản lý asset/image                            |
| Blazor Test Player nâng cao | Timer, review panel, auto-save                     |
| Spaced Repetition thật      | Thuật toán ôn từ vựng theo ngày                    |
| Adaptive Test               | Chọn câu theo trình độ                             |
| Estimated TOEIC Score       | Chỉ nên ghi là điểm ước lượng, không phải điểm ETS |
| Speaking/Writing            | Có thể thêm sau, phức tạp hơn                      |
| AI tạo câu hỏi              | Phải có bước human review                          |

---

# Prompt tổng để bắt đầu giao cho Agent

Bạn có thể dùng prompt này làm “system prompt” hoặc “project instruction” cho coding agent:

```text
Bạn là coding agent hỗ trợ xây dựng web app học TOEIC bằng C# .NET 10.

Công nghệ:
- ASP.NET Core MVC
- Entity Framework Core
- ASP.NET Core Identity
- Razor Views
- Có thể dùng Blazor component cho phần UI tương tác cao, nhưng không bắt buộc trong giai đoạn đầu.

Kiến trúc:
- Toeic.Web
- Toeic.Application
- Toeic.Domain
- Toeic.Infrastructure
- Toeic.Tests

Nguyên tắc:
- Mỗi lần chỉ làm đúng task được giao.
- Không tự ý thêm feature ngoài yêu cầu.
- Không sinh quá nhiều code không cần thiết.
- Code phải build được.
- Sau mỗi task phải báo:
  1. File đã tạo/sửa
  2. Cách chạy
  3. Cách test
  4. Việc chưa làm

Lưu ý nội dung TOEIC:
- Không copy câu hỏi ETS.
- Chỉ tạo câu hỏi tự biên soạn theo phong cách TOEIC.
- Nội dung tập trung vào workplace English, vocabulary, grammar, listening, reading.
```

---

# Cách chạy dự án theo ngày

Mỗi ngày bạn nên làm theo quy trình này:

```text
1. Copy task của ngày hôm đó cho Agent.
2. Đợi Agent code.
3. Chạy dotnet build.
4. Chạy dotnet test nếu có test.
5. Mở web kiểm tra thủ công.
6. Nếu lỗi, chỉ yêu cầu Agent sửa lỗi đó, không thêm chức năng mới.
7. Commit code với message rõ ràng.
```

Ví dụ commit:

```bash
git add .
git commit -m "Day 06: Add TOEIC Question entity"
```

Kế hoạch tốt nhất là đi theo thứ tự:

```text
Nền project
→ Database
→ Auth
→ Question bank
→ Practice
→ Mock test
→ Dashboard
→ Teacher/Admin
→ Test/README
```

Không nên làm Listening/audio, Adaptive Test, AI tạo câu hỏi ngay từ đầu vì các phần đó dễ làm project phình to và khó kiểm soát.
