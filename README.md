# DEMO-CRUD
图书管理系统：
1. 核心/通用模块 (Core/Shared Modules)
   身份认证与授权 (Authentication & Authorization)

功能：用户登录、注册、密码管理、Token 生成与验证（推荐使用 JWT）。

目的：保障 API 安全，区分不同角色的用户权限。

图书搜索模块 (Book Search Module)

功能：提供强大的图书搜索功能，可按书名、作者、ISBN、分类进行模糊查询和精确查询。

目的：方便所有用户快速找到所需图书。

2. 用户模块 (Member-Facing Modules)
   个人信息管理 (Personal Profile Management)

功能：查看和修改个人资料（如联系方式）。

目的：用户自主管理个人信息。

我的借阅 (My Loans)

功能：查看当前借阅的书籍列表、应还日期、借阅历史。

目的：清晰展示用户的借阅状态。

图书预约 (Book Reservation)

功能：当书籍被借光时，用户可以预约。书籍归还后，按预约顺序通知用户。

目的：提高图书流转率和用户满意度。

3. 管理员模块 (Admin-Facing Modules)
   图书管理 (Book Management - CRUD)

功能：添加新书、修改图书信息、删除图书、管理图书库存（复本）。

目的：维护图书馆的图书资产。

分类/标签管理 (Category/Tag Management - CRUD)

功能：添加、修改、删除图书分类。

目的：更好地组织和管理图书。

用户管理 (User Management - CRUD)

功能：添加新用户、编辑用户信息、禁用/启用用户账户、设置用户角色（管理员/普通用户）。

目的：管理系统所有用户。

借阅与归还处理 (Loan Processing)

功能：为用户办理借书、还书手续。

目的：处理核心的借阅业务流程。

罚款管理 (Fine Management)

功能：处理逾期罚款的生成、查询和缴纳。

目的：管理逾期行为。

统计与报表 (Statistics & Reporting)

功能：（可选高级功能）生成报表，如热门借阅图书排行、用户借阅活跃度分析等。

目的：为图书馆运营提供数据支持。