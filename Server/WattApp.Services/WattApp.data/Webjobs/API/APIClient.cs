using BuildingApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;

namespace WattApp.WebJobs.API
{
    public class APIClient
    {
        private readonly string _apiBaseUrl;
        private readonly ITokenProvider _tokenProvider;
        private readonly EquipmentClient _equipmentClient;
        public APIClient(ITokenProvider tokenProvider, string buildingApiUrl)
        {
            this._tokenProvider = tokenProvider;
            this._apiBaseUrl = buildingApiUrl;
            _equipmentClient = new EquipmentClient(_tokenProvider, _apiBaseUrl);
        }

        public string APIBaseUrl { get { return _apiBaseUrl; } }
        public EquipmentClient EquipmentClient { get { return _equipmentClient; } }


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
