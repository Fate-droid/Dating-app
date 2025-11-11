using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class MessagesController(IMessageRepository messageRepository,
    IMemberRepository memberRepository) : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
    {
        var sender = await memberRepository.GetMemberByIDAsync(User.GetMemberId());
        var recipient = await memberRepository.GetMemberByIDAsync(createMessageDTO.RecipientId);

        if (recipient == null || sender == null || sender.Id == createMessageDTO.RecipientId)
            return BadRequest("Cannot send this message");

        var message = new Message
        {
            SenderId = sender.Id,
            RecipientId = recipient.Id,
            Content = createMessageDTO.Content
        };

        messageRepository.AddMessage(message);
        if (await messageRepository.SaveAllAsync()) return message.ToDto();
        return BadRequest("Failed to send message");
    }
}
