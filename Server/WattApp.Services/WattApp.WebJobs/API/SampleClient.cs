using BuildingApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;

namespace WattApp.WebJobs.API
{
    public class SampleClient
    {
        private readonly string _apiBaseUrl;
        private readonly ITokenProvider _tokenProvider;
        public SampleClient(ITokenProvider tokenProvider, string buildingApiUrl)
        {
            this._tokenProvider = tokenProvider;
            this._apiBaseUrl = buildingApiUrl;
        }

        public string APIBaseUrl { get { return _apiBaseUrl; } }

        public IEnumerable<Sample> GetSamples(string ptId, DateTime startDate, DateTime endDate, Company company)
        {
            var url = _apiBaseUrl.AppendPathSegment("building/points").AppendPathSegment(ptId).AppendPathSegment("Samples").SetQueryParams(new
            {
                _startTime = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                _endTime = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                _interval = "Auto"
            }).ToString();
            return _executeGetRequest(url, company);
        }

        public IEnumerable<Sample> GetSamples(string ptId, DateTime startDate, Company company)
        {
            var url = _apiBaseUrl.AppendPathSegment("building/points").AppendPathSegment(ptId).AppendPathSegment("Samples").SetQueryParams(new
            {
                _startTime = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                _interval = "Auto"
            }).ToString();

            return _executeGetRequest(url, company);
        }

        private IEnumerable<Sample> _executeGetRequest(string url, Company company)
        {
            Console.WriteLine(string.Format("request url -> {0}", url));
            var resp = HttpHelper.Get<Page<Sample>>(company, url, _tokenProvider);
            return (resp == null || resp.Items == null) ? new List<Sample>() : resp.Items;
        }

    }
}
