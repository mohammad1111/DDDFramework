using Gig.Framework.Application;
using Gig.Framework.Core.DataProviders;
using Gig.Sample.Write.Applications.Application.Contracts.Product;
using Gig.Sample.Write.Domains.Domain.Contract.Events;
using Gig.Sample.Write.Domains.Domain.Product.Services;
using System;
using System.Threading.Tasks;

namespace Gig.Sample.Write.Applications.Application.Product
{
    public class ProductCommandHandler : CommandHandler,
        ICommandHandlerAsync<CreateProductCommand>,
        ICommandHandlerAsync<UpdateProductCommand>,
        ICommandHandlerAsync<RemoveProductCommand>
    {

        private readonly IProductRepository _repository;
        public ProductCommandHandler(IUnitOfWork uow, IProductRepository repository) : base(uow)
        {
            _repository = repository;
        }
        public async Task HandleAsync(CreateProductCommand command)
        {
            var theProduct = new Domains.Domain.Product.Product()
            {
                 Title = command.Title,
                 Description=command.Description,
                 Count = command.Count,
                 UserId = command.UserId,
                 Price = command.Price,
                 HasUsed = command.HasUsed
            };
            theProduct.AddEvent(new ProductCreatedEvent());
            await _repository.AddAsync(theProduct);
        }

        public async Task HandleAsync(UpdateProductCommand command)
        {
            var theProduct = await _repository.GetByIdAsync(command.Id);
            theProduct.ChangeCount(command.Count);
            theProduct.ChangeDescription(command.Description);
            theProduct.ChangeHasUsed(command.HasUsed);
            theProduct.ChangeTitle(command.Title);
            theProduct.ChangePrice(command.Price);
            theProduct.ChangeUserId(command.UserId);
        }

        public async Task HandleAsync(RemoveProductCommand command)
        {          
            await _repository.RemoveAsync<Domains.Domain.Product.Product>(command.Id);
        }
    }
}
