using System;
using System.Collections.Generic;
using PointCloudWeb.Server.Models;

namespace PointCloudWeb.Server.Services
{
    public class DataService
    {
        private readonly PointCloudService _pointCloudService;
        private readonly ScanConverterService _scanConverterService;

        public DataService(PointCloudService pointCloudService, ScanConverterService scanConverterService)
        {
            _pointCloudService = pointCloudService;
            _scanConverterService = scanConverterService;
        }

        public void AddScan(ScanDataList scanData)
        {
            _pointCloudService.AddPoints(scanData.Id, ConvertToPoints(scanData));
        }

        private IList<Point> ConvertToPoints(ScanDataList scanData)
        {
            var list = new List<Point>();

            foreach (var scan in scanData.List)
            {
                list.Add(_scanConverterService.Transform(scan));
            }

            return list;
        }
    }
}