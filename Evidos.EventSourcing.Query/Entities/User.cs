using System;
using Evidos.EventSourcing.Query.Abstractions;

namespace Evidos.EventSourcing.Query.Entities
{
    public class User : IEntity
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public  string Password { get; set; }
        public  string Status { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
}
