using DEMO_CRUD.Data;
using DEMO_CRUD.Exceptions;
using DEMO_CRUD.Models.DTO;
using DEMO_CRUD.Models.Entity;
using Mapster;
using Microsoft.EntityFrameworkCore;
using static DEMO_CRUD.Constants.IServiceConstants;

namespace DEMO_CRUD.Services.Impl
{
    public class BooksServiceImpl : IBooksService
    {
        private readonly ApplicationDbContext _context;

        public BooksServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BookVO>> GetAllBooksAsync()
        {
            return await _context.Books
                // 加载关联的 Author 和 Publisher
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                // 将原本的 Book 实体映射为 BookVO
                .ProjectToType<BookVO>()
                .ToListAsync();
        }

        public async Task<BookVO?> GetBookByIdAsync(int id)
        {
            return await _context.Books
                // 同时加载关联的 Author 、Publisher
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                // 将原本的 Book 实体映射为 BookVO
                .ProjectToType<BookVO>()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BookVO>> SearchBooksAsync(string title, string isbn, string authorName,
            string publisherName, DateTime? publishedDateBegin, DateTime? publishedDateEnd)
        {
            // 获取所有书籍、加载关联的 Author 和 Publisher,然后转为LINQ查询
            var query = _context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .AsQueryable();

            // 一系列筛选条件
            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(b => b.Title.Contains(title));
            }

            if (!string.IsNullOrEmpty(isbn))
            {
                query = query.Where(b => b.Isbn.Contains(isbn));
            }

            if (!string.IsNullOrEmpty(authorName))
            {
                query = query.Where(b => b.Author.Name.Contains(authorName));
            }

            if (!string.IsNullOrEmpty(publisherName))
            {
                query = query.Where(b => b.Publisher.Name.Contains(publisherName));
            }

            if (publishedDateBegin.HasValue)
            {
                query = query.Where(b => b.PublishedDate >= publishedDateBegin.Value);
            }

            if (publishedDateEnd.HasValue)
            {
                query = query.Where(b => b.PublishedDate <= publishedDateEnd.Value);
            }

            List<BookVO> bookVos = await query.ProjectToType<BookVO>().ToListAsync();
            return bookVos;
        }

        public async Task<string> LoanBookAsync(int id, string username)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }

            if (book.Available <= 0)
            {
                throw new BusinessException(NOT_ENOUGH_AVAILABLE);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Name == username);
            if (user == null)
            {
                throw new ArgumentException(USER_NOT_FOUND);
            }

            var loan = new Loan
            {
                BookId = book.Id,
                UserId = user.Id,
                LoanDate = DateTime.Now,
                // 预计还书日期是1个月后
                DueDate = DateTime.Now.AddMonths(1),
                // 初始未归还
                ReturnDate = null
            };
            // 减少该书籍的可用数量
            book.Available -= 1;
            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();
            return $"书籍 {book.Title} 已借出给 {username}。";
        }

        public async Task<string> ReturnBookAsync(int id, string username)
        {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .FirstOrDefaultAsync(l => l.BookId == id && l.User.Name == username && l.ReturnDate == null);
            if (loan == null)
            {
                throw new ArgumentException(RECORD_NOT_FOUND);
            }

            loan.ReturnDate = DateTime.Now;
            // 超期还书处理
            if (loan.DueDate < loan.ReturnDate)
            {
                // span.Days VS span.TotalDays : 前者是整数，后者带小数
                TimeSpan span = loan.ReturnDate.Value - loan.DueDate;
                int spanDays = span.Days;
                var fine = new Fine
                {
                    Amount = 0.5m * spanDays,
                    Reason = "超期还书",
                    LoanId = loan.Id,
                    UserId = loan.UserId
                };
                _context.Fines.Add(fine);
            }

            // 更新书籍的可用数量
            loan.Book.Available += 1;
            await _context.SaveChangesAsync();
            return OPERATION_SUCCESS;
        }

        public async Task<int> AddBookAsync(EditBookDTO bookDTO)
        {
            var author = await _context.Authors.FindAsync(bookDTO.AuthorId);
            if (author == null)
            {
                throw new ArgumentException(AUTHOR_NOT_FOUND);
            }

            var publisher = await _context.Publishers.FindAsync(bookDTO.PublisherId);
            if (publisher == null)
            {
                throw new ArgumentException(PUBLISHER_NOT_FOUND);
            }

            if (await _context.Books.FirstOrDefaultAsync(b => b.Isbn == bookDTO.Isbn) != null)
            {
                throw new ArgumentException(ISBN_ALREADY_EXISTS);
            }

            if (bookDTO.CategoryIds.Count == 0)
            {
                throw new ArgumentException(ONE_CATEGORY_NEED);
            }

            // 验证传入的所有分类ID是否存在
            List<Category> existingCategories = await _context.Categories
                .Where(c => bookDTO.CategoryIds.Contains(c.Id))
                .ToListAsync();
            if (existingCategories.Count != bookDTO.CategoryIds.Count)
            {
                // 找出不存在的类别ID
                List<int> nonExistentCategoryIds = bookDTO.CategoryIds
                    .Except(existingCategories.Select(c => c.Id))
                    .ToList();
                throw new ArgumentException($"部分或全部书籍类别不存在：{string.Join(", ", nonExistentCategoryIds)}");
            }

            Book book = bookDTO.Adapt<Book>();
            _context.Books.Add(book);

            // 添加书籍类别和书籍的关联(处理联结表BookCategory)
            // 保存book后会获得数据库生成的ID，在SaveChanges()之前,可以建立BookCategory的关系
            foreach (Category category in existingCategories)
            {
                book.BookCategories.Add(new BookCategory
                {
                    Category = category,
                });
            }

            await _context.SaveChangesAsync();
            return book.Id;
        }

        public async Task UpdateBookAsync(int id, EditBookDTO bookDTO)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }
            
            var existingBook = await _context.Books.FirstOrDefaultAsync(b => b.Isbn == bookDTO.Isbn && b.Id != id);
            if (existingBook != null)
            {
                throw new ArgumentException(ISBN_ALREADY_EXISTS);
            }

            var author = await _context.Authors.FindAsync(bookDTO.AuthorId);
            if (author == null)
            {
                throw new ArgumentException(AUTHOR_NOT_FOUND);
            }

            var publisher = await _context.Publishers.FindAsync(bookDTO.PublisherId);
            if (publisher == null)
            {
                throw new ArgumentException(PUBLISHER_NOT_FOUND);
            }

            if (bookDTO.Available > bookDTO.Stock)
            {
                throw new ArgumentException(AVAILABLE_EXCEEDS_STOCK);
            }

            // 更新书籍属性
            bookDTO.Adapt(book);

            // 获取当前书籍的类别关联
            List<BookCategory> currentBookCategories = await _context.BookCategories
                .Where(bc => bc.BookId == book.Id)
                .ToListAsync();
            // 删除不再需要的图书的分类
            foreach (BookCategory existing in currentBookCategories)
            {
                if (!bookDTO.CategoryIds.Contains(existing.CategoryId))
                {
                    _context.BookCategories.Remove(existing);
                }
            }
            // 添加新增的分类
            foreach (int categoryId in bookDTO.CategoryIds)
            {
                if (currentBookCategories.All(bc => bc.CategoryId != categoryId))
                {
                    BookCategory bookCategory = new BookCategory
                    {
                        BookId = book.Id,
                        CategoryId = categoryId
                    };
                    _context.BookCategories.Add(bookCategory);
                }
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBooksAsync(List<int> ids)
        {
            List<Book> books = await _context.Books.Where(book => ids.Contains(book.Id)).ToListAsync();
            if (books.Count == 0)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }
            _context.Books.RemoveRange(books);
            await _context.SaveChangesAsync();
        }
    }
}