using System;

namespace Minesweeper.GameServices.Utilities.Providers
{
    public interface IGuidProvider
    {
        Guid GenerateGuid();

        string GenerateGuidString();
    }
}