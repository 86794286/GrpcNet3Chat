using Grpc.Core;
using Grpc.Net.Client;
using GrpcServices2;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var ChatClient2 = new Chat.ChatClient(channel);
            var chat = ChatClient2.join();
            string Name = "";
            if (Name.Equals(""))
            {
                Console.WriteLine("输入用户名：");
                Name = Console.ReadLine();
            }
            Console.WriteLine("输入消息：");
            string Message = "";
            Message = Console.ReadLine();
            
            //定义接收响应逻辑
            /*_ = Task.Run(async () =>
            {
                while (await chat.ResponseStream.MoveNext(cancellationToken: CancellationToken.None))
                {
                    var response = chat.ResponseStream.Current;
                    Console.WriteLine($"{response.Message}");
                }
                await foreach (var resp in chat.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine(resp.Message);
                }
            });

            //await chat.RequestStream.WriteAsync(new ChatRequest { Name = Name, Message = $"{Name} has joined the room" });

            string line;
            while ((line = Console.ReadLine()) != null)
            {
                if (line.ToLower() == "bye")
                {
                    break;
                }
                await chat.RequestStream.WriteAsync(new ChatRequest { Name = Name, Message = line });
            }
            await chat.RequestStream.CompleteAsync();*/

            await SendMessage(channel, Name, Message);

            Console.ReadKey();
        }

        private static async Task SendMessage(GrpcChannel channel, string Name, string Message)
        {

            var ChatClient2 = new Chat.ChatClient(channel);
            var test2 = ChatClient2.ChatStram();

            //定义接收响应逻辑

            var ChatRespTask = Task.Run(async () =>
            {
                while (await test2.ResponseStream.MoveNext())
                {
                    var response = test2.ResponseStream.Current;
                    Console.WriteLine($"{response.Message}");
                }
                /*    await foreach (var resp in test2.ResponseStream.ReadAllAsync())
                    {
                        Console.WriteLine(resp.Message);
                    }*/
            });


            ChatRequest chatRequest = new ChatRequest() { Name = Name, Message = Message };
            //发送完毕
            await test2.RequestStream.WriteAsync(chatRequest);
            await test2.RequestStream.CompleteAsync();
            // Console.WriteLine(test2.ResponseStream.ReadAllAsync().GetAsyncEnumerator().Current.Message);

            //开始接收响应
            await ChatRespTask;
            //await Listening(channel);
            Message = Console.ReadLine();
            if (!Message.Equals("Bye"))
            {
                await SendMessage(channel, Name, Message);
            }
            else
            {
                Console.WriteLine("结束");
            }
        }
    }
}
