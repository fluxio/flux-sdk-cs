using Flux.SDK.Properties;
using System;

namespace Flux.SDK
{
    /// <summary>
    /// Provides links to common Flux urls.
    /// </summary>
    public static class FluxLinkSDKExtension
    {
        /// <summary>
        /// Returns link to Flux Download url.
        /// </summary>
        /// <param name="sdk"></param>
        /// <returns>Link to Flux Download url.</returns>
        public static Uri GetDownloadUri(this FluxSDK sdk)
        {
            return new Uri(sdk.FluxUri + FluxApiData.DownloadUrl);
        }

        /// <summary>
        /// Returns link to Flux signup url.
        /// </summary>
        /// <param name="sdk"></param>
        /// <returns>Link to Flux signup url.</returns>
        public static Uri GetRegisterUri(this FluxSDK sdk)
        {
            return new Uri(sdk.FluxUri + FluxApiData.RegisterUrl);
        }

        /// <summary>
        /// Returns link to Flux Release notes password url.
        /// </summary>
        /// <param name="sdk"></param>
        /// <returns>Link to Flux Release notes.</returns>
        public static Uri GetReleaseNotesUri(this FluxSDK sdk)
        {
            return new Uri(sdk.FluxUri + FluxApiData.ReleaseNotesUrl);
        }

        /// <summary>
        /// Returns link to Forget password url.
        /// </summary>
        /// <param name="sdk"></param>
        /// <returns>Link to Forget password url.</returns>
        public static Uri GetForgetUri(this FluxSDK sdk)
        {
            return new Uri(sdk.FluxUri + FluxApiData.ForgetUrl);
        }

        /// <summary>
        /// Returns Flux help link.
        /// </summary>
        /// <param name="sdk"></param>
        /// <returns>Flux help link.</returns>
        public static Uri GetHelpUri(this FluxSDK sdk)
        {
            return new Uri(FluxApiData.HelpUrl);
        }

        /// <summary>
        /// Returns link to My Profile at Flux.
        /// </summary>
        /// <param name="sdk"></param>
        /// <returns>Link to My Profile at Flux.</returns>
        public static Uri GetMyProfileUri(this FluxSDK sdk)
        {
            return new Uri(sdk.FluxUri + FluxApiData.MyProfileUrl);
        }
    }
}
