using ClassLibrary1.Facade;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public sealed class LoginToken
    {

        private static LoginToken _instance;
        private static object key = new object();
        private LoginService login;
        

        private LoginToken()
        {
            login = new LoginService();
        }
        public static LoginToken GetInstance()
        {
            if (_instance == null)
            {
                lock (key)
                {
                    if (_instance == null)
                    {
                        _instance = new LoginToken();
                    }
                }
            }
            return _instance;
        }



        public ILoginToken Login(string username, string password)
        {

            if (login.TryAdminLogin(username, password, out LoginToken<Administrator> adminToken))
            {
                return adminToken;
            }
            if (login.TryCustomerLogin(username, password, out LoginToken<Customer> customerToken))
            {
                return customerToken;
            }
            if (login.TryAirlineLogin(username, password, out LoginToken<AirlineCompany> airlineToken))
            {
                return airlineToken;
            }
            else
            {
                throw new UserNotFoundException($"user name:{username} and password:{password} doesn't match");
            }
        }

        public FacadeBase GetFacade(ILoginToken token)
        {
            if (token == null)
            {
                return new AnonymousUserFacade();
            }
            if (token.GetType() == typeof(LoginToken<Administrator>))
            {
                return new LoggedInAdministratorFacade();
            }
            if (token.GetType() == typeof(LoginToken<Customer>))
            {
                return new LoggedInCustomerFacade();
            }
            if (token.GetType() == typeof(LoginToken<AirlineCompany>))
            {
                return new LoggedInAirlineFacade();
            }
            return new AnonymousUserFacade();
        }



    }
}
