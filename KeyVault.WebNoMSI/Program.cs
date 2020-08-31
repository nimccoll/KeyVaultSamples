//===============================================================================
// Microsoft FastTrack for Azure
// Azure Key Vault Samples
//===============================================================================
// Copyright © Microsoft Corporation.  All rights reserved.
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
//===============================================================================
using Microsoft.AspNetCore.Hosting;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureKeyVault;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;

namespace KeyVault.WebNoMSI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, builder) =>
            {
                var keyVaultEndpoint = GetKeyVaultEndpoint();
                if (!string.IsNullOrEmpty(keyVaultEndpoint))
                {
                    // Authenticate to Key Vault using a Client Id and Client Secret
                    var keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
                    {
                        AuthenticationContext authContext = new AuthenticationContext(authority);
                        ClientCredential clientCred = new ClientCredential(GetClientId(), GetClientSecret());
                        AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

                        if (result == null)
                            throw new InvalidOperationException("Failed to obtain the JWT token");

                        return result.AccessToken;
                    });
                    builder.AddAzureKeyVault(
                        keyVaultEndpoint, keyVaultClient, new DefaultKeyVaultSecretManager());
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });

        private static string GetKeyVaultEndpoint() => Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT");
        private static string GetClientId() => Environment.GetEnvironmentVariable("CLIENT_ID");
        private static string GetClientSecret() => Environment.GetEnvironmentVariable("CLIENT_SECRET");

    }
}
