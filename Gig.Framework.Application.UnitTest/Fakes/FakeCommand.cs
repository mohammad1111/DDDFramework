using System;

namespace Gig.Framework.Application.UnitTest.Fakes
{
    public class FakeCommand : ICommand
    {
        public Guid Id { get; set; }
    }
}