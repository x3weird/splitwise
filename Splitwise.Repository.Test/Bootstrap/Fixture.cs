using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Splitwise.Repository.Test.Bootstrap
{
    [CollectionDefinition("Register Dependency")]
    public class Fixture : ICollectionFixture<Initialize>
    {
    }
}
