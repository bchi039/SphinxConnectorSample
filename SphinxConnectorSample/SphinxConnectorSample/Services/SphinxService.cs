using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SphinxConnector;
using SphinxConnector.Common;
using SphinxConnector.FluentApi;
using SphinxConnectorSample.Config;
using SphinxConnectorSample.Model.Parameters;
using SphinxConnectorSample.Model.Sphinx;

namespace SphinxConnectorSample.Services
{
    public interface ISphinxService
    {
        IList<SphinxConnectorTest> Search(SearchCriteria criteria);
    }

    public class SphinxService : ISphinxService
    {
        private readonly IFulltextStore _fulltextStore;
        private readonly SphinxConfig _sphinxConfig;
        private readonly IQueryBuilder _queryBuilder;
        private readonly ILogger _logger;

        public SphinxService(IOptions<SphinxConfig> sphinxConfigOption, IFulltextStore fulltextStore, IQueryBuilder queryBuilder, ILogger<SphinxService> logger)
        {
            _sphinxConfig = sphinxConfigOption.Value;
            _logger = logger;

            _queryBuilder = queryBuilder;
            _fulltextStore = fulltextStore;

            Initialise();
        }

        private void Initialise()
        {
            try
            {
                var defaultFluentApiPort = 9306;
                var connectionHost = _sphinxConfig.Host ?? "localhost";
                var connectionPort = _sphinxConfig.Port > 0 ? _sphinxConfig.Port : defaultFluentApiPort;

                var defaultEmptyKey = "key";
                if (!string.IsNullOrEmpty(_sphinxConfig.License) && _sphinxConfig.License != defaultEmptyKey)
                    SphinxConnectorLicensing.SetLicense(_sphinxConfig.License);
                else
                    _logger.LogError("Sphinx license was not provided. You are using the trial version of sphinx connector.");

                _fulltextStore.ConnectionString.IsThis(
                    $"Data Source={connectionHost};Port={connectionPort};Pooling=true");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to initialise Sphinx license");
            }
        }

        public IList<SphinxConnectorTest> Search(SearchCriteria criteria)
        {
            using (var session = _fulltextStore.StartSession())
            {
                var query = _queryBuilder.BuildQuery(criteria);

                var sphinxQuery = session.Query<SphinxConnectorTest>()
                        .Options(SetOptions)
                        .Metadata(out var meta)
                        .Match(query)
                    ;

                _queryBuilder.FilterQuery(sphinxQuery, criteria);

                var results = sphinxQuery.ToList();

                return results;
            }
        }

        private static void SetOptions<T>(IFulltextQueryOptions<T> par)
        {
            par.MaxMatches(5000);
            par.Ranker(SphinxRankMode.SPH04);
        }
    }
}
