using System;
using System.Collections.Generic;
using System.Net;
using Funq;
using ServiceStack.CacheAccess;
using ServiceStack.CacheAccess.Providers;
using ServiceStack.ServiceHost;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceInterface.Auth;
using ServiceStack.WebHost.Endpoints;
using System.Runtime.Serialization;
using System.EnterpriseServices;
using ServiceStack.ServiceClient.Web;

namespace ServiceStack.Hello
{
    /// Create the name of your Web Service (i.e. the Request DTO)
    [Authenticate]
    [DataContract]
    [Description("ServiceStack's Hello World web service.")]
    [RestService("/hello")] //Optional: Define an alternate REST-ful url for this service
    [RestService("/hello/{Name}")]
    [RestService("/hello/{Name*}")]
    public class Hello
    {
        [DataMember]
        public string Name { get; set; }
    }

    /// Define your Web Service response (i.e. Response DTO)
    [DataContract]
    public class HelloResponse
    {
        [DataMember]
        public string Result { get; set; }
    }

    /// Create your Web Service implementation 
    public class HelloService : IService<Hello>
    {
        [Authenticate]
        public object Execute(Hello request)
        {
            return new HelloResponse { Result = "Hello, " + request.Name };
        }
    }


    public class Global : System.Web.HttpApplication
    {
        /// Web Service Singleton AppHost
        public class HelloAppHost : AppHostBase
        {
            //Tell Service Stack the name of your application and where to find your web services
            public HelloAppHost()
                : base("Hello Web Services", typeof(HelloService).Assembly) { }

            public override void Configure(Funq.Container container)
            {
                container.Register<ICacheClient>(new MemoryCacheClient());

                Plugins.Add(new AuthFeature(
                    () => new AuthUserSession(),
                    new IAuthProvider[] { new BasicAuthProvider() }
                  ));

                var userRep = new InMemoryAuthRepository();
                container.Register<IUserAuthRepository>(userRep);

                //Add a user for testing purposes
                string hash;
                string salt;
                new SaltedHash().GetHashAndSaltString("test", out hash, out salt);
                userRep.CreateUserAuth(new UserAuth
                {
                    Id = 1,
                    DisplayName = "DisplayName",
                    Email = "as@if.com",
                    UserName = "john",
                    FirstName = "FirstName",
                    LastName = "LastName",
                    PasswordHash = hash,
                    Salt = salt,
                }, "test");
            }

        }


        protected void Application_Start(object sender, EventArgs e)
        {
            //Initialize your application
            var appHost = new HelloAppHost();
            appHost.Init();
        }
    }

   



        public void TEST()
        {

        }
}