﻿using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using CupcakeFactory.ServiceProxy;
using CupcakeFactory.ServiceProxy.Dispatchers;
using CupcakeFactory.ServiceProxy.Serializers;
using CupcakeFactory.ServiceProxy.Tests;

namespace MyBenchmarks
{
    [RPlotExporter]
    public class ProxyBaselines
    {
        ITestService jsonDotNetProxy;

        public ProxyBaselines()
        {
            IDispatch jsonDotNetDispatcher = new InProcDispatcher<ITestService>(new JsonProxySerializer<ITestService>(), new TestService());
            jsonDotNetProxy = ServiceProxy<ITestService>.GetProxy(jsonDotNetDispatcher);
        }

        [Benchmark]
        public async Task<SimpleObject> JsonDotNetWarmed() => await jsonDotNetProxy.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject());

        [Benchmark]
        public async Task<SimpleObject> JsonDotNetCold()
        {
            IDispatch coldJsonDotNetDispatcher = new InProcDispatcher<ITestService>(new JsonProxySerializer<ITestService>(), new TestService());
            var coldProxy = ServiceProxy<ITestService>.GetProxy(coldJsonDotNetDispatcher);

            return await coldProxy.DoWorkWithTaskAndMultipleUserTypeParameters(new SimpleObject() { Int = 321, Long = 123 }, new ComplexObject(), new SelfReferenceingObject());
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<ProxyBaselines>();
        }
    }
}