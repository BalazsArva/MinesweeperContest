using System;

namespace Minesweeper.GameServices.Utilities.Providers
{
    public class GuidProvider : IGuidProvider
    {
        public Guid GenerateGuid()
        {
            return Guid.NewGuid();
        }

        public string GenerateGuidString()
        {
            return Guid.NewGuid().ToString();
        }
    }
}