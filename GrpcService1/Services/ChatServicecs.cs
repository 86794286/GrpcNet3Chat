using Grpc.Core;
using GrpcServices1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcService1.Services
{
    public class ChatServicecs : Chat.ChatBase
    {
        private  ChatRoomService _chatroomService;

        public ChatServicecs(ChatRoomService chatroomService)
        {
            _chatroomService = chatroomService;
        }

        

        public override async Task ChatStram(IAsyncStreamReader<ChatRequest> requestStream, IServerStreamWriter<ChatResponse> responseStream, ServerCallContext context)
        {
            List<ChatRequest> chatRequests = new List<ChatRequest>();
            while (await requestStream.MoveNext())
            {
                ChatRequest chatRequest = new ChatRequest();
                chatRequest.Name = requestStream.Current.Name;
                chatRequest.Message = requestStream.Current.Message;
                chatRequests.Add(chatRequest);

            }
            for (int i = 0; i < chatRequests.Count; i++)
            {
                await responseStream.WriteAsync(new ChatResponse() { Name = chatRequests[i].Name, Message = chatRequests[i].Name + ": " + chatRequests[i].Message });
            }
        }

        public override async Task join(IAsyncStreamReader<ChatRequest> requestStream, IServerStreamWriter<ChatRequest> responseStream, ServerCallContext context)
        {
            if (!await requestStream.MoveNext()) return;

            do
            {
                _chatroomService.Join(requestStream.Current.Name, responseStream);
                await _chatroomService.BroadcastMessageAsync(requestStream.Current);
            } while (await requestStream.MoveNext());

            _chatroomService.Remove(context.Peer);
        }
    }
}
