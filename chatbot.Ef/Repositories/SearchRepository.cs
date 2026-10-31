using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using chatbot.Core.DTOs;
using chatbot.Core.DTOs.Search;
using chatbot.Core.Enums;
using chatbot.Core.Interfaces.Repositories;
using chatbot.Core.Models;
using chatbot.Ef.Data;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace chatbot.Ef.Repositories
{
    public class SearchRepository(ApplicationDbContext context) : ISearchRepository
    {
        public async Task<PagedResultDto<SearchResultDto>> SearchConversationsAsync(Guid currentUserId, SearchRequestDto request)
        {
            var keyword = request.Keyword.Trim();

            var query = context.Conversations
                .AsNoTracking()
                .Where(c =>
                    !c.IsDeleted &&
                    c.Members.Any(m =>
                        m.UserId == currentUserId &&
                        m.LeftAt == null &&
                        !m.IsBanned) &&
                    (
                        (c.Title != null &&
                         c.Title.Contains(keyword)) ||

                        (c.Description != null &&
                         c.Description.Contains(keyword))
                    ));

            var totalCount = await query.CountAsync();
            var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new SearchResultDto
            {
                Type = SearchType.Conversations,
                Id = c.Id,
                ConversationId = c.Id,
                Title = c.Title ?? string.Empty,
                Description = c.Description,
                ImageUrl = c.Image != null ? c.Image.FileUrl : null,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync();

            return CreatePagedResult(items,request,totalCount);
        }

        public async Task<PagedResultDto<SearchResultDto>> SearchFilesAsync(Guid currentUserId, SearchRequestDto request)
        {
            var keyword = request.Keyword.Trim();

            var query = context.MessageAttachments
                .AsNoTracking()
                .Where(x =>
                    !x.Message.Deletions.Any(d => d.DeletedAt < DateTime.UtcNow) &&
                    !x.Message.IsDeletedForEveryone &&
                    x.Message.Conversation.Members.Any(member =>
                        member.UserId == currentUserId &&
                        member.LeftAt == null &&
                        !member.IsBanned) &&
                    x.StoredFile.OriginalFileName.Contains(keyword)
                );
            if (request.ConversationId.HasValue)
            {
                query = query.Where(x =>
                    x.Message.ConversationId ==
                    request.ConversationId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.Message.SendAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new SearchResultDto
                {
                    Type = SearchType.Files,
                    Id = x.StoredFileId,
                    ConversationId = x.Message.ConversationId,
                    Title =x.StoredFile.OriginalFileName,
                    Description =x.StoredFile.ContentType,
                    ImageUrl = x.StoredFile.FileUrl,
                    CreatedAt =x.Message.SendAt
                })
                .ToListAsync();

            return CreatePagedResult(items,request,totalCount);
        }

        public async Task<PagedResultDto<SearchResultDto>> SearchGroupsAsync(Guid currentUserId, SearchRequestDto request)
        {
            var keyword = request.Keyword.Trim();

            var query = context.Conversations
                .AsNoTracking()
                .Where(c =>
                    c.Type == ConversationType.Group &&
                    !c.IsDeleted &&
                    c.Members.Any(m =>
                        m.UserId == currentUserId &&
                        m.LeftAt == null &&
                        !m.IsBanned) &&

                    (
                        (c.Title != null &&
                         c.Title.Contains(keyword)) ||

                        (c.Description != null &&
                         c.Description.Contains(keyword))
                    ));

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(c => c.Title)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(c => new SearchResultDto
                {
                    Type = SearchType.Groups,
                    Id = c.Id,
                    ConversationId = c.Id,
                    Title = c.Title ?? string.Empty,
                    Description = c.Description,
                    ImageUrl = c.Image != null ? c.Image.FileUrl : null,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return CreatePagedResult(items,request, totalCount);
        }

        public async Task<PagedResultDto<MessageSearchResultDto>> SearchMessagesAsync(Guid userId, MessageSearchQuery request)
        {
            var keyword = request.Keyword.Trim();
            var query = context.Messages
                .AsNoTracking()
                .Where(m =>
                    !m.Deletions.Any(d => d.DeletedAt < DateTime.UtcNow) &&
                    !m.IsDeletedForEveryone &&
                    m.Content != null &&
                    m.Content.Contains(keyword) &&
                    m.Conversation.Members.Any(member =>
                        member.UserId == userId &&
                        member.LeftAt == null &&
                        !member.IsBanned));


            if (request.ConversationId.HasValue)
            {
                query = query.Where(m => m.ConversationId == request.ConversationId.Value);
            }


            if (request.SenderId.HasValue)
            {
                query = query.Where(m => m.SenderId == request.SenderId.Value);
            }


            if (request.MessageType.HasValue)
            {
                query = query.Where(m => m.MessageType == request.MessageType.Value);
            }


            if (request.From.HasValue)
            {
                query = query.Where(m => m.SendAt >= request.From.Value);
            }


            if (request.To.HasValue)
            {
                query = query.Where(m => m.SendAt < request.To.Value);
            }


            var totalCount = await query.CountAsync();


            var items = await query
                .OrderByDescending(m => m.SendAt)
                .Skip(
                    (request.Page - 1)
                    * request.PageSize)
                .Take(request.PageSize)
                .Select(m =>
                    new MessageSearchResultDto
                    {
                        MessageId = m.Id,
                        ConversationId = m.ConversationId,
                        SenderId = m.SenderId,
                        SenderName = m.Sender.UserName,
                        Content = m.Content,
                        MessageType = m.MessageType,
                        CreatedAt = m.SendAt
                    })

                .ToListAsync();


            return new PagedResultDto<MessageSearchResultDto>
            {
                Items = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }

        public async Task<PagedResultDto<SearchResultDto>> SearchUsersAsync(Guid currentUserId, SearchRequestDto request)
        {
            var keyword = request.Keyword.Trim();

            var query = context.Users
                .AsNoTracking()
                .Where(u =>
                    u.Id != currentUserId &&
                    (
                        (u.UserName != null &&
                         u.UserName.Contains(keyword)) ||

                        (u.Email != null &&
                         u.Email.Contains(keyword))
                ));

            var totalCount = await query.CountAsync();

            var items = await query
            .OrderBy(u => u.UserName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new SearchResultDto
            {
                Type = SearchType.Users,
                Id = u.Id,
                Title = u.UserName ?? string.Empty,
                ImageUrl = u.ProfileImage != null
                    ? u.ProfileImage.FileUrl
                    : null
            })
            .ToListAsync();

            return CreatePagedResult(items,request,totalCount);
        }

        private static PagedResultDto<SearchResultDto> CreatePagedResult(List<SearchResultDto> items, SearchRequestDto request,int totalCount)
        {
            return new PagedResultDto<SearchResultDto>
            {
                Items = items,
                PageNumber = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            };
        }
    }
}
