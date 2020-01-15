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
using KeyVault.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;

namespace KeyVault.Web.Controllers
{
    public class HomeController : Controller
    {
        private IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            // Option 1 - Retrieve the secret from configuration which was loaded from the Key Vault
            // Configuration does not automatically get reloaded when the Key Vault is updated.
            string mySecret = _configuration.GetValue<string>("MySecret");

            // Option 2 - Make a direct call to the Key Vault to retrieve the secret
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    azureServiceTokenProvider.KeyVaultTokenCallback));
            var mySecretBundle = keyVaultClient.GetSecretAsync(Environment.GetEnvironmentVariable("KEYVAULT_ENDPOINT"), "MySecret").Result;
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
