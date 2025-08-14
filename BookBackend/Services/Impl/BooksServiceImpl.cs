using book_backend.Data;
using book_backend.Exceptions;
using book_backend.Models.DTO;
using book_backend.Models.Entity;
using book_backend.Models.VO;
using book_backend.utils;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Serilog.Core;
using static book_backend.Constants.IServiceConstants;
using ILogger = Serilog.ILogger;

namespace book_backend.Services.Impl
{
    public class BooksServiceImpl(ApplicationDbContext context, ILogger logger) : IBooksService
    {
        public async Task<List<RawBookVO>> GetAllRawBooksAsync()
        {
            // 第一步：执行一个扁平化查询，类似于你提供的 SQL 语句
            // EF Core 会生成 JOIN 语句来获取所有关联数据
            var flatQuery = from b in context.Books
                join a in context.Authors on b.AuthorId equals a.Id
                join p in context.Publishers on b.PublisherId equals p.Id
                from bc in context.BookCategories.Where(bc => bc.BookId == b.Id).DefaultIfEmpty()
                from c in context.Categories.Where(c => bc != null && c.Id == bc.CategoryId).DefaultIfEmpty()
                select new
                {
                    Book = b,
                    AuthorName = a.Name,
                    PublisherName = p.Name,
                    CategoryId = c != null ? c.Id : (int?)null,
                    CategoryName = c != null ? c.Name : null
                };

            // 第二步：使用 GroupBy 将扁平化结果按书籍进行分组
            // EF Core 可以在数据库中执行到这里
            var groupedBooks = await flatQuery
                .GroupBy(x => new
                {
                    x.Book.Id,
                    x.Book.Title,
                    x.Book.Isbn,
                    x.Book.PublishedDate,
                    x.Book.Stock,
                    x.Book.Available,
                    x.Book.CreatedTime,
                    x.Book.UpdatedTime,
                    x.Book.AuthorId,
                    x.AuthorName,
                    x.Book.PublisherId,
                    x.PublisherName
                })
                // 关键步骤：在这里将数据加载到内存中
                .ToListAsync();

            // 关键步骤：在 C# 内存中处理 Dictionary
            var finalResult = groupedBooks
                .Select(g => new RawBookVO
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    Isbn = g.Key.Isbn,
                    PublishedDate = g.Key.PublishedDate,
                    Stock = g.Key.Stock,
                    Available = g.Key.Available,
                    CreatedTime = g.Key.CreatedTime,
                    UpdatedTime = g.Key.UpdatedTime,
        
                    AuthorId = g.Key.AuthorId,
                    AuthorName = g.Key.AuthorName,

                    PublisherId = g.Key.PublisherId,
                    PublisherName = g.Key.PublisherName,

                    // 可以在内存中安全地调用 ToDictionary
                    CategoryDictionary = g.Where(x => x.CategoryId.HasValue)
                        .ToDictionary(
                            x => x.CategoryId.Value,
                            x => x.CategoryName
                        )
                })
                .ToList();

            return finalResult;
        }

        public async Task<IEnumerable<BookVO>> GetAllBooksAsync()
        {
            return await context.Books
                // 加载关联的 Author 和 Publisher
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                // 将原本的 Book 实体映射为 BookVO
                .ProjectToType<BookVO>()
                .ToListAsync();
        }

        public async Task<BookVO?> GetBookByIdAsync(int id)
        {
            return await context.Books
                // 同时加载关联的 Author 、Publisher
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                // 将原本的 Book 实体映射为 BookVO
                .ProjectToType<BookVO>()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BookVO>> SearchBooksAsync(string title, string isbn, string categoryName,
            string authorName,
            string publisherName, DateTime? publishedDateBegin, DateTime? publishedDateEnd)
        {
            // 获取所有书籍、加载关联的 Author、Publisher和BookCategory,然后转为LINQ查询
            var query = context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
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

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(b => b.BookCategories.Any(bc => bc.Category.Name.Contains(categoryName)));
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

            var bookVos = await query.ProjectToType<BookVO>().ToListAsync();
            return bookVos;
        }

        public async Task<Pagination<BookVO>> SearchBooksPaginatedAsync(PaginationRequest paginationRequest, string? title, string? isbn, string? categoryName,
            string? authorName, string? publisherName, DateTime? publishedDateBegin, DateTime? publishedDateEnd)
        {
            // 添加日志记录传入的参数
            logger.Information("Search parameters - Title: {Title}, ISBN: {Isbn}, Category: {CategoryName}, Author: {AuthorName}, Publisher: {PublisherName}, DateBegin: {DateBegin}, DateEnd: {DateEnd}",
                title, isbn, categoryName, authorName, publisherName, publishedDateBegin, publishedDateEnd);

            
            // 获取所有书籍、加载关联的 Author、Publisher和BookCategory,然后转为LINQ查询
            var query = context.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .Include(b => b.BookCategories)
                .ThenInclude(bc => bc.Category)
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

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(b => b.BookCategories.Any(bc => bc.Category.Name.Contains(categoryName)));
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

            // 排序逻辑
            if (!string.IsNullOrEmpty(paginationRequest.SortField))
            {
                query = paginationRequest.SortField.ToLower() switch
                {
                    "title" => paginationRequest.SortOrder?.ToLower() == "descend" 
                        ? query.OrderByDescending(b => b.Title) 
                        : query.OrderBy(b => b.Title),
                    "isbn" => paginationRequest.SortOrder?.ToLower() == "descend" 
                        ? query.OrderByDescending(b => b.Isbn) 
                        : query.OrderBy(b => b.Isbn),
                    "publisheddate" => paginationRequest.SortOrder?.ToLower() == "descend" 
                        ? query.OrderByDescending(b => b.PublishedDate) 
                        : query.OrderBy(b => b.PublishedDate),
                    "stock" => paginationRequest.SortOrder?.ToLower() == "descend" 
                        ? query.OrderByDescending(b => b.Stock) 
                        : query.OrderBy(b => b.Stock),
                    "available" => paginationRequest.SortOrder?.ToLower() == "descend" 
                        ? query.OrderByDescending(b => b.Available) 
                        : query.OrderBy(b => b.Available),
                    "authorname" => paginationRequest.SortOrder?.ToLower() == "descend" 
                        ? query.OrderByDescending(b => b.Author.Name) 
                        : query.OrderBy(b => b.Author.Name),
                    "publishername" => paginationRequest.SortOrder?.ToLower() == "descend" 
                        ? query.OrderByDescending(b => b.Publisher.Name) 
                        : query.OrderBy(b => b.Publisher.Name),
                    _ => query.OrderBy(b => b.Id)
                };
            }
            else
            {
                // 默认按ID排序
                query = query.OrderBy(b => b.Id);
            }

            var paginatedBookVOs = await query.ProjectToType<BookVO>()
                .ToPaginatedListAsync(paginationRequest.PageIndex, paginationRequest.PageSize);
            return paginatedBookVOs;
        }

        public async Task<string> LoanBookAsync(int id, string username)
        {
            var book = await context.Books.FindAsync(id);
            if (book == null)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }

            if (book.Available <= 0)
            {
                throw new BusinessException(NOT_ENOUGH_AVAILABLE);
            }

            var user = await context.Users.FirstOrDefaultAsync(u => u.Name == username);
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
            context.Loans.Add(loan);
            await context.SaveChangesAsync();
            return $"书籍 {book.Title} 已借出给 {username}。";
        }

        public async Task<string> ReturnBookAsync(int id, string username)
        {
            var loan = await context.Loans
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
                context.Fines.Add(fine);
            }

            // 更新书籍的可用数量
            loan.Book.Available += 1;
            await context.SaveChangesAsync();
            return OPERATION_SUCCESS;
        }

        public async Task<int> AddBookAsync(EditBookDTO bookDTO)
        {
            var author = await context.Authors.FindAsync(bookDTO.AuthorId);
            if (author == null)
            {
                throw new ArgumentException(AUTHOR_NOT_FOUND);
            }

            var publisher = await context.Publishers.FindAsync(bookDTO.PublisherId);
            if (publisher == null)
            {
                throw new ArgumentException(PUBLISHER_NOT_FOUND);
            }

            if (await context.Books.FirstOrDefaultAsync(b => b.Isbn == bookDTO.Isbn) != null)
            {
                throw new ArgumentException(ISBN_ALREADY_EXISTS);
            }

            if (bookDTO.CategoryIds.Count == 0)
            {
                throw new ArgumentException(ONE_CATEGORY_NEED);
            }

            // 验证传入的所有分类ID是否存在
            List<Category> existingCategories = await context.Categories
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
            book.Available = bookDTO.Stock;
            context.Books.Add(book);

            // 添加书籍类别和书籍的关联(处理联结表BookCategory)
            // 保存book后会获得数据库生成的ID，在SaveChanges()之前,可以建立BookCategory的关系
            foreach (Category category in existingCategories)
            {
                book.BookCategories.Add(new BookCategory
                {
                    Category = category,
                });
            }

            await context.SaveChangesAsync();
            return book.Id;
        }

        public async Task UpdateBookAsync(int id, EditBookDTO bookDTO)
        {
            var book = await context.Books.FindAsync(id);
            if (book == null)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }

            var existingBook = await context.Books.FirstOrDefaultAsync(b => b.Isbn == bookDTO.Isbn && b.Id != id);
            if (existingBook != null)
            {
                throw new ArgumentException(ISBN_ALREADY_EXISTS);
            }

            var author = await context.Authors.FindAsync(bookDTO.AuthorId);
            if (author == null)
            {
                throw new ArgumentException(AUTHOR_NOT_FOUND);
            }

            var publisher = await context.Publishers.FindAsync(bookDTO.PublisherId);
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
            List<BookCategory> currentBookCategories = await context.BookCategories
                .Where(bc => bc.BookId == book.Id)
                .ToListAsync();
            // 删除不再需要的图书的分类
            foreach (BookCategory existing in currentBookCategories)
            {
                if (!bookDTO.CategoryIds.Contains(existing.CategoryId))
                {
                    context.BookCategories.Remove(existing);
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
                    context.BookCategories.Add(bookCategory);
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task DeleteBookAsync(int id)
        {
            var book = await context.Books.FindAsync(id);
            if (book == null)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }

            context.Books.Remove(book);
            await context.SaveChangesAsync();
        }


        public async Task DeleteBooksAsync(List<int> ids)
        {
            List<Book> books = await context.Books.Where(book => ids.Contains(book.Id)).ToListAsync();
            if (books.Count == 0)
            {
                throw new ArgumentException(BOOK_NOT_FOUND);
            }

            context.Books.RemoveRange(books);
            await context.SaveChangesAsync();
        }
    }
}