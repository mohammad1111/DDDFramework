using Gig.Sample.Write.Domains.Domain.Product.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Gig.Sample.Write.Domains.Domain.Service.Product
{
    public class ProductDomainService : IProductDomainService
    {
        private readonly IProductRepository _productRepository;
        public ProductDomainService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
    }

}
