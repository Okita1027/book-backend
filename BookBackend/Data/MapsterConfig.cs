﻿using Mapster;
using book_backend.Models.DTO;
using book_backend.Models.Entity;
using Masuit.Tools.Security;
using book_backend.Models.VO;

namespace book_backend.Data
{
    public static class MapsterConfig
    {
        public static void Configure()
        {
            // 全局配置
            TypeAdapterConfig.GlobalSettings.Default
                .NameMatchingStrategy(NameMatchingStrategy.Flexible) // 支持更灵活的命名匹配
                .IgnoreNullValues(true); // 所有映射都忽略 null 值


            // User 映射
            TypeAdapterConfig<EditUserDTO, User>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Email, src => src.Email)
                .Map(dest => dest.PasswordHash, src => src.Password.MDString())
                .IgnoreNullValues(true);
            
            // Category 映射
            TypeAdapterConfig<EditCategoryDTO, Category>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .IgnoreNullValues(true);
            
            // Publisher 映射
            TypeAdapterConfig<EditPublisherDTO, Publisher>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .IgnoreNullValues(true);

            // Author 映射
            TypeAdapterConfig<EditAuthorDTO, Author>.NewConfig()
                .Map(dest => dest.Name, src => src.Name)
                .Map(dest => dest.Biography, src => src.Biography)
                .IgnoreNullValues(true);

            // Book <= BookDTO 映射
            TypeAdapterConfig<EditBookDTO, Book>.NewConfig()
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Isbn, src => src.Isbn)
                .Map(dest => dest.PublishedDate, src => src.PublishedDate)
                .Map(dest => dest.Stock, src => src.Stock)
                .Map(dest => dest.Available, src => src.Available)
                .Map(dest => dest.AuthorId, src => src.AuthorId)
                .Map(dest => dest.PublisherId, src => src.PublisherId)
                .IgnoreNullValues(true);

            // Book => BookVO 映射
            TypeAdapterConfig<Book, BookVO>.NewConfig()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.Title, src => src.Title)
                .Map(dest => dest.Isbn, src => src.Isbn)
                .Map(dest => dest.Stock, src => src.Stock)
                .Map(dest => dest.Available, src => src.Available)
                .Map(dest => dest.AuthorName, src => src.Author.Name)
                .Map(dest => dest.AuthorId, src => src.Author.Id)
                .Map(dest => dest.PublisherName, src => src.Publisher.Name)
                .Map(dest => dest.PublisherId, src => src.Publisher.Id)
                .Map(dest => dest.PublishedDate, src => src.PublishedDate)
                .Map(dest => dest.CategoryIds, src => src.BookCategories.Select(bc => bc.Category.Id).ToList())
                .Map(dest => dest.CategoryNames, src => src.BookCategories.Select(bc => bc.Category.Name).ToList())
                .IgnoreNullValues(true);
            
            // Loan => LoanVO 映射
            TypeAdapterConfig<Loan, LoanVO>.NewConfig()
                .Map(dest => dest.Title, src => src.Book.Title)
                .Map(dest => dest.Username, src => src.User.Name)
                .Map(dest => dest.LoanDate, src => src.LoanDate)
                .Map(dest => dest.DueDate, src => src.DueDate)
                .Map(dest => dest.ReturnDate, src => src.ReturnDate)
                .IgnoreNullValues(true);

            // Fine => FineVO 映射
            TypeAdapterConfig<Fine, FineVO>.NewConfig()
                .Map(dest => dest.LoadId, src => src.Loan.Id)
                .Map(dest => dest.UserId, src => src.User.Id)
                .Map(dest => dest.Username, src => src.User.Name)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.Reason, src => src.Reason)
                .Map(dest => dest.PaidDate, src => src.PaidDate)
                .Map(dest => dest.CreatedTime, src => src.CreatedTime)
                .IgnoreNullValues(true);
        }
    }
}