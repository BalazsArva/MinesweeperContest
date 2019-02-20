using System;

namespace Minesweeper.GameServices.Providers
{
    public interface IGuidProvider
    {
        Guid GenerateGuid();

        string GenerateGuidString();
    }
}