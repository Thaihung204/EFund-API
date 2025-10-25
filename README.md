1. Unit Test Setup Prompts

Prompt:

“Sửa mock _repoMock cho phương thức GetAsync<TEntity> để khớp với chữ ký: Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity,bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = null, int? skip = null, int? take = null)”

Purpose:

Giúp viết đúng mock khi unit test các phương thức của SavingGoalService.

2. Generate Unit Tests for Service Methods

Prompt:

“Dựa vào phương thức GetAllByUserAsync, sinh các test case cho các hàm service: CreateAsync, GetByIdAsync, UpdateAsync, DeleteAsync, đảm bảo kiểm tra cả trường hợp hợp lệ và trường hợp negative path.”

Purpose:

Sinh nhanh các unit test chuẩn xUnit + Moq cho tất cả các chức năng CRUD của SavingGoalService.

3. Generate Additional Edge Case Tests

Prompt:

“Sinh thêm 3 test case nữa cho SavingGoalService, tập trung vào các trường hợp biên như TargetAmount = 0, user chưa có goal nào, hoặc update với DurationMonths không hợp lệ.”

Purpose:

Kiểm tra các trường hợp đặc biệt, đảm bảo service xử lý đúng các edge case.

4. Prompt về Repository Interface

Prompt:

“Dựa trên interface IRepository và IReadOnlyRepository, mock các phương thức như GetByIdAsync, GetAsync<Transaction> để MapToResponseAsync hoạt động trong unit test.”

Purpose:

Đảm bảo unit test chạy mà không cần kết nối database thật, chỉ dùng dữ liệu mock.

5. Testing Mapping Logic

Prompt:

“Viết test case kiểm tra MapToResponseAsync trả về SavingGoalResponse đúng với tính toán DailyBudget, CurrentSaved, Progress.”

Purpose:

Đảm bảo dữ liệu phản hồi từ service luôn chính xác theo logic tính toán.

6. Optional: Generate Full Test File

Prompt:

“Viết một file unit test đầy đủ cho SavingGoalService với tất cả phương thức CRUD, sử dụng xUnit và Moq, bao gồm cả positive và negative scenarios.”

Purpose:

Tiết kiệm thời gian viết test, chuẩn hóa cấu trúc test và dễ maintain.