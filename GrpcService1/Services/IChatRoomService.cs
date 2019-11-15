using Grpc.Core;
using GrpcServices1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcService1.Services
{
    public interface IChatRoomService
    {
        public void Join(string name, IServerStreamWriter<ChatRequest> response);
        public void Remove(string name);
        public  Task BroadcastMessageAsync(ChatRequest message);

    }
}
