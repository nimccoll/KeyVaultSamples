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
using KeyVault.WebNoMSI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Diagnostics;

namespace KeyVault.WebNoMSI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private IConfiguration _configuration;
        private KeyVaultClient _keyVaultClient;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            // Authenticate to Key Vault using a Client Id and Client Secret
            _keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
            {
                AuthenticationContext authContext = new AuthenticationContext(authority);
                ClientCredential clientCred = new ClientCredential(Environment.GetEnvironmentVariable("CLIENT_ID"), Environment.GetEnvironmentVariable("CLIENT_SECRET"));
                AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                    throw new InvalidOperationException("Failed to obtain the JWT token");

                return result.AccessToken;
            });
        }

        public IActionResult Index()
        {
            // Option 1 - Retrieve the secret from configuration which was loaded from the Key Vault
            // Configuration does not automatically get reloaded when the Key Vault is updated.
            string mySecret = _configuration.GetValue<string>("MySecret");

            // Option 2 - Make a direct call to the Key Vault to retrieve the secret
            var mySecretBundle = _keyVaultClient.GetSecretAsync(Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT"), "MySecret").Result;
            var mySecretString = mySecretBundle.Value;

            ViewBag.MySecret = mySecretString;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
