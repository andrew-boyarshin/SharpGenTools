﻿using SharpGen.CppModel;
using SharpGen.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace SharpGen.UnitTests.Mapping
{
    public class Interface : MappingTestBase
    {
        public Interface(ITestOutputHelper outputHelper) : base(outputHelper)
        {
        }

        [Fact]
        public void Simple()
        {
            var config = new Config.ConfigFile
            {
                Id = nameof(Simple),
                Assembly = nameof(Simple),
                Namespace = nameof(Simple),
                Includes =
                {
                    new Config.IncludeRule
                    {
                        File = "interface.h",
                        Attach = true,
                        Namespace = nameof(Simple)
                    }
                },
                Bindings =
                {
                    new Config.BindRule("int", "System.Int32")
                }
            };

            var iface = new CppInterface
            {
                Name = "Interface",
                TotalMethodCount = 1
            };

            iface.Add(new CppMethod
            {
                Name = "method",
                ReturnValue = new CppReturnValue
                {
                    TypeName = "int"
                }
            });

            var include = new CppInclude
            {
                Name = "interface"
            };

            include.Add(iface);

            var module = new CppModule();
            module.Add(include);

            var (solution, _) = MapModel(module, config);

            Assert.Single(solution.EnumerateDescendants().OfType<CsInterface>());

            var csIface = solution.EnumerateDescendants().OfType<CsInterface>().First();

            Assert.Single(csIface.Methods);

            var method = csIface.Methods.First();

            Assert.Equal(0, method.Offset);

            Assert.IsType<CsFundamentalType>(method.ReturnValue.PublicType);

            Assert.Equal(typeof(int), ((CsFundamentalType)method.ReturnValue.PublicType).Type);
        }
    }
}