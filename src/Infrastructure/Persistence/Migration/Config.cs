using System.Collections.Generic;
using IdentityServer4.Models;
using static IdentityServer4.IdentityServerConstants;

namespace TenderManagement.Infrastructure.Persistence.Migration
{
    internal static class Config
    {
        internal static IEnumerable<Client> GetClients()
        {
            yield return new Client
            {
                ClientId = "default-client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { StandardScopes.OpenId, StandardScopes.Profile, "tender.read", "tender.write", "tender.delete" },
                AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                AllowOfflineAccess = true
            };
        }

        internal static IEnumerable<ApiResource> GetApiResources()
        {
            yield return new ApiResource("tender", "Tender API")
            {
                Scopes = { "tender.read", "tender.write", "tender.delete" }
            };
        }

        internal static IEnumerable<ApiScope> GetApiScopes()
        {
            yield return new ApiScope("tender.read", "Read Tender API");
            yield return new ApiScope("tender.write", "Write Tender API");
            yield return new ApiScope("tender.delete", "Delete Tender API");
        }

        public static IEnumerable<IdentityResource> DefaultResources()
        {
            yield return new IdentityResources.Address();
            yield return new IdentityResources.OpenId();
            yield return new IdentityResources.Email();
            yield return new IdentityResources.Profile();
            yield return new IdentityResources.Phone();
        }
    }
}