﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace NeoMir.Classes
{
    /// <summary>
    /// Class to Communicate with WebView's App content.
    /// </summary>
    public class Communicate
    {
        //PROPERTIES

        //METHODS

        /// <summary>
        /// Call an available function inside the webview.
        /// </summary>
        /// <param name="webView">The concerned webview.</param>
        /// <param name="functionName">The name of the function to call.</param>
        /// <param name="args">The parameters of the function => example : {"param1", "param2", "param3"}</param>
        async public static void CallFunction(WebView webView, string functionName, string[] args)
        {
            string returnValue = await webView.InvokeScriptAsync(functionName, args);
        }

        /// <summary>
        /// Inject content to the WebView.
        /// </summary>
        /// <param name="webView">The concerned WebView.</param>
        /// <param name="content">The content to inject.</param>
        async public static void InjectContent(WebView webView, string content)
        {
            await webView.InvokeScriptAsync("eval", new string[] { content });
        }

        // Webview's event handler response (calls 'window.external.notify' from the webview)
        public static void ScriptNotify(object sender, NotifyEventArgs e)
        {
            // TODO
            // Add the URI in the app manifest
            // Get the response and display it for checking errors.
        }

        /// <summary>
        /// Go back in the navigation of the webview.
        /// </summary>
        /// <param name="webView">The concerned webview</param>
        public static void GoBack(WebView webView)
        {
            webView.GoBack();
        }

        /// <summary>
        /// Go forward in the navigation of the webview.
        /// </summary>
        /// <param name="webView">The concerned webview</param>
        public static void GoForward(WebView webView)
        {
            webView.GoForward();
        }
    }
}