using JGUZDV.Extensions.SAML2.Certificates;
using JGUZDV.Extensions.SAML2.Metadata;

using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection;


/// <summary>
/// Extensions for setting up SAML2 services in an <see cref="IServiceCollection" />.
/// </summary>
public static class JGUZDVSaml2ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        /// Adds a metadata manager and returns an options builder for the relying party options. 
        /// The metadata manager will use the specified metadata loader to load the metadata for the relying parties.
        /// </summary>
        public OptionsBuilder<RelyingPartyOptions> AddSaml2MetadataManager<TSPMetadata, TLoader>()
            where TLoader : MetadataLoader<TSPMetadata>
        {
            services.AddSingleton<MetadataLoader<TSPMetadata>, TLoader>();
            services.AddHostedService<MetadataManager<TSPMetadata>>();
            services.AddSingleton<MetadataContainer<TSPMetadata>>();

            return services.AddOptions<RelyingPartyOptions>(typeof(TSPMetadata).FullName);
        }

        /// <summary>
        /// Adds a certificate manager and returns the service collection. 
        /// The certificate manager will be used to manage the certificates used for signing and encryption in SAML2.
        /// </summary>
        /// <returns></returns>
        public IServiceCollection AddSaml2CertificateManager()
        {
            services.AddSingleton<CertificateContainer>();
            services.AddHostedService<CertificateManager>();

            return services;
        }
    }
}
