using Gig.Framework.Domain;
using System;

namespace Gig.Sample.Write.Domains.Domain.User
{
    public class User : AggregateRoot
    {
        public User()
        {

        }

        public User(Guid id) : base(id)
        {

        }

        public User(long code, string firstName, string lastName, string userName, string email, bool hasAcitve)
        {
            Code = code;
            FirstName = firstName;
            LastName = lastName;
            UserName = userName;
            Email = email;
            HasAcitve = hasAcitve;
        }
        public long Code { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public bool HasAcitve { get; set; }

        public void ChangeCode(long code)
        {
            Code = Code;
        }
        public void ChangeFirstName(string firstName)
        {
            FirstName = firstName;
        }
        public void ChangeLastName(string lastName)
        {
            LastName = lastName;
        }
        public void ChangeUserName(string userName)
        {
            UserName = userName;
        }
        public void ChangeEmail(string email)
        {
            Email = email;
        }
        public void ChangeHasAcitve(bool hasAcitve)
        {
            HasAcitve = hasAcitve;
        }
    }
}
