using Microsoft.Identity.Web;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using System.Net.Http.Headers;

namespace LoginMVC.Services
{

    public class GraphAuthProvider : IAuthenticationProvider
    {
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly string[] _scopes;

        public GraphAuthProvider(ITokenAcquisition tokenAcquisition, string[] scopes)
        {
            _tokenAcquisition = tokenAcquisition;
            _scopes = scopes;
        }

        public async Task AuthenticateRequestAsync(
            RequestInformation request,
            Dictionary<string, object>? additionalAuthenticationContext = null,
            CancellationToken cancellationToken = default)
        {
            var accessToken = await _tokenAcquisition.GetAccessTokenForUserAsync(
                _scopes,
                authenticationScheme: Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectDefaults.AuthenticationScheme,
                tenantId: null,
                user: null,
                tokenAcquisitionOptions: null
            );

            request.Headers["Authorization"] = new List<string> { $"Bearer {accessToken}" };
        }
    }
}
