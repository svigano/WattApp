﻿using BuildingApi;
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
        private readonly string apiBaseUrl;
        private readonly ITokenProvider tokens;
        public SampleClient(ITokenProvider tokenProvider, string buildingApiUrl)
        {
            this.tokens = tokenProvider;
            this.apiBaseUrl = buildingApiUrl;
        }

        public string APIBaseUrl { get { return apiBaseUrl; } }

        public IEnumerable<Sample> GetSamples(string ptId, DateTime startDate, DateTime endDate, Company company)
        {
            var url = apiBaseUrl.AppendPathSegment("building/points").AppendPathSegment(ptId).AppendPathSegment("Samples").SetQueryParams(new
            {
                _startTime = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                _endTime = endDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                _interval = "Auto"
            }).ToString();
            Console.WriteLine(string.Format("request url -> {0}", url));
            var resp = HttpHelper.Get<Page<Sample>>(company, url, tokens);
            return (resp == null || resp.Items == null) ? new List<Sample>() : resp.Items;            
        }

    }
}
