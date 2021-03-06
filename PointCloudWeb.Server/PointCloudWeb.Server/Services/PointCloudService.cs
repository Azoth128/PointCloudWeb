using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PointCloudWeb.Server.Models;
using PointCloudWeb.Server.Utils;

namespace PointCloudWeb.Server.Services
{
    public class PointCloudService
    {
        private readonly IPointCloudRegistrationService _pointCloudRegistration;
        private readonly PointCloudCollection _pointClouds;

        public PointCloudService( IPointCloudRegistrationService pointCloudRegistration)
        {
            _pointClouds = new PointCloudCollection();
            _pointCloudRegistration = pointCloudRegistration;
            InitSampleData();
        }

        private void GeneratePotreeData(Guid id)
        {
            var pathTarget = Globals.PotreeDataPath + $"/{id.ToString()}";
            var tempFile = Globals.TempPath + $"/{id}.las";

            var pc = _pointClouds.GetById(id);
            pc.WriteToLas(tempFile);

            var potreeConverter = new Process();
            potreeConverter.StartInfo.FileName = Globals.PotreeConverterExe;
            potreeConverter.StartInfo.Arguments = $"\"{tempFile}\" -o \"{Globals.TempPath}/{id.ToString()}\"";
            potreeConverter.Start();
            potreeConverter.WaitForExit();

            Directory.Move(Globals.TempPath + "/" + id, pathTarget);
        }

        private void InitSampleData()
        {
            EthTestData.CreateData(this);
        }

        private void RaiseIfNotExists(Guid id)
        {
            if (!_pointClouds.Contains(id))
                throw new ArgumentOutOfRangeException($"The Id {id.ToString()} was not found!");
        }

        public void AddPoints(Guid id, IEnumerable<Point> points)
        {
            var pc = _pointClouds.GetById(id)
                     ?? AddPointCloud(id);

            foreach (var point in points)
            {
                if (point.X != 0 || point.Y != 0 || point.Z != 0)
                    pc.Points.Add(point);
            }
        }

        public PointCloud AddPointCloud(Guid? id = null)
        {
            if (_pointClouds.Contains(id))
                throw new ArgumentOutOfRangeException($"Add of existing id \"{id}\" not possible!");

            var pc = new PointCloud(id ?? Guid.NewGuid(), $"Scan #{_pointClouds.Count + 1}");
            _pointClouds.Add(pc);
            return pc;
        }

        public IEnumerable<PointCloud> GetAll() => _pointClouds;

        public PointCloud GetById(Guid id) => _pointClouds.GetById(id);

        private void RegisterPointCloud(Guid id)
        {
            RaiseIfNotExists(id);

            var source = GetById(id);
            
            //the first can't be registered
            if (_pointClouds.IndexOf(source) == 0)
                return;
            
            var target = _pointClouds[_pointClouds.IndexOf(source) - 1];

            var result = _pointCloudRegistration.RegisterPointCloud(source, target);
            
            source.Rotation = result.Rotation;
            source.Transformation = result.Transformation;
        }

        public void RegisterPointClouds()
        {
            foreach (var pointCloud in _pointClouds)
                RegisterPointCloud(pointCloud.Id);
        }

        public void RemoveById(Guid id)
        {
            _pointClouds.RemoveById(id);
        }

        public void PointCloudCompleted(Guid id)
        {
            GeneratePotreeData(id);
            RegisterPointCloud(id);
        }
    }
}