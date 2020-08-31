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
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Configuration;
using System.Web.Mvc;

namespace KeyVault.NetNoMSI.Controllers
{
    public class HomeController : Controller
    {
        private KeyVaultClient _keyVaultClient = null;

        public HomeController()
        {
            // Authenticate to Key Vault using a Client Id and Client Secret
            _keyVaultClient = new KeyVaultClient(async (authority, resource, scope) =>
            {
                AuthenticationContext authContext = new AuthenticationContext(authority);
                ClientCredential clientCred = new ClientCredential(ConfigurationManager.AppSettings["ida.ClientID"], ConfigurationManager.AppSettings["ida.ClientSecret"]);
                AuthenticationResult result = await authContext.AcquireTokenAsync(resource, clientCred);

                if (result == null)
                    throw new InvalidOperationException("Failed to obtain the JWT token");

                return result.AccessToken;
            });
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