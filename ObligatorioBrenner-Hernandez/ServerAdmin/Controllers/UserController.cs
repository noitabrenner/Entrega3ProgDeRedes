using Business.Domain;
using Grpc.Net.Client;
using GrpcGreeterClient;
using Microsoft.AspNetCore.Mvc;
using ServerAdmin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServerAdmin.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController : ControllerBase
    {
        private static GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5001");
        private static Greeter.GreeterClient user = new Greeter.GreeterClient(channel);

        [HttpPost]
        public async Task<string> AddUser([FromBody] User userBody)
        {

            Reply reply = await user.CreateUserAsync(
               new RequestUser()
               {
                   Username = userBody.Username,
                   Name = userBody.Name,
                   Password = userBody.Password,
                   NameBy = "admin"

               });
            return reply.Message;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            //falta
            return null;
        }

        [HttpGet("{userId}")]
        public async Task<string> GetUser(int id)
        {
            Reply reply = await user.GetUserAsync(
                new RequestUserId()
                {
                    UserId = id,
                    Name = "admin"
                });
            return reply.Message;
        }

        [HttpPut("{userId}")]
        public async Task<string> UpdateUser(int id, [FromBody] User userBody)
        {
            Reply reply = await user.ModifyUserAsync(
               new RequestUser()
               {
                   Id = id,
                   Username = userBody.Username,
                   Name = userBody.Name,
                   Password = userBody.Password,
                   NameBy = "admin"

               });
            return reply.Message;
        }

        [HttpDelete("{userId}")]
        public async Task<string> Delete(int userId)
        {

            Reply reply = await user.DeleteUserAsync(
           new RequestId()
           {
               Info = userId,
               Name = "admin"
           });
            return reply.Message;
        }
    }
}
