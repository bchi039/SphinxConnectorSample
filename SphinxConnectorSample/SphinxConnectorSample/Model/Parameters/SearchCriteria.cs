namespace SphinxConnectorSample.Model.Parameters { 
    public class SearchCriteria
    {
        public SearchCriteria()
        {
            Bedrooms = new Range<int>(null, null);
        }

        public SearchCriteria(SearchParameters @params) : this()
        {
            Bedrooms = new Range<int>(@params.BedroomFrom, @params.BedroomTo);
        }

        public Range<int> Bedrooms { get; }
    }
}
