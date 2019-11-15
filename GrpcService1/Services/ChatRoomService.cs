using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using GrpcServices1;

namespace GrpcService1.Services
{
    public class ChatRoomService: IChatRoomService
    {
        private ConcurrentDictionary<string, IServerStreamWriter<ChatRequest>> users = new ConcurrentDictionary<string, IServerStreamWriter<ChatRequest>>();

        public void Join(string name, IServerStreamWriter<ChatRequest> response) => users.TryAdd(name, response);

        public void Remove(string name) => users.TryRemove(name, out var s);

        public async Task BroadcastMessageAsync(ChatRequest message) => await BroadcastMessages(message);

        private async Task BroadcastMessages(ChatRequest message)
        {
            foreach (var user in users.Where(x => x.Key != message.Name))
            {
                var item = await SendMessageToSubscriber(user, message);
                if (item != null)
                {
                    Remove(item?.Key);
                };
            }
        }

        private async Task<Nullable<KeyValuePair<string, IServerStreamWriter<ChatRequest>>>> SendMessageToSubscriber(KeyValuePair<string, IServerStreamWriter<ChatRequest>> user, ChatRequest message)
        {
            try
            {
                await user.Value.WriteAsync(message);
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return user;
            }
        }
    }
}
