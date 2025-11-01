using System;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MessageRepository(AppDbContext context) : IMessageRepository
{
    public void AddMessage(Message message)
    {
        context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        context.Messages.Remove(message);
    }

    public async Task<Message?> GetMessage(string MessageId)
    {
        return await context.Messages.FindAsync(MessageId);
    }

    public async Task<PaginatedResults<MessageDTO>> GetMessagesForMember(MessageParams
         messageParams)
    {
        var query = context.Messages.AsQueryable();
        query = messageParams.Container switch
        {
            "Outbox" => query.Where(m => m.SenderId == messageParams.MemberId && m.SenderDeleted == false),
            _ => query.Where(m => m.RecipientId == messageParams.MemberId), 
            
        };

        var messageQuery = query.Select(MessageExtensions.ToDtoProjection());

        return await PaginationHelper.CreateAsync<MessageDTO>(messageQuery,
            messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDTO>> GetMessageThread(string currentMemberId, string recipientMemberId)
    {
        await context.Messages.Where(x => x.RecipientId == currentMemberId && x.DateRead == null && x.SenderId == recipientMemberId)
        .ExecuteUpdateAsync(setters => setters.SetProperty(m => m.DateRead, DateTime.UtcNow));

        return await context.Messages
            .Where(m => (m.RecipientId == currentMemberId && m.RecipientDeleted==false && m.SenderId == recipientMemberId)
                     || (m.RecipientId == recipientMemberId && m.SenderDeleted== false &&m.SenderId == currentMemberId))
            .OrderBy(m => m.MessageSent)
            .Select(MessageExtensions.ToDtoProjection())
            .ToListAsync(); //messages between two members
    }
        

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
