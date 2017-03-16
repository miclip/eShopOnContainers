﻿using Autofac;
using Microsoft.eShopOnContainers.Services.Ordering.API.Application.Queries;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.BuyerAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Domain.AggregatesModel.OrderAggregate;
using Microsoft.eShopOnContainers.Services.Ordering.Infrastructure.Repositories;

namespace Microsoft.eShopOnContainers.Services.Ordering.API.Infrastructure.AutofacModules
{

    public class ApplicationModule
        :Autofac.Module
    {

       

        public ApplicationModule()
        {
            

        }

        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterType<OrderQueries>()
                .As<IOrderQueries>()
                .InstancePerLifetimeScope();

            builder.RegisterType<BuyerRepository>()
                .As<IBuyerRepository<Buyer>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<OrderRepository>()
                .As<IOrderRepository<Order>>()
                .InstancePerLifetimeScope();

            builder.RegisterType<RequestManager>()
               .As<IRequestManager>()
               .InstancePerLifetimeScope();
        }
    }
}
