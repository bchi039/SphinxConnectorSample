using SphinxConnector.FluentApi;
using SphinxConnectorSample.Model.Parameters;
using SphinxConnectorSample.Model.Sphinx;

namespace SphinxConnectorSample.Services
{
    public interface IQueryBuilder
    {
        string BuildQuery(SearchCriteria criteria);
        void FilterQuery(IFulltextQuery<SphinxConnectorTest> sphinxQuery, SearchCriteria criteria);
    }

    public class QueryBuilder : IQueryBuilder
    {
        public string BuildQuery(SearchCriteria criteria)
        {
            return "@description bed* ";    //hardcoded something to get sphinx running
        }

        public void FilterQuery(IFulltextQuery<SphinxConnectorTest> sphinxQuery, SearchCriteria criteria)
        {
            FilterByBedrooms(sphinxQuery, criteria.Bedrooms);
        }

        private void FilterByBedrooms(IFulltextQuery<SphinxConnectorTest> sphinxQuery, Range<int> bedrooms)
        {
            if (bedrooms.From.HasValue)
                sphinxQuery.Where(x => x.Bedrooms >= bedrooms.From);

            if (bedrooms.To.HasValue)
                sphinxQuery.Where(x => x.Bedrooms <= bedrooms.To);
        }
    }
}
