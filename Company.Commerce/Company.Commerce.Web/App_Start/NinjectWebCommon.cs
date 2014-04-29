[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Company.Commerce.Web.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethodAttribute(typeof(Company.Commerce.Web.App_Start.NinjectWebCommon), "Stop")]

namespace Company.Commerce.Web.App_Start
{
    using System;
    using System.Web;

    using Microsoft.Web.Infrastructure.DynamicModuleHelper;

    using Ninject;
    using Ninject.Web.Common;
    using Company.Commerce.Service;
    using Company.Commerce.Repository;
    using Company.Commerce.Data;
    using Company.Commerce.Data.EntityFramework.Context;
    using System.IO;
    using System.Data.Entity;
    using Company.Commerce.Data.EntityFramework.Context.Initializers;
    using Company.Commerce.Service.Email;

    public static class NinjectWebCommon
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start()
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }

        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            try
            {
                kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
                kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();

                RegisterServices(kernel);
                return kernel;
            }
            catch
            {
                kernel.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ICategoryService>().To<CategoryService>().InRequestScope();
            kernel.Bind<IDbContext>().To<MainContext>().InRequestScope();
            kernel.Bind<IEmailService>().To<EmailService>().InSingletonScope();
            kernel.Bind<IImageService>().To<ImageService>().InRequestScope();
            kernel.Bind<IOrderService>().To<OrderService>().InRequestScope();
            kernel.Bind<IProductService>().To<ProductService>().InRequestScope();
            kernel.Bind<IShippingService>().To<ShippingService>().InSingletonScope();
            kernel.Bind<IShoppingCartService>().To<ShoppingCartService>().InRequestScope();
            kernel.Bind<IUnitOfWork>().To<EfUnitOfWork>().InRequestScope();
            kernel.Bind<IUserService>().To<UserService>().InRequestScope();
        }
    }
}
