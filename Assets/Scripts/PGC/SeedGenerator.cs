using System;

public static class SeedGenerator
{
    private static Random _random = new Random();
    public static int GenerateSeed()
    {
        return _random.Next(
            int.MinValue,
            int.MaxValue
        );
    }
}