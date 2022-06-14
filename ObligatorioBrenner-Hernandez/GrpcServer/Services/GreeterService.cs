using Business.Domain;
using BusinessLogicInterface.Interfaces;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcServer
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IUserLogic _userLogic;
        public GreeterService(ILogger<GreeterService> logger, IUserLogic userLogic)
        {
            _userLogic = userLogic;
            _logger = logger;
        }

        public override Task<Reply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Reply
            {
                Message = "Hello " + request.Name
            });
        }
        public override Task<Reply> CreateUser(RequestUser request, ServerCallContext context)
        {
            User user = new User(request.Username, request.Name, request.Password);
            string log = request.Name + " - " + DateTime.Now.ToString("dd/MM/yyyy") + " - ";
            Reply reply = new Reply();
            reply.Message = _userLogic.AddUser(user);
            log += reply.Message;
           // this._logProducer.PublishMessage(log);

            return Task.FromResult(reply);
        }

        public override Task<Reply> DeleteUser(RequestId request, ServerCallContext context)
        {
            return Task.FromResult(new Reply
            {
                Message = _userLogic.DeleteUser(request.Info)
            });
        }

        public override Task<Reply> ModifyUser(RequestUser request, ServerCallContext context)
        {
            User userToModify = new User(request.Username, request.Name, request.Password);
            return Task.FromResult(new Reply
            {
                Message = _userLogic.UpdateUser(request.Id, userToModify)
            });
        }

        public override Task<Reply> GetUser(RequestUserId request, ServerCallContext context)
        {
            Reply reply = new Reply();
            User user = _userLogic.GetById(request.UserId);
            if (user == null)
            {
                reply.Message = "No hay usuario con ese id";
            }
            else
            {
                reply.Message = $"el usuario con el id buscado es: {user.Username}";
            }
            return Task.FromResult(reply);
        }
    }
}
