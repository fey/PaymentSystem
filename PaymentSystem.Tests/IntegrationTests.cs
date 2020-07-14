using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Mvc.Testing;
using PaymentSystem.Model.Dto.Auth;
using PaymentSystem.Model.Dto.Payments;
using Xunit;

namespace PaymentSystem.Tests
{
    public class IntegrationTests
    {
        private readonly WebApplicationFactory<Startup> _factory;

        private static string paymentSessionRoute = "/api/payment/session";
        private static string paymentInitiationRoute = "/api/payment/initiate";
        private static string paymentHistoryRoute = "/api/payment/history";

        private static string userRegistrationRoute = "/api/user/register";
        private static string userLoginRoute = "/api/user/login";
        private static string userLogoutRoute = "/api/user/logout";

        private static string openApiSpecRoute = "/swagger/v1/swagger.json";

        public IntegrationTests() =>
            _factory = new WebApplicationFactory<Startup>();

        [Fact]
        public async void ShouldNotGiveSessionIdWithoutPayment()
        {
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(paymentSessionRoute);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Theory, AutoData]
        public async void ShouldGiveSessionIdForPayment(PaymentRequest payment)
        {
            HttpClient client = _factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, paymentSessionRoute)
            {
                Content = JsonContent.Create(payment)
            };
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.True(response.IsSuccessStatusCode);
            Assert.IsType<Guid>(await response.Content.ReadFromJsonAsync(typeof(Guid)));
        }

        [Fact]
        public async void ShouldNotGiveSessionIdForWrongPayment()
        {
            HttpClient client = _factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, paymentSessionRoute)
            {
                Content = JsonContent.Create(new PaymentRequest(){ Sum = -10, Purpose = "Just for lulz" })
            };
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory, AutoData]
        public async void ShouldNotMakePaymentForInvalidSession(Card card)
        {
            HttpClient client = _factory.CreateClient();
            string requestUrl = $"{paymentInitiationRoute}?sessionId={Uri.EscapeDataString(Guid.Empty.ToString())}";
            HttpResponseMessage response = await client.PostAsJsonAsync(requestUrl, card);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory, AutoData]
        public async void ShouldNotMakePaymentForInvalidCard(PaymentRequest payment)
        {
            HttpClient client = _factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, paymentSessionRoute)
            {
                Content = JsonContent.Create(payment)
            };
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.True(response.IsSuccessStatusCode);
            Guid sessionId = Assert.IsType<Guid>(await response.Content.ReadFromJsonAsync(typeof(Guid)));
            Card invalidCard = new Card()
                {
                    Number = "4a561261212345464",
                    SecurityCode = "404"
                };
            string requestUrl = $"{paymentInitiationRoute}?sessionId={Uri.EscapeDataString(sessionId.ToString())}";
            response = await client.PostAsJsonAsync(requestUrl, invalidCard);
            var content = await response.Content.ReadAsStringAsync();
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory, AutoData]
        public async void ShouldMakePaymentForValidCard(PaymentRequest payment)
        {
            HttpClient client = _factory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, paymentSessionRoute)
            {
                Content = JsonContent.Create(payment)
            };
            HttpResponseMessage response = await client.SendAsync(request);
            Assert.True(response.IsSuccessStatusCode);
            Guid sessionId = Assert.IsType<Guid>(await response.Content.ReadFromJsonAsync(typeof(Guid)));
            Card validCard = new Card()
                {
                    Number = "4561261212345467",
                    SecurityCode = "404"
                };
            string requestUrl = $"{paymentInitiationRoute}?sessionId={Uri.EscapeDataString(sessionId.ToString())}";
            response = await client.PostAsJsonAsync(requestUrl, validCard);
            var content = await response.Content.ReadAsStringAsync();
            Assert.True(response.IsSuccessStatusCode);
        }
        
        [Theory, AutoData]
        public async void ShouldNotRegisterInvalidUser(string login, string password)
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = login,
                Password = password,
                PasswordConfirmation = $"{password}{password}"
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Theory, AutoData]
        public async void ShouldRegisterValidUser(string login, string password)
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = login,
                Password = password,
                PasswordConfirmation = password
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Theory, AutoData]
        public async void ShouldNotRegisterSameUser(string login, string password)
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = login,
                Password = password,
                PasswordConfirmation = password
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            Assert.False(response.IsSuccessStatusCode);
        }

        [Theory, AutoData]
        public async void ShouldNotLoginInexistentUser(LoginModel credentials)
        {
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.PostAsJsonAsync(userLoginRoute, credentials);
            Assert.False(response.IsSuccessStatusCode);
            Assert.False(response.Headers.Contains("Set-Cookie"));
        }

        [Theory, AutoData]
        public async void ShouldLoginExistingUser(LoginModel credentials)
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = credentials.Login,
                Password = credentials.Password,
                PasswordConfirmation = credentials.Password
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            response = await client.PostAsJsonAsync(userLoginRoute, credentials);
            Assert.True(response.IsSuccessStatusCode);
            Assert.True(response.Headers.Contains("Set-Cookie"));
        }

        [Theory, AutoData]
        public async void ShouldProperlyLogout(LoginModel credentials)
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = credentials.Login,
                Password = credentials.Password,
                PasswordConfirmation = credentials.Password
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            response = await client.PostAsJsonAsync(userLoginRoute, credentials);
            response = await client.GetAsync(userLogoutRoute);
            Assert.True(!response.Headers.Contains("Set-Cookie") ||
                response.Headers.GetValues("Set-Cookie")
                    .Any(header => header.Contains(".AspNetCore.Cookies=;")));
        }

        [Theory, AutoData]
        public async void ShouldNotLoginUserWithWrongPassword(LoginModel credentials)
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = credentials.Login,
                Password = $"пароль:{credentials.Password}",
                PasswordConfirmation = $"пароль:{credentials.Password}"
            };
            HttpResponseMessage response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            response = await client.PostAsJsonAsync(userLoginRoute, credentials);
            Assert.False(response.IsSuccessStatusCode);
            Assert.False(response.Headers.Contains("Set-Cookie"));
        }

        [Fact]
        public async void ShouldNotGivePaymentHistoryToUnauthenticatedUser()
        {
            HttpClient client = _factory.CreateClient();
            string historyUrl = 
                $"{paymentHistoryRoute}?periodStart={Uri.EscapeDataString("06/20")}"+
                $"&periodEnd={Uri.EscapeDataString("08/20")}";
            HttpResponseMessage response = await client.GetAsync(historyUrl);
            Assert.Equal(HttpStatusCode.MethodNotAllowed, response.StatusCode);
        }

        [Theory, AutoData]
        public async void ShouldGivePaymentHistoryToAuthenticatedUser(
            LoginModel credentials, PaymentRequest payment
        )
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = credentials.Login,
                Password = credentials.Password,
                PasswordConfirmation = credentials.Password
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, paymentSessionRoute)
            {
                Content = JsonContent.Create(payment)
            };
            HttpResponseMessage response = await client.SendAsync(request);
            response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            response = await client.PostAsJsonAsync(userLoginRoute, credentials);
            string historyUrl = $@"{paymentHistoryRoute}?
            periodStart={Uri.EscapeDataString("06/20")}&periodEnd={Uri.EscapeDataString("08/20")}";
            response = await client.GetAsync(historyUrl);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            List<PaymentRecord> payments = await response.Content.ReadFromJsonAsync<List<PaymentRecord>>();
            Assert.Collection(payments, item => {
                Assert.Equal(payment.Sum, item.PaymentSum);
                Assert.Equal( payment.Purpose, item.Purpose);
            });
        }

        [Theory, AutoData]
        public async void ShouldNotGivePaymentHistoryOutOfPeriod(
            LoginModel credentials, PaymentRequest payment
        )
        {
            HttpClient client = _factory.CreateClient();
            RegisterModel regInfo = new RegisterModel()
            {
                Username = credentials.Login,
                Password = credentials.Password,
                PasswordConfirmation = credentials.Password
            };
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, paymentSessionRoute)
            {
                Content = JsonContent.Create(payment)
            };
            HttpResponseMessage response = await client.SendAsync(request);
            response = await client.PostAsJsonAsync(userRegistrationRoute, regInfo);
            response = await client.PostAsJsonAsync(userLoginRoute, credentials);
            string historyUrl = $@"{paymentHistoryRoute}?
            periodStart={Uri.EscapeDataString("05/20")}&periodEnd={Uri.EscapeDataString("06/20")}";
            response = await client.GetAsync(historyUrl);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            List<PaymentRecord> payments = await response.Content.ReadFromJsonAsync<List<PaymentRecord>>();
            Assert.Empty(payments);
        }

        [Fact]
        public async void ShouldGiveOpenApiSpec()
        {
            HttpClient client = _factory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(openApiSpecRoute);
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}