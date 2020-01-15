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
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KeyVault.Net.Controllers
{
    public class HomeController : Controller
    {
        private AzureServiceTokenProvider _azureServiceTokenProvider = null;
        private KeyVaultClient _keyVaultClient = null;

        public HomeController()
        {
            // Initialize the KeyVaultClient
            // The AzureServiceTokenProvider allows us to access the KeyVault using the 
            // application's Managed Service Identity
            _azureServiceTokenProvider = new AzureServiceTokenProvider();
            _keyVaultClient = new KeyVaultClient(
                new KeyVaultClient.AuthenticationCallback(
                    _azureServiceTokenProvider.KeyVaultTokenCallback));
        }

        public ActionResult Index()
        {
            // Retrieve the secret value from the KeyVault
            SecretBundle mySecretBundle = _keyVaultClient.GetSecretAsync(ConfigurationManager.AppSettings["KEYVAULT_ENDPOINT"], "MySecret").Result;
            ViewBag.MySecret = mySecretBundle.Value;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}