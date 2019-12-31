using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StructureMap;
using StructureMap.Diagnostics;

namespace BloggerAPI.IntegrationTests
{
    public sealed class ContainerFixture : IDisposable
    {
        public IContainer GetContainer => _container;

        private readonly IContainer _container;
        public ContainerFixture()
        {
            _container = new Container(x => x.AddRegistry(new StructureMapRegistry()));
        }
        public void Dispose()
        {
            this._container.Dispose();
        }
    }
}
